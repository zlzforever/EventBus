using System;

namespace EventBus.Default
{
    public class ActivatorHandlerFactory : IHandlerFactory
    {
        public object Create(Type handlerType)
        {
            return Activator.CreateInstance(handlerType);
        }
    }
}