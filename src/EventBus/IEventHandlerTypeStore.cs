using System;
using System.Collections.Generic;

namespace EventBus
{
	public interface IEventHandlerTypeStore
	{
		bool Add(Type eventType, Type handlerType);
		bool Remove(Type eventType);
		bool Remove(Type eventType, Type handlerType);
		IEnumerable<Type> GetHandlerTypes(Type eventType);
		Dictionary<Type, List<Type>> GetAllHandlerTypes();
	}
}