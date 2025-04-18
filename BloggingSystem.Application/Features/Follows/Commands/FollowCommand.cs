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

namespace BloggingSystem.Application.Features.Follows.Commands
{
    #region Follow User Command

    public class FollowUserCommand : IRequest<FollowDto>
    {
        public long FollowingId { get; set; }
    }

    public class FollowUserCommandHandler : IRequestHandler<FollowUserCommand, FollowDto>
    {
        private readonly IRepository<Domain.Entities.User> _userRepository;
        private readonly IRepository<Follower> _followerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<FollowUserCommandHandler> _logger;

        public FollowUserCommandHandler(
            IRepository<Domain.Entities.User> userRepository,
            IRepository<Follower> followerRepository,
            ICurrentUserService currentUserService,
            IDomainEventService domainEventService,
            ILogger<FollowUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _followerRepository = followerRepository;
            _currentUserService = currentUserService;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<FollowDto> Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            if (currentUserId.Value == request.FollowingId)
                throw new DomainException("Users cannot follow themselves");

            // Check if user to follow exists
            var followingUser = await _userRepository.GetByIdAsync(request.FollowingId, cancellationToken);
            if (followingUser == null)
                throw new NotFoundException("User", request.FollowingId);

            // Check if the follow relationship already exists
            var existingFollowSpec = new FollowerSpecification(
                currentUserId.Value,
                request.FollowingId);
            var existingFollow = await _followerRepository.FirstOrDefaultAsync(
                existingFollowSpec,
                cancellationToken);

            if (existingFollow != null)
                throw new DomainException("You are already following this user");

            // Create new follow relationship
            var follower = Follower.Create(
                currentUserId.Value,
                request.FollowingId);

            await _followerRepository.AddAsync(follower, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(follower.DomainEvents);

            return new FollowDto
            {
                Id = follower.Id,
                FollowerId = follower.FollowerId,
                FollowingId = follower.FollowingId,
                CreatedAt = follower.CreatedAt
            };
        }
    }

    #endregion
}