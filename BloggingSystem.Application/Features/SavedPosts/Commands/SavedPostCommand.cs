using System;
using System.Threading;
using System.Threading.Tasks;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Events;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Features.SavedPosts.Commands
{
    #region Save Post Command

    public class SavePostCommand : IRequest<SavedPostDto>
    {
        public long PostId { get; set; }
    }

    public class SavePostCommandHandler : IRequestHandler<SavePostCommand, SavedPostDto>
    {
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly IRepository<SavedPost> _savedPostRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<SavePostCommandHandler> _logger;

        public SavePostCommandHandler(
            IRepository<Domain.Entities.Post> postRepository,
            IRepository<SavedPost> savedPostRepository,
            ICurrentUserService currentUserService,
            IDomainEventService domainEventService,
            ILogger<SavePostCommandHandler> logger)
        {
            _postRepository = postRepository;
            _savedPostRepository = savedPostRepository;
            _currentUserService = currentUserService;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<SavedPostDto> Handle(SavePostCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Check if post exists
            var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
            if (post == null)
                throw new NotFoundException("Post", request.PostId);

            // Check if post is already saved
            var existingSavedPostSpec = new SavedPostSpecification(currentUserId ?? 0, request.PostId);
            var existingSavedPost = await _savedPostRepository.FirstOrDefaultAsync(existingSavedPostSpec, cancellationToken);

            if (existingSavedPost != null)
                throw new DomainException("Post is already saved");

            // Create new saved post
            var savedPost = SavedPost.Create(currentUserId ?? 0, request.PostId);

            await _savedPostRepository.AddAsync(savedPost, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(savedPost.DomainEvents);

            return new SavedPostDto
            {
                Id = savedPost.Id,
                UserId = savedPost.UserId,
                PostId = savedPost.PostId,
                CreatedAt = savedPost.CreatedAt
            };
        }
    }

    #endregion
}