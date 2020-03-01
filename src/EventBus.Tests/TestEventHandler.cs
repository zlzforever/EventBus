using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EventBus.Tests
{
    public class TestEventHandler : IEventHandler<TestEvent>
    {
        private static readonly object Locker = new object();
        public static string Name;
        public static int Counter = 0;

        public Task HandleAsync(TestEvent @event)
        {
            Name = @event.Name;
            Console.WriteLine(JsonConvert.SerializeObject(@event));
            lock (Locker)
            {
                Counter++;
            }

            return Task.CompletedTask;
        }
    }
}