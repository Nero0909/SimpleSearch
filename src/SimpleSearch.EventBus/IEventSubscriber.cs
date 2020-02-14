using SimpleSearch.Messages;

namespace SimpleSearch.EventBus
{
    public interface IEventSubscriber
    {
        void Subscribe<T>(string subscriptionName) where T : BaseMessage;
    }
}