using System.Threading.Tasks;
using EventBus.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EventBus.Tests
{
	public class DependencyInjectionTests : BasicTests
	{
		protected override IEventBus GetEventBus()
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddEventBus(typeof(DependencyInjectionTests));

			var services = serviceCollection.BuildServiceProvider();
			return services.GetRequiredService<IEventBus>();
		}

		[Fact]
		public override async Task PublishToDynamicEventHandler()
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddEventBus(typeof(DependencyInjectionTests));
			serviceCollection.AddScoped<TestEventDynamicEventHandler>();

			var services = serviceCollection.BuildServiceProvider();
			var eventBus = services.GetRequiredService<IEventBus>();
			eventBus.Register<TestEvent>(typeof(TestEventDynamicEventHandler));
			await eventBus.PublishAsync(new TestEvent("test-publish"));
			Assert.Equal("test-publish", TestEventDynamicEventHandler.Name);
		}

		protected override void RegisterConcurrentHandler(IEventBus eventBus)
		{
			eventBus.Register<TestEvent, TestEventHandler3>();
		}

		protected override int AssertConcurrentCounter()
		{
			return TestEventHandler3.Counter;
		}
	}
}