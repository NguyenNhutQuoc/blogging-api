using System;
using System.Threading;
using System.Threading.Tasks;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Events;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Features.SavedPosts.Commands
{
    #region Unsave Post Command

    public class UnsavePostCommand : IRequest<bool>
    {
        public long PostId { get; set; }
    }

    public class UnsavePostCommandHandler : IRequestHandler<UnsavePostCommand, bool>
    {
        private readonly IRepository<SavedPost> _savedPostRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<UnsavePostCommandHandler> _logger;

        public UnsavePostCommandHandler(
            IRepository<SavedPost> savedPostRepository,
            ICurrentUserService currentUserService,
            IDomainEventService domainEventService,
            ILogger<UnsavePostCommandHandler> logger)
        {
            _savedPostRepository = savedPostRepository;
            _currentUserService = currentUserService;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<bool> Handle(UnsavePostCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Find the saved post
            var savedPostSpec = new SavedPostSpecification(
                currentUserId.Value,
                request.PostId);
            var savedPost = await _savedPostRepository.FirstOrDefaultAsync(savedPostSpec, cancellationToken);

            if (savedPost == null)
                throw new NotFoundException("Saved post not found");

            savedPost.Delete(currentUserId.Value, request.PostId);
            
            // Save domain events to publish after deletion
            var domainEvents = savedPost.DomainEvents;

            // Delete the saved post
            await _savedPostRepository.DeleteAsync(savedPost, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(domainEvents);

            return true;
        }
    }

    #endregion
}