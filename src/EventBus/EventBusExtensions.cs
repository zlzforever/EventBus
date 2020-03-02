using System;
using System.Linq;

namespace EventBus
{
	public static class EventBusExtensions
	{
		public static bool IsHandler(this Type handlerType)
		{
			if (typeof(IDynamicEventHandler).IsAssignableFrom(handlerType))
			{
				return true;
			}

			var eventType = handlerType.GetInterface("IEventHandler`1")?.GenericTypeArguments
				.SingleOrDefault();
			return eventType != null && eventType.IsEvent();
		}

		public static bool IsEvent(this Type eventType)
		{
			return typeof(Event).IsAssignableFrom(eventType);
		}
	}
}