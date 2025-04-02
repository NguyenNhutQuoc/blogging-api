using System.Threading;
using System.Threading.Tasks;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Events;
using BloggingSystem.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Infrastructure.EventHandlers
{
    /// <summary>
    /// Handler for CommentUpdatedEvent
    /// </summary>
    public class CommentUpdatedEventHandler : INotificationHandler<DomainEventNotification<CommentUpdatedEvent>>
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<CommentUpdatedEventHandler> _logger;

        public CommentUpdatedEventHandler(
            IRepository<Post> postRepository,
            IRepository<User> userRepository,
            IAuditLogService auditLogService,
            ILogger<CommentUpdatedEventHandler> logger)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        public async Task Handle(DomainEventNotification<CommentUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            _logger.LogInformation("BloggingSystem Domain Event: {DomainEvent}", domainEvent.GetType().Name);

            // Log the update in audit logs
            await _auditLogService.AddAuditLogAsync(
                domainEvent.UserId, // User who updated the comment
                "update",
                "comment",
                domainEvent.CommentId,
                null, // We don't have the old content readily available
                null); // We don't have the new content readily available

            // Optionally notify post author that a comment was updated and needs review

            var post = await _postRepository.GetByIdAsync(domainEvent.PostId, cancellationToken);
            if (post != null && post.AuthorId != domainEvent.UserId) // Don't notify if the post author is the one updating the comment
            {
                // This could trigger a notification to the post author that a comment was updated
                // We'd use a notification service here, similar to other event handlers
                // For brevity, I'm leaving out the actual notification code since it would be similar to other handlers
                _logger.LogInformation("Comment updated on post {PostId} by user {UserId}", domainEvent.PostId, domainEvent.UserId);
            }
        }
    }
}