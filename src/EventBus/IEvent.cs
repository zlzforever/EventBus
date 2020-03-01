using System;

namespace EventBus
{
    public interface IEvent
    {
        /// <summary>
        /// 事件源标识
        /// </summary>
        Guid EventId { get; set; }

        /// <summary>
        /// 事件发生时间
        /// </summary>
        DateTimeOffset EventTime { get; set; }

        /// <summary>
        /// 触发事件的对象
        /// </summary>
        object EventSource { get; set; }

        /// <summary>
        /// 判断事件是否过期
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        bool IsExpired(int seconds = 30);
    }
}