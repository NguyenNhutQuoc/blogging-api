using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Commons;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Infrastructure.Services
{
    /// <summary>
    /// Implementation of domain event service
    /// </summary>
    public class DomainEventService : IDomainEventService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DomainEventService> _logger;
        
        public DomainEventService(
            IMediator mediator,
            ILogger<DomainEventService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        /// <summary>
        /// Publish a single domain event
        /// </summary>
        public async Task PublishEventAsync(DomainEvent domainEvent)
        {
            _logger.LogInformation("Publishing domain event. Event: {event}", domainEvent.GetType().Name);
            await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent));
        }
        
        /// <summary>
        /// Publish multiple domain events
        /// </summary>
        public async Task PublishEventsAsync(IEnumerable<DomainEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                await PublishEventAsync(domainEvent);
            }
        }
        
        /// <summary>
        /// Convert domain event to notification
        /// </summary>
        private INotification GetNotificationCorrespondingToDomainEvent(DomainEvent domainEvent)
        {
            return (INotification)Activator.CreateInstance(
                typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()),
                domainEvent);
        }
    }
    
    /// <summary>
    /// Domain event notification for MediatR
    /// </summary>
    /// <typeparam name="TDomainEvent">Type of domain event</typeparam>
    public class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : DomainEvent
    {
        public TDomainEvent DomainEvent { get; }
        
        public DomainEventNotification(TDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}