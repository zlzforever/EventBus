using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EventBus.Tests
{
    public class TestEventHandler2 : IEventHandler<TestEvent>
    {
        public static string Name;

        public Task HandleAsync(TestEvent @event)
        {
            Name = @event.Name;
            return Task.CompletedTask;
        }
    }
}