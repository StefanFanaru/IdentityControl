using MercuryBus.Events.Subscriber;
using MercuryBus.Helpers;
using Microsoft.Extensions.Logging;

namespace IdentityControl.API.Events
{
    public class BlogCreatedEventHandler : IDomainEventHandler<BlogCreatedEvent>
    {
        private readonly ILogger _logger;

        public BlogCreatedEventHandler(ILogger<BlogCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public void Handle(IDomainEventEnvelope<BlogCreatedEvent> @event)
        {
            var payload = @event.Message.Payload.FromJson<BlogCreatedEvent>();
            _logger.LogInformation($"Got message BlogCreatedEvent for blog with ID: {payload.BlogId}");
        }
    }
}