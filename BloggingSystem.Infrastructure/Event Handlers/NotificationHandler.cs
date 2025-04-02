using System.Threading;
using System.Threading.Tasks;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Events;
using BloggingSystem.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Infrastructure.EventHandlers
{
    /// <summary>
    /// Base notification handler
    /// </summary>
    /// <typeparam name="TDomainEvent">Type of domain event</typeparam>
    public abstract class NotificationHandler<TDomainEvent> : INotificationHandler<DomainEventNotification<TDomainEvent>>
        where TDomainEvent : DomainEvent
    {
        protected readonly ILogger<NotificationHandler<TDomainEvent>> _logger;
        protected readonly INotificationService _notificationService;
        
        protected NotificationHandler(
            ILogger<NotificationHandler<TDomainEvent>> logger,
            INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }
        
        public abstract Task Handle(DomainEventNotification<TDomainEvent> notification, CancellationToken cancellationToken);
    }
}