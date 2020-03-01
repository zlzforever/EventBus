using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.RabbitMQ;
using Xunit;

namespace EventBus.Tests
{
	public class BasicTests
	{
		protected virtual IEventBus GetEventBus()
		{
			return new EventBus();
		}

		[Fact]
		public void Register()
		{
			var eventBus = GetEventBus();
			eventBus.Register<TestEvent, TestEventHandler1>();
			var handlerTypes = eventBus.GetHandlerTypes<TestEvent>().ToArray();
			Assert.Single(handlerTypes);
			Assert.Equal(typeof(TestEventHandler1), handlerTypes.First());
		}

		[Fact]
		public void RegisterMulti()
		{
			var eventBus = GetEventBus();
			eventBus.Register<TestEvent, TestEventHandler1>();
			eventBus.Register<TestEvent, TestEventHandler2>();
			var handlerTypes = eventBus.GetHandlerTypes<TestEvent>().ToArray();
			Assert.Equal(2, handlerTypes.Count());
			Assert.Contains(typeof(TestEventHandler2), handlerTypes);
			Assert.Contains(typeof(TestEventHandler1), handlerTypes);
		}

		[Fact]
		public void RegisterDuplicate()
		{
			var eventBus = GetEventBus();
			eventBus.Register<TestEvent, TestEventHandler1>();
			eventBus.Register<TestEvent, TestEventHandler1>();

			var handlerTypes = eventBus.GetHandlerTypes<TestEvent>().ToArray();
			Assert.Single(handlerTypes);
			Assert.Equal(typeof(TestEventHandler1), handlerTypes.First());
		}

		[Fact]
		public void ConcurrentRegister()
		{
			var eventBus = GetEventBus();
			Parallel.For(0, 1000, i => { eventBus.Register<TestEvent, TestEventHandler1>(); });
			var handlerTypes = eventBus.GetHandlerTypes<TestEvent>().ToArray();
			Assert.Single(handlerTypes);
			Assert.Equal(typeof(TestEventHandler1), handlerTypes.First());
		}

		[Fact]
		public async Task Publish()
		{
			var eventBus = GetEventBus();
			eventBus.Register<TestEvent, TestEventHandler1>();
			await eventBus.PublishAsync(new TestEvent("test-publish"));

			Sleep();
			Assert.Equal("test-publish", TestEventHandler1.Name);
		}

		[Fact]
		public virtual async Task PublishToDynamicEventHandler()
		{
			var eventBus = GetEventBus();
			eventBus.Register<TestEvent>(typeof(TestEventDynamicEventHandler));
			await eventBus.PublishAsync(new TestEvent("test-publish"));

			Sleep();
			Assert.Equal("test-publish", TestEventDynamicEventHandler.Name);
		}

		[Fact]
		public virtual void ConcurrentPublish()
		{
			var eventBus = GetEventBus();
			RegisterConcurrentHandler(eventBus);
			var list = new List<Task>();
			for (int i = 0; i < 100; ++i)
			{
				list.Add(Task.Factory.StartNew(async () => { await eventBus.PublishAsync(new TestEvent("test-publish")); }));
			}

			Task.WaitAll(list.ToArray());

			Sleep();

			if (eventBus is RabbitMQEventBus rabbit)
			{
				Assert.Equal(100, rabbit.PublishCounter);
				Assert.Equal(100, rabbit.ConsumerCounter);
			}

			Assert.Equal(100, AssertConcurrentCounter());
		}

		protected virtual int AssertConcurrentCounter()
		{
			return TestEventHandler1.Counter;
		}

		protected virtual void RegisterConcurrentHandler(IEventBus eventBus)
		{
			eventBus.Register<TestEvent, TestEventHandler1>();
		}

		[Fact]
		public void UnregisterAll()
		{
			var eventBus = GetEventBus();
			eventBus.Register<TestEvent, TestEventHandler1>();
			eventBus.Register<TestEvent, TestEventHandler2>();
			eventBus.Unregister<TestEvent>();
			var handlerTypes = eventBus.GetHandlerTypes<TestEvent>().ToArray();
			Assert.Empty(handlerTypes);
		}

		[Fact]
		public void UnregisterSingle()
		{
			var eventBus = GetEventBus();
			eventBus.Register<TestEvent, TestEventHandler1>();
			eventBus.Register<TestEvent, TestEventHandler2>();
			eventBus.Unregister<TestEvent, TestEventHandler1>();
			var handlerTypes = eventBus.GetHandlerTypes<TestEvent>().ToArray();
			Assert.Single(handlerTypes);
			Assert.Equal(typeof(TestEventHandler2), handlerTypes.First());
		}

		[Fact]
		public void RegisterDynamicEventHandler()
		{
			var eventBus = GetEventBus();
			eventBus.Register<TestEvent>(typeof(TestEventDynamicEventHandler));
			eventBus.Register<TestEvent, TestEventHandler1>();
			var handlerTypes = eventBus.GetHandlerTypes<TestEvent>().ToArray();
			Assert.Contains(typeof(TestEventDynamicEventHandler), handlerTypes);
			Assert.Contains(typeof(TestEventHandler1), handlerTypes);
		}

		protected virtual void Sleep()
		{
		}
	}
}