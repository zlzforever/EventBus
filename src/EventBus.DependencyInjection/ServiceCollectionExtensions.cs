using System;
using System.Linq;
using EventBus.Default;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection serviceCollection, params Type[] types)
        {
            serviceCollection.AddSingleton<IEventHandlerTypeStore, DefaultEventHandlerTypeStore>();
            serviceCollection.LoadHandlers(types);
            serviceCollection.AddScoped<IHandlerFactory, DependencyInjectionHandlerFactory>();
            serviceCollection.AddScoped<IEventBus, EventBus>();
            return serviceCollection;
        }

        public static IServiceCollection LoadHandlers(this IServiceCollection serviceCollection,
            params Type[] handlerTypes)
        {
            var types = handlerTypes.SelectMany(x => x.Assembly.GetTypes());

            var baseEventType = typeof(Event);

            foreach (var handlerType in types)
            {
                var eventType = handlerType.GetInterface("IEventHandler`1")?.GenericTypeArguments
                    .SingleOrDefault();
                if (eventType != null && baseEventType.IsAssignableFrom(baseEventType))
                {
                    serviceCollection.AddScoped(handlerType);
                }
            }

            return serviceCollection;
        }
    }
}