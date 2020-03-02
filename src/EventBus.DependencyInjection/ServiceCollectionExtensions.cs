using System;
using System.Collections.Generic;
using System.Linq;
using EventBus.Default;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		private static readonly Dictionary<Type, HashSet<Type>> HandlerTypes = new Dictionary<Type, HashSet<Type>>();

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

			var dynamicHandler = typeof(IDynamicEventHandler);
			foreach (var handlerType in types)
			{
				if (handlerType.IsHandler())
				{
					serviceCollection.AddScoped(handlerType);
				}

				if (!dynamicHandler.IsAssignableFrom(handlerType))
				{
					var eventType = handlerType.GetInterface("IEventHandler`1")?.GenericTypeArguments
						.SingleOrDefault();
					if (eventType != null)
					{
						if (!HandlerTypes.ContainsKey(eventType))
						{
							HandlerTypes.Add(eventType, new HashSet<Type>());
						}

						HandlerTypes[eventType].Add(handlerType);
					}
				}
			}

			return serviceCollection;
		}

		public static IServiceProvider UseEventBus(this IServiceProvider serviceProvider)
		{
			using var scope = serviceProvider.CreateScope();
			var eventBus = scope.ServiceProvider.GetService<IEventBus>();
			if (eventBus != null)
			{
				foreach (var kv in HandlerTypes)
				{
					foreach (var handlerType in kv.Value)
					{
						eventBus.Register(kv.Key, handlerType);
					}
				}
			}

			return serviceProvider;
		}
	}
}