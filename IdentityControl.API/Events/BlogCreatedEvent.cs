using MercuryBus.Events.Common;

namespace IdentityControl.API.Events
{
    public class BlogCreatedEvent : IDomainEvent
    {
        public string BlogId { get; set; }
        public string UserId { get; set; }
    }
}