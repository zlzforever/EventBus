using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventBus
{
    public interface IEventBus
    {
        bool Register<TEvent, TEventHandler>()
            where TEvent : class, IEvent
            where TEventHandler : IEventHandler<TEvent>;

        bool Register<TEvent>(Type handlerType)
            where TEvent : class, IEvent;

        bool Unregister<TEvent>() where TEvent : class, IEvent;

        bool Unregister<TEvent, TEventHandler>() where TEvent : class, IEvent
            where TEventHandler : IEventHandler<TEvent>;

        Task PublishAsync(object @event);

        IEnumerable<Type> GetHandlerTypes<TEvent>() where TEvent : class, IEvent;
        
        long HandleCount { get; }
    }
}