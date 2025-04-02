using System;
using System.Collections.Generic;

namespace BloggingSystem.Domain.Commons
{
    /// <summary>
    /// Base class for all domain entities providing common functionality
    /// </summary>
    public abstract class BaseEntity
    {
        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();

        /// <summary>
        /// Unique identifier for the entity
        /// </summary>
        public long Id { get; protected set; }

        /// <summary>
        /// Creation timestamp
        /// </summary>
        public DateTime CreatedAt { get; protected set; }

        /// <summary>
        /// Last modification timestamp
        /// </summary>
        public DateTime? UpdatedAt { get; protected set; }

        /// <summary>
        /// Domain events raised by this entity
        /// </summary>
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Add a domain event to be dispatched when the entity is saved
        /// </summary>
        public void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Clear all domain events
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
        
        /// <summary>
        /// Update the modification timestamp
        /// </summary>
        protected void SetModified()
        {
            UpdatedAt = DateTime.UtcNow;
        }
        
        protected void SetDeleted()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}