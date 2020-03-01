using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            eventBus.Register<TestEvent, TestEventHandler>();
            var handlerTypes = eventBus.GetHandlerTypes<TestEvent>().ToArray();
            Assert.Single(handlerTypes);
            Assert.Equal(typeof(TestEventHandler), handlerTypes.First());
        }

        [Fact]
        public void RegisterMulti()
        {
            var eventBus = GetEventBus();
            eventBus.Register<TestEvent, TestEventHandler>();
            eventBus.Register<TestEvent, TestEventHandler2>();
            var handlerTypes = eventBus.GetHandlerTypes<TestEvent>().ToArray();
            Assert.Equal(2, handlerTypes.Count());
            Assert.Contains(typeof(TestEventHandler2), handlerTypes);
            Assert.Contains(typeof(TestEventHandler), handlerTypes);
        }

        [Fact]
        public void RegisterDuplicate()
        {
            var eventBus = GetEventBus();
            eventBus.Register<TestEvent, TestEventHandler>();
            eventBus.Register<TestEvent, TestEventHandler>();

            var handlerTypes = eventBus.GetHandlerTypes<TestEvent>().ToArray();
            Assert.Single(handlerTypes);
            Assert.Equal(typeof(TestEventHandler), handlerTypes.First());
        }

        [Fact]
        public void ConcurrentRegister()
        {
            var eventBus = GetEventBus();
            Parallel.For(0, 1000, i => { eventBus.Register<TestEvent, TestEventHandler>(); });
            var handlerTypes = eventBus.GetHandlerTypes<TestEvent>().ToArray();
            Assert.Single(handlerTypes);
            Assert.Equal(typeof(TestEventHandler), handlerTypes.First());
        }

        [Fact]
        public async Task Publish()
        {
            var eventBus = GetEventBus();
            eventBus.Register<TestEvent, TestEventHandler>();
            await eventBus.PublishAsync(new TestEvent("test-publish"));
            Assert.Equal("test-publish", TestEventHandler.Name);
        }

        [Fact]
        public virtual async Task PublishToDynamicEventHandler()
        {
            var eventBus = GetEventBus();
            eventBus.Register<TestEvent>(typeof(TestEventDynamicEventHandler));
            await eventBus.PublishAsync(new TestEvent("test-publish"));
            Assert.Equal("test-publish", TestEventDynamicEventHandler.Name);
        }

        [Fact]
        public void ConcurrentPublish()
        {
            var eventBus = GetEventBus();
            TestEventHandler.Counter = 0;
            eventBus.Register<TestEvent, TestEventHandler>();
            var list = new List<Task>();
            for (int i = 0; i < 1000; ++i)
            {
                list.Add(Task.Factory.StartNew(async () =>
                {
                    await eventBus.PublishAsync(new TestEvent("test-publish"));
                }));
            }

            Task.WaitAll(list.ToArray());

            Assert.Equal(1000, TestEventHandler.Counter);
        }

        [Fact]
        public void UnregisterAll()
        {
            var eventBus = GetEventBus();
            eventBus.Register<TestEvent, TestEventHandler>();
            eventBus.Register<TestEvent, TestEventHandler2>();
            eventBus.Unregister<TestEvent>();
            var handlerTypes = eventBus.GetHandlerTypes<TestEvent>().ToArray();
            Assert.Empty(handlerTypes);
        }

        [Fact]
        public void UnregisterSingle()
        {
            var eventBus = GetEventBus();
            eventBus.Register<TestEvent, TestEventHandler>();
            eventBus.Register<TestEvent, TestEventHandler2>();
            eventBus.Unregister<TestEvent, TestEventHandler>();
            var handlerTypes = eventBus.GetHandlerTypes<TestEvent>().ToArray();
            Assert.Single(handlerTypes);
            Assert.Equal(typeof(TestEventHandler2), handlerTypes.First());
        }

        [Fact]
        public void RegisterDynamicEventHandler()
        {
            var eventBus = GetEventBus();
            eventBus.Register<TestEvent>(typeof(TestEventDynamicEventHandler));
            eventBus.Register<TestEvent, TestEventHandler>();
            var handlerTypes = eventBus.GetHandlerTypes<TestEvent>().ToArray();
            Assert.Contains(typeof(TestEventDynamicEventHandler), handlerTypes);
            Assert.Contains(typeof(TestEventHandler), handlerTypes);
        }
    }
}