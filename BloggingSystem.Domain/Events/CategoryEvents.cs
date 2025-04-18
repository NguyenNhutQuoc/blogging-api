using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Events
{
    public class CategoryCreatedEvent : DomainEvent
    {
        public Category Category { get; }

        public CategoryCreatedEvent(Category category)
        {
            Category = category;
        }
    }

    public class CategoryUpdatedEvent : DomainEvent
    {
        public Category Category { get; }

        public CategoryUpdatedEvent(Category category)
        {
            Category = category;
        }
    }

    public class CategoryDeletedEvent : DomainEvent
    {
        public Category Category { get; }

        public CategoryDeletedEvent(Category category)
        {
            Category = category;
        }
    }
} 