using System.Threading.Tasks;

namespace EventBus
{
    public interface IEventHandler<in TEvent>
        where TEvent : class, IEvent
    {
        Task HandleAsync(TEvent @event);
    }
}