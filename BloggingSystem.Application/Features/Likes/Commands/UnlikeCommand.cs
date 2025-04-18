using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Events;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Features.Likes.Commands
{
    #region Unlike Entity Command

    public class UnlikeEntityCommand : IRequest<bool>
    {
        public string? EntityType { get; set; }
        public long EntityId { get; set; }
    }

    public class UnlikeEntityCommandHandler : IRequestHandler<UnlikeEntityCommand, bool>
    {
        private readonly IRepository<Like> _likeRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<UnlikeEntityCommandHandler> _logger;

        public UnlikeEntityCommandHandler(
            IRepository<Like> likeRepository,
            ICurrentUserService currentUserService,
            IDomainEventService domainEventService,
            ILogger<UnlikeEntityCommandHandler> logger)
        {
            _likeRepository = likeRepository;
            _currentUserService = currentUserService;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<bool> Handle(UnlikeEntityCommand request, CancellationToken cancellationToken)
        {
            // Validate entity type
            if (request.EntityType != "post" && request.EntityType != "comment")
                throw new DomainException("Invalid entity type. Supported types: post, comment");

            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Find the like
            var likeSpec = new LikeSpecification(
                currentUserId.Value,
                request.EntityId,
                request.EntityType);
            var like = await _likeRepository.FirstOrDefaultAsync(likeSpec, cancellationToken);

            if (like == null)
                throw new NotFoundException("Like not found");

            like.UnLiked(currentUserId.Value, request.EntityType, request.EntityId);

            // Save domain events to publish after deletion
            var domainEvents = like.DomainEvents;

            // Delete the like
            await _likeRepository.DeleteAsync(like, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(domainEvents);

            return true;
        }
    }

    #endregion
}