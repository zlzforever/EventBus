using System;

namespace EventBus
{
    public interface IHandlerFactory
    {
        object Create(Type handlerType);
    }
}