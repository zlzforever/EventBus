using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.Default;

namespace EventBus
{
    public class EventBus : IEventBus
    {
        private readonly IEventHandlerTypeStore _eventHandlerTypeStore;
        private readonly IHandlerFactory _handlerFactory;

        public EventBus() : this(new DefaultEventHandlerTypeStore(), new ActivatorHandlerFactory())
        {
        }

        public EventBus(IEventHandlerTypeStore eventHandlerTypeStore,
            IHandlerFactory handlerFactory)
        {
            _handlerFactory = handlerFactory;
            _eventHandlerTypeStore = eventHandlerTypeStore;
        }

        public bool Register<TEvent, TEventHandler>()
            where TEvent : class, IEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            var handlerType = typeof(TEventHandler);
            return Register<TEvent>(handlerType);
        }

        public bool Register<TEvent>(Type handlerType) where TEvent : class, IEvent
        {
            if (handlerType == null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }

            var handlerInterfaces = new[]
            {
                typeof(IEventHandler<TEvent>),
                typeof(IDynamicEventHandler)
            };
            if (handlerInterfaces.Any(x => x.IsAssignableFrom(handlerType)))
            {
                var eventType = typeof(TEvent);
                return _eventHandlerTypeStore.Add(eventType, handlerType);
            }
            else
            {
                throw new ArgumentException($"Type {handlerType} is not a valid handler");
            }
        }

        public bool Unregister<TEvent>() where TEvent : class, IEvent
        {
            var eventType = typeof(TEvent);
            return _eventHandlerTypeStore.Remove(eventType);
        }

        public bool Unregister<TEvent, TEventHandler>() where TEvent : class, IEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventType = typeof(TEvent);
            var eventHandlerType = typeof(TEventHandler);
            return _eventHandlerTypeStore.Remove(eventType, eventHandlerType);
        }

        public async Task PublishAsync(object @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            var eventType = @event.GetType();
            var handlerTypes = _eventHandlerTypeStore.GetHandlerTypes(eventType);
            foreach (var handlerType in handlerTypes)
            {
                var methodInfo = handlerType.GetMethod("HandleAsync");
                if (methodInfo != null)
                {
                    var handler = _handlerFactory.Create(handlerType);
                    if (handler != null)
                    {
                        if (methodInfo.Invoke(handler, new[] {@event}) is Task task)
                        {
                            await task;
                        }
                        else
                        {
                            throw new ApplicationException($"Handle method is not async method");
                        }
                    }
                    else
                    {
                        throw new ApplicationException($"Create handler {handlerType} object failed");
                    }
                }
            }
        }

        public IEnumerable<Type> GetHandlerTypes<TEvent>() where TEvent : class, IEvent
        {
            var eventType = typeof(TEvent);
            return _eventHandlerTypeStore.GetHandlerTypes(eventType);
        }
    }
}