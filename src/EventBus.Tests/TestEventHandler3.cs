using System.Threading;
using System.Threading.Tasks;

namespace EventBus.Tests
{
	public class TestEventHandler3
		: IEventHandler<TestEvent>
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