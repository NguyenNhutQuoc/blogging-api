using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Events;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Features.Likes.Commands
{
    #region Like Entity Command

    public class LikeEntityCommand : IRequest<LikeDto>
    {
        public string? EntityType { get; set; }
        public long EntityId { get; set; }
    }

    public class LikeEntityCommandHandler : IRequestHandler<LikeEntityCommand, LikeDto>
    {
        private readonly IRepository<Domain.Entities.User> _userRepository;
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly IRepository<Domain.Entities.Comment> _commentRepository;
        private readonly IRepository<Like> _likeRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<LikeEntityCommandHandler> _logger;

        public LikeEntityCommandHandler(
            IRepository<Domain.Entities.User> userRepository,
            IRepository<Domain.Entities.Post> postRepository,
            IRepository<Domain.Entities.Comment> commentRepository,
            IRepository<Like> likeRepository,
            ICurrentUserService currentUserService,
            IDomainEventService domainEventService,
            ILogger<LikeEntityCommandHandler> logger)
        {
            _userRepository = userRepository;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _likeRepository = likeRepository;
            _currentUserService = currentUserService;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<LikeDto> Handle(LikeEntityCommand request, CancellationToken cancellationToken)
        {
            // Validate entity type
            if (request.EntityType != "post" && request.EntityType != "comment")
                throw new DomainException("Invalid entity type. Supported types: post, comment");

            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Verify that entity exists
            if (request.EntityType == "post")
            {
                var post = await _postRepository.GetByIdAsync(request.EntityId, cancellationToken);
                if (post == null)
                    throw new NotFoundException("Post", request.EntityId);
            }
            else if (request.EntityType == "comment")
            {
                var comment = await _commentRepository.GetByIdAsync(request.EntityId, cancellationToken);
                if (comment == null)
                    throw new NotFoundException("Comment", request.EntityId);
            }

            // Check if the like already exists
            var existingLikeSpecification = new LikeSpecification(
                currentUserId.Value,
                request.EntityId, request.EntityType);
            
            var existingLike = await _likeRepository.FirstOrDefaultAsync(
                existingLikeSpecification,
                cancellationToken);

            if (existingLike != null)
                throw new DomainException("You have already liked this entity");

            // Create new like
            var like = Like.Create(
                currentUserId.Value,
                request.EntityType,
                request.EntityId
            );

            await _likeRepository.AddAsync(like, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(like.DomainEvents);

            return new LikeDto
            {
                Id = like.Id,
                UserId = like.UserId,
                EntityType = like.EntityType,
                EntityId = like.EntityId,
                CreatedAt = like.CreatedAt
            };
        }
    }

    #endregion
}