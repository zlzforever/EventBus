using System;

namespace EventBus
{
	public abstract class Event : IEvent
	{
		/// <summary>
		/// 事件源标识
		/// </summary>
		public Guid EventId { get; set; }

		/// <summary>
		/// 事件发生时间
		/// </summary>
		public DateTimeOffset EventTime { get; set; }

		/// <summary>
		/// 触发事件的对象
		/// </summary>
		public object EventSource { get; set; }

		protected Event()
		{
			EventId = Guid.NewGuid();
			EventTime = DateTimeOffset.UtcNow;
		}

		/// <summary>
		/// 判断事件是否过期
		/// </summary>
		/// <param name="seconds"></param>
		/// <returns></returns>
		public bool IsExpired(int seconds = 30)
		{
			return (DateTimeOffset.Now - EventTime).TotalSeconds > seconds;
		}
	}
}