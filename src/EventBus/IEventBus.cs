using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventBus
{
	public interface IEventBus
	{
		bool Register<TEvent, TEventHandler>()
			where TEvent : Event
			where TEventHandler : IEventHandler<TEvent>;

		bool Register<TEvent>(Type handlerType)
			where TEvent : Event;

		bool Register(Type eventType, Type handlerType);

		bool Unregister<TEvent>() where TEvent : Event;

		bool Unregister<TEvent, TEventHandler>() where TEvent : Event
			where TEventHandler : IEventHandler<TEvent>;

		Task PublishAsync(object @event);

		IEnumerable<Type> GetHandlerTypes<TEvent>() where TEvent : Event;

		long HandleCount { get; }

		IEventHandlerTypeStore EventHandlerTypeStore { get; }
	}
}