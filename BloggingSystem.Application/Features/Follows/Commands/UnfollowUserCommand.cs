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

namespace BloggingSystem.Application.Features.Follows.Commands
{
    #region Unfollow User Command

    public class UnfollowUserCommand : IRequest<bool>
    {
        public long FollowingId { get; set; }
    }

    public class UnfollowUserCommandHandler : IRequestHandler<UnfollowUserCommand, bool>
    {
        private readonly IRepository<Follower> _followerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<UnfollowUserCommandHandler> _logger;

        public UnfollowUserCommandHandler(
            IRepository<Follower> followerRepository,
            ICurrentUserService currentUserService,
            IDomainEventService domainEventService,
            ILogger<UnfollowUserCommandHandler> logger)
        {
            _followerRepository = followerRepository;
            _currentUserService = currentUserService;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<bool> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Find the follow relationship
            var followSpec = new FollowerSpecification(
                currentUserId.Value,
                request.FollowingId);
            var follow = await _followerRepository.FirstOrDefaultAsync(followSpec, cancellationToken);

            if (follow == null)
                throw new NotFoundException("Follow relationship not found");

            follow.UnFollow(currentUserId.Value, request.FollowingId);

            // Save domain events to publish after deletion
            var domainEvents = follow.DomainEvents;

            // Delete the follow relationship
            await _followerRepository.DeleteAsync(follow, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(domainEvents);

            return true;
        }
    }

    #endregion
}