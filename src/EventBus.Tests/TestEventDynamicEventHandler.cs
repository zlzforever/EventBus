using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EventBus.Tests
{
    public class TestEventDynamicEventHandler : IDynamicEventHandler
    {
        public static string Name;

        public Task HandleAsync(object @event)
        {
            var obj = (TestEvent) @event;
            Name = obj.Name;
            Console.WriteLine(JsonConvert.SerializeObject(@event));
            return Task.CompletedTask;
        }
    }
}