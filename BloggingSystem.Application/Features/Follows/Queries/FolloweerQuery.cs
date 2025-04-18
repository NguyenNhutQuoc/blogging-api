using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Commons.Specifications;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Features.Follows.Queries
{
    #region Get Following Query

    public class GetFollowingQuery : IRequest<PaginatedResponseDto<UserSummaryDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetFollowingQueryHandler : IRequestHandler<GetFollowingQuery, PaginatedResponseDto<UserSummaryDto>>
    {
        private readonly IRepository<Follower> _followerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetFollowingQueryHandler> _logger;

        public GetFollowingQueryHandler(
            IRepository<Follower> followerRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetFollowingQueryHandler> logger)
        {
            _followerRepository = followerRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<UserSummaryDto>> Handle(GetFollowingQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Create specification to get users that the current user follows
            var spec = new GetFollowingSpecification(
                currentUserId.Value, 
                request.PageNumber, 
                request.PageSize);

            // Get count without pagination
            var countSpec = new GetFollowingSpecification(currentUserId.Value);
            var totalCount = await _followerRepository.CountAsync(countSpec, cancellationToken);

            // Get follows with pagination
            var follows = await _followerRepository.ListAsync(spec, cancellationToken);

            // Map to UserSummaryDto
            var users = new List<UserSummaryDto>();
            foreach (var follow in follows)
            {
                users.Add(new UserSummaryDto
                {
                    Id = follow.Following.Id,
                    Username = follow.Following.Username,
                    DisplayName = follow.Following.UserProfile?.DisplayName ?? follow.Following.Username,
                    AvatarUrl = follow.Following.UserProfile?.AvatarUrl
                });
            }

            return new PaginatedResponseDto<UserSummaryDto>
            {
                Data = users,
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion
    
    #region Check Follow Query

    public class CheckFollowQuery : IRequest<bool>
    {
        public long FollowingId { get; set; }
    }

    public class CheckFollowQueryHandler : IRequestHandler<CheckFollowQuery, bool>
    {
        private readonly IRepository<Follower> _followerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CheckFollowQueryHandler> _logger;

        public CheckFollowQueryHandler(
            IRepository<Follower> followerRepository,
            ICurrentUserService currentUserService,
            ILogger<CheckFollowQueryHandler> logger)
        {
            _followerRepository = followerRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<bool> Handle(CheckFollowQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");
            
            

            // Check if the follow relationship already exists
            var existingFollowSpec = new FollowerSpecification(
                currentUserId.Value,
                request.FollowingId);
            var existingFollow = await _followerRepository.FirstOrDefaultAsync(
                existingFollowSpec,
                cancellationToken);

            return existingFollow != null;
        }
    }

    #endregion
    
     #region Get Followers Query

    public class GetFollowersQuery : IRequest<PaginatedResponseDto<UserSummaryDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetFollowersQueryHandler : IRequestHandler<GetFollowersQuery, PaginatedResponseDto<UserSummaryDto>>
    {
        private readonly IRepository<Follower> _followerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetFollowersQueryHandler> _logger;

        public GetFollowersQueryHandler(
            IRepository<Follower> followerRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetFollowersQueryHandler> logger)
        {
            _followerRepository = followerRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<UserSummaryDto>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Create specification to get users that follow the current user
            var spec = new GetFollowersSpecification(
                currentUserId.Value, 
                request.PageNumber, 
                request.PageSize);

            // Get count without pagination
            var countSpec = new GetFollowersSpecification(currentUserId.Value);
            var totalCount = await _followerRepository.CountAsync(countSpec, cancellationToken);

            // Get follows with pagination
            var follows = await _followerRepository.ListAsync(spec, cancellationToken);

            // Map to UserSummaryDto
            var users = new List<UserSummaryDto>();
            foreach (var follow in follows)
            {
                users.Add(new UserSummaryDto
                {
                    Id = follow.FollowerNavigation.Id,
                    Username = follow.FollowerNavigation.Username,
                    DisplayName = follow.FollowerNavigation.UserProfile?.DisplayName ?? follow.FollowerNavigation.Username,
                    AvatarUrl = follow.FollowerNavigation.UserProfile?.AvatarUrl
                });
            }

            return new PaginatedResponseDto<UserSummaryDto>
            {
                Data = users,
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion
    
    #region Get User Followers Query

    public class GetUserFollowersQuery : IRequest<PaginatedResponseDto<UserSummaryDto>>
    {
        public long UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetUserFollowersQueryHandler : IRequestHandler<GetUserFollowersQuery, PaginatedResponseDto<UserSummaryDto>>
    {
        private readonly IRepository<Domain.Entities.User> _userRepository;
        private readonly IRepository<Follower> _followerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserFollowersQueryHandler> _logger;

        public GetUserFollowersQueryHandler(
            IRepository<Domain.Entities.User> userRepository,
            IRepository<Follower> followerRepository,
            IMapper mapper,
            ILogger<GetUserFollowersQueryHandler> logger)
        {
            _userRepository = userRepository;
            _followerRepository = followerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<UserSummaryDto>> Handle(GetUserFollowersQuery request, CancellationToken cancellationToken)
        {
            // Verify that user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                throw new NotFoundException("User", request.UserId);

            // Create specification to get users that follow the specified user
            var spec = new GetFollowersSpecification(
                request.UserId, 
                request.PageNumber, 
                request.PageSize);

            // Get count without pagination
            var countSpec = new GetFollowersSpecification(request.UserId);
            var totalCount = await _followerRepository.CountAsync(countSpec, cancellationToken);

            // Get follows with pagination
            var follows = await _followerRepository.ListAsync(spec, cancellationToken);

            // Map to UserSummaryDto
            var users = new List<UserSummaryDto>();
            foreach (var follow in follows)
            {
                users.Add(new UserSummaryDto
                {
                    Id = follow.FollowerNavigation.Id,
                    Username = follow.FollowerNavigation.Username,
                    DisplayName = follow.FollowerNavigation.UserProfile?.DisplayName ?? follow.FollowerNavigation.Username,
                    AvatarUrl = follow.FollowerNavigation.UserProfile?.AvatarUrl
                });
            }

            return new PaginatedResponseDto<UserSummaryDto>
            {
                Data = users,
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion
    
    #region Get User Following Query

    public class GetUserFollowingQuery : IRequest<PaginatedResponseDto<UserSummaryDto>>
    {
        public long UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetUserFollowingQueryHandler : IRequestHandler<GetUserFollowingQuery, PaginatedResponseDto<UserSummaryDto>>
    {
        private readonly IRepository<Domain.Entities.User> _userRepository;
        private readonly IRepository<Follower> _followerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserFollowingQueryHandler> _logger;

        public GetUserFollowingQueryHandler(
            IRepository<Domain.Entities.User> userRepository,
            IRepository<Follower> followerRepository,
            IMapper mapper,
            ILogger<GetUserFollowingQueryHandler> logger)
        {
            _userRepository = userRepository;
            _followerRepository = followerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<UserSummaryDto>> Handle(GetUserFollowingQuery request, CancellationToken cancellationToken)
        {
            // Verify that user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                throw new NotFoundException("User", request.UserId);

            // Create specification to get users that the specified user follows
            var spec = new GetFollowingSpecification(
                request.UserId, 
                request.PageNumber, 
                request.PageSize);

            // Get count without pagination
            var countSpec = new GetFollowingSpecification(request.UserId);
            var totalCount = await _followerRepository.CountAsync(countSpec, cancellationToken);

            // Get follows with pagination
            var follows = await _followerRepository.ListAsync(spec, cancellationToken);

            // Map to UserSummaryDto
            var users = new List<UserSummaryDto>();
            foreach (var follow in follows)
            {
                users.Add(new UserSummaryDto
                {
                    Id = follow.Following.Id,
                    Username = follow.Following.Username,
                    DisplayName = follow.Following.UserProfile?.DisplayName ?? follow.Following.Username,
                    AvatarUrl = follow.Following.UserProfile?.AvatarUrl
                });
            }

            return new PaginatedResponseDto<UserSummaryDto>
            {
                Data = users,
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion

}