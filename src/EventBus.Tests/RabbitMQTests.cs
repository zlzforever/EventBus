using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventBus.RabbitMQ;
using Xunit;

namespace EventBus.Tests
{
	public class RabbitMQTests : BasicTests
	{
		protected override IEventBus GetEventBus()
		{
			return new RabbitMQEventBus(new RabbitMQOptions
			{
				Exchange = "EventBus",
				HostName = "localhost",
				UserName = "user",
				Password = "password"
			});
		}

		protected override void Sleep()
		{
			Thread.Sleep(3000);
		}

		protected override void RegisterConcurrentHandler(IEventBus eventBus)
		{
			eventBus.Register<TestEvent, TestEventHandler4>();
		}

		protected override int AssertConcurrentCounter()
		{
			return TestEventHandler4.Counter;
		}
	}
}