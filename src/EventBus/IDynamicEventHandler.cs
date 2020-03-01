using System.Threading.Tasks;

namespace EventBus
{
    public interface IDynamicEventHandler
    {
        Task HandleAsync(object @event);
    }
}