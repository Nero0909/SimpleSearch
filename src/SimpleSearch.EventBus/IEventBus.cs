using SimpleSearch.Messages;

namespace SimpleSearch.EventBus
{
    public interface IEventBus
    {
        void Publish(BaseMessage message);
    }
}