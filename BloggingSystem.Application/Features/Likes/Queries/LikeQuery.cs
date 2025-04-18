using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.Post;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Features.Likes.Queries
{
    #region Get Like Count Query

    public class GetLikeCountQuery : IRequest<int>
    {
        public string EntityType { get; set; }
        public long EntityId { get; set; }
    }

    public class GetLikeCountQueryHandler : IRequestHandler<GetLikeCountQuery, int>
    {
        private readonly IRepository<Like> _likeRepository;
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly IRepository<Domain.Entities.Comment> _commentRepository;
        private readonly ILogger<GetLikeCountQueryHandler> _logger;

        public GetLikeCountQueryHandler(
            IRepository<Like> likeRepository,
            IRepository<Domain.Entities.Post> postRepository,
            IRepository<Domain.Entities.Comment> commentRepository,
            ILogger<GetLikeCountQueryHandler> logger)
        {
            _likeRepository = likeRepository;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _logger = logger;
        }

        public async Task<int> Handle(GetLikeCountQuery request, CancellationToken cancellationToken)
        {
            // Validate entity type
            if (request.EntityType != "post" && request.EntityType != "comment")
                throw new DomainException("Invalid entity type. Supported types: post, comment");

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

            // Count likes for the entity
            var countSpec = new GetEntityLikesSpecification(request.EntityType, request.EntityId);
            var count = await _likeRepository.CountAsync(countSpec, cancellationToken);

            return count;
        }
    }

    #endregion
    
    #region Check Like Query

    public class CheckLikeQuery : IRequest<bool>
    {
        public string EntityType { get; set; }
        public long EntityId { get; set; }
    }

    public class CheckLikeQueryHandler : IRequestHandler<CheckLikeQuery, bool>
    {
        private readonly IRepository<Like> _likeRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CheckLikeQueryHandler> _logger;

        public CheckLikeQueryHandler(
            IRepository<Like> likeRepository,
            ICurrentUserService currentUserService,
            ILogger<CheckLikeQueryHandler> logger)
        {
            _likeRepository = likeRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<bool> Handle(CheckLikeQuery request, CancellationToken cancellationToken)
        {
            // Validate entity type
            if (request.EntityType != "post" && request.EntityType != "comment")
                throw new DomainException("Invalid entity type. Supported types: post, comment");

            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Check if like exists
            // Check if the like already exists
            var existingLikeSpecification = new LikeSpecification(
                currentUserId.Value,
                request.EntityId, request.EntityType);
            
            var existingLike = await _likeRepository.FirstOrDefaultAsync(
                existingLikeSpecification,
                cancellationToken);

            return existingLike != null;
        }
    }

    #endregion
    
    #region Get Entity Like Users Query

    public class GetEntityLikeUsersQuery : IRequest<PaginatedResponseDto<UserSummaryDto>>
    {
        public string EntityType { get; set; }
        public long EntityId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetEntityLikeUsersQueryHandler : IRequestHandler<GetEntityLikeUsersQuery, PaginatedResponseDto<UserSummaryDto>>
    {
        private readonly IRepository<Like> _likeRepository;
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly IRepository<Domain.Entities.Comment> _commentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetEntityLikeUsersQueryHandler> _logger;

        public GetEntityLikeUsersQueryHandler(
            IRepository<Like> likeRepository,
            IRepository<Domain.Entities.Post> postRepository,
            IRepository<Domain.Entities.Comment> commentRepository,
            IMapper mapper,
            ILogger<GetEntityLikeUsersQueryHandler> logger)
        {
            _likeRepository = likeRepository;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<UserSummaryDto>> Handle(GetEntityLikeUsersQuery request, CancellationToken cancellationToken)
        {
            // Validate entity type
            if (request.EntityType != "post" && request.EntityType != "comment")
                throw new DomainException("Invalid entity type. Supported types: post, comment");

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

            // Create specification to get likes for the entity
            var spec = new GetEntityLikesSpecification(
                request.EntityType,
                request.EntityId,
                request.PageNumber,
                request.PageSize);

            // Get count without pagination
            var countSpec = new GetEntityLikesSpecification(request.EntityType, request.EntityId);
            var totalCount = await _likeRepository.CountAsync(countSpec, cancellationToken);

            // Get likes with pagination
            var likes = await _likeRepository.ListAsync(spec, cancellationToken);

            // Map to UserSummaryDto
            var users = new List<UserSummaryDto>();
            foreach (var like in likes)
            {
                users.Add(new UserSummaryDto
                {
                    Id = like.User.Id,
                    Username = like.User.Username,
                    DisplayName = like.User.UserProfile?.DisplayName ?? like.User.Username,
                    AvatarUrl = like.User.UserProfile?.AvatarUrl
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
    
    #region Get User Liked Posts Query

    public class GetUserLikedPostsQuery : IRequest<PaginatedResponseDto<PostSummaryDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetUserLikedPostsQueryHandler : IRequestHandler<GetUserLikedPostsQuery, PaginatedResponseDto<PostSummaryDto>>
    {
        private readonly IRepository<Like> _likeRepository;
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserLikedPostsQueryHandler> _logger;

        public GetUserLikedPostsQueryHandler(
            IRepository<Like> likeRepository,
            IRepository<Domain.Entities.Post> postRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetUserLikedPostsQueryHandler> logger)
        {
            _likeRepository = likeRepository;
            _postRepository = postRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<PostSummaryDto>> Handle(GetUserLikedPostsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Create specification for finding likes by the current user
            var spec = new UserLikedPostsSpecification(
                currentUserId.Value,
                request.PageNumber,
                request.PageSize);

            // Get count without pagination
            var countSpec = new UserLikedPostsSpecification(currentUserId.Value);
            var totalCount = await _likeRepository.CountAsync(countSpec, cancellationToken);

            // Get likes with pagination
            var likes = await _likeRepository.ListAsync(spec, cancellationToken);

            // Create a list to store post summaries
            var postSummaries = new List<PostSummaryDto>();

            // For each like, fetch the corresponding post
            foreach (var like in likes)
            {
                var postSpec = new PostByIdWithAuthorSpecification(like.EntityId);
                var post = await _postRepository.FirstOrDefaultAsync(postSpec, cancellationToken);
                
                if (post != null)
                {
                    // Map to PostSummaryDto
                    var postSummary = _mapper.Map<PostSummaryDto>(post);
                    postSummary.LikeAt = like.CreatedAt;
                    postSummaries.Add(postSummary);
                }
            }

            return new PaginatedResponseDto<PostSummaryDto>
            {
                Data = postSummaries,
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion
}