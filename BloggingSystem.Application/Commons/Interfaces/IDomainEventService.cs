using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Application.Commons.Interfaces
{
    /// <summary>
    /// Service interface for publishing domain events
    /// </summary>
    public interface IDomainEventService
    {
        /// <summary>
        /// Publish a single domain event
        /// </summary>
        Task PublishEventAsync(DomainEvent domainEvent);
        
        /// <summary>
        /// Publish multiple domain events
        /// </summary>
        Task PublishEventsAsync(IEnumerable<DomainEvent> domainEvents);
    }
}