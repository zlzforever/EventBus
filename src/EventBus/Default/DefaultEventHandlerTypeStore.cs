using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EventBus.Default
{
    public class DefaultEventHandlerTypeStore : IEventHandlerTypeStore
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>> _eventHandlerTypesDict;

        public DefaultEventHandlerTypeStore()
        {
            _eventHandlerTypesDict = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>>();
        }

        public bool Add(Type eventType, Type handlerType)
        {
            if (!_eventHandlerTypesDict.ContainsKey(eventType))
            {
                if (!_eventHandlerTypesDict.TryAdd(eventType, new ConcurrentDictionary<Type, object>()))
                {
                    return false;
                }
            }

            lock (_eventHandlerTypesDict)
            {
                return _eventHandlerTypesDict[eventType].ContainsKey(handlerType) ||
                       _eventHandlerTypesDict[eventType].TryAdd(handlerType, null);
            }
        }

        public bool Remove(Type eventType)
        {
            if (_eventHandlerTypesDict.ContainsKey(eventType))
            {
                return _eventHandlerTypesDict.TryRemove(eventType, out _);
            }

            return true;
        }

        public bool Remove(Type eventType, Type handlerType)
        {
            if (_eventHandlerTypesDict.TryGetValue(eventType, out var handlerTypes))
            {
                return handlerTypes.TryRemove(handlerType, out _);
            }

            return true;
        }

        public IEnumerable<Type> GetHandlerTypes(Type eventType)
        {
            if (_eventHandlerTypesDict.TryGetValue(eventType, out var handlerTypes))
            {
                return handlerTypes.Keys;
            }

            return Enumerable.Empty<Type>();
        }
    }
}