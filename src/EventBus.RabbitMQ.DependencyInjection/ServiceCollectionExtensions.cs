using System;
using EventBus.Default;
using EventBus.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.RabbitMQ.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddRabbitMQEventBus(this IServiceCollection serviceCollection,
			params Type[] types)
		{
			serviceCollection.AddEventBus(types);
			serviceCollection.AddScoped<IEventBus, RabbitMQEventBus>();
			serviceCollection.AddScoped<IRabbitMQOptions, RabbitMQOptions>();
			return serviceCollection;
		}
	}
}