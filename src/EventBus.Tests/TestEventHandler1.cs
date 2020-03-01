using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EventBus.Tests
{
	public class TestEventHandler1 : IEventHandler<TestEvent>
	{
		public static string Name;
		public static int Counter = 0;

		public Task HandleAsync(TestEvent @event)
		{
			Name = @event.Name;
			Interlocked.Increment(ref Counter);
			return Task.CompletedTask;
		}
	}
}