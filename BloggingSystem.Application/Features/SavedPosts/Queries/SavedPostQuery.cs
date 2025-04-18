using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Commons.Specifications;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Features.SavedPosts.Queries
{
    #region Get Saved Posts Query

    public class GetSavedPostsQuery : IRequest<PaginatedResponseDto<SavedPostSummaryDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetSavedPostsQueryHandler : IRequestHandler<GetSavedPostsQuery, PaginatedResponseDto<SavedPostSummaryDto>>
    {
        private readonly IRepository<SavedPost> _savedPostRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSavedPostsQueryHandler> _logger;

        public GetSavedPostsQueryHandler(
            IRepository<SavedPost> savedPostRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetSavedPostsQueryHandler> logger)
        {
            _savedPostRepository = savedPostRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<SavedPostSummaryDto>> Handle(GetSavedPostsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Create specification to get saved posts with pagination
            var spec = new GetSavedPostsSpecification(
                currentUserId.Value,
                request.PageNumber,
                request.PageSize);

            // Get count without pagination
            var countSpec = new GetSavedPostsSpecification(currentUserId.Value);
            var totalCount = await _savedPostRepository.CountAsync(countSpec, cancellationToken);

            // Get saved posts with pagination
            var savedPosts = await _savedPostRepository.ListAsync(spec, cancellationToken);

            // Map to PostSummaryDto
            var postSummaries = new List<SavedPostSummaryDto>();
            foreach (var savedPost in savedPosts)
            {
                postSummaries.Add(new SavedPostSummaryDto
                {
                    Id = savedPost.Post.Id,
                    Title = savedPost.Post.Title,
                    Slug = savedPost.Post.Slug,
                    Excerpt = savedPost.Post.Excerpt,
                    FeaturedImageUrl = savedPost.Post.FeaturedImageUrl,
                    CreatedAt = savedPost.Post.CreatedAt,
                    PublishedAt = savedPost.Post.PublishedAt,
                });
            }

            return new PaginatedResponseDto<SavedPostSummaryDto>
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
    
    #region Check Saved Post Query

    public class CheckSavedPostQuery : IRequest<bool>
    {
        public long PostId { get; set; }
    }

    public class CheckSavedPostQueryHandler : IRequestHandler<CheckSavedPostQuery, bool>
    {
        private readonly IRepository<SavedPost> _savedPostRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CheckSavedPostQueryHandler> _logger;

        public CheckSavedPostQueryHandler(
            IRepository<SavedPost> savedPostRepository,
            ICurrentUserService currentUserService,
            ILogger<CheckSavedPostQueryHandler> logger)
        {
            _savedPostRepository = savedPostRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<bool> Handle(CheckSavedPostQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            var existingSavedPostSpec = new SavedPostSpecification(currentUserId.Value, request.PostId);
            var existingSavedPost = await _savedPostRepository.FirstOrDefaultAsync(existingSavedPostSpec, cancellationToken);
            return existingSavedPost != null;
        }
    }

    #endregion
    
    #region Get Saved Posts Count Query

    public class GetSavedPostsCountQuery : IRequest<int>
    {
    }

    public class GetSavedPostsCountQueryHandler : IRequestHandler<GetSavedPostsCountQuery, int>
    {
        private readonly IRepository<SavedPost> _savedPostRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetSavedPostsCountQueryHandler> _logger;

        public GetSavedPostsCountQueryHandler(
            IRepository<SavedPost> savedPostRepository,
            ICurrentUserService currentUserService,
            ILogger<GetSavedPostsCountQueryHandler> logger)
        {
            _savedPostRepository = savedPostRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<int> Handle(GetSavedPostsCountQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Count saved posts for the current user
            var sp = new SavedPostSpecification(currentUserId.Value);

            // Get count without pagination
            return await _savedPostRepository.CountAsync(sp, cancellationToken);
            
        }
    }

    #endregion
    
    #region Search Saved Posts Query

    public class SearchSavedPostsQuery : IRequest<PaginatedResponseDto<SavedPostSummaryDto>>
    {
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SearchSavedPostsQueryHandler : IRequestHandler<SearchSavedPostsQuery, PaginatedResponseDto<SavedPostSummaryDto>>
    {
        private readonly IRepository<SavedPost> _savedPostRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<SearchSavedPostsQueryHandler> _logger;

        public SearchSavedPostsQueryHandler(
            IRepository<SavedPost> savedPostRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<SearchSavedPostsQueryHandler> logger)
        {
            _savedPostRepository = savedPostRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<SavedPostSummaryDto>> Handle(SearchSavedPostsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Create specification to search saved posts
            var spec = new SearchSavedPostsSpecification(
                currentUserId.Value,
                request.SearchTerm,
                request.PageNumber,
                request.PageSize);

            // Get count without pagination
            var countSpec = new SearchSavedPostsSpecification(currentUserId.Value, request.SearchTerm);
            var totalCount = await _savedPostRepository.CountAsync(countSpec, cancellationToken);

            // Get saved posts with pagination
            var savedPosts = await _savedPostRepository.ListAsync(spec, cancellationToken);

            // Map to PostSummaryDto
            var postSummaries = new List<SavedPostSummaryDto>();
            foreach (var savedPost in savedPosts)
            {
                postSummaries.Add(new SavedPostSummaryDto
                {
                    Id = savedPost.Post.Id,
                    Title = savedPost.Post.Title,
                    Slug = savedPost.Post.Slug,
                    Excerpt = savedPost.Post.Excerpt,
                    FeaturedImageUrl = savedPost.Post.FeaturedImageUrl,
                    CreatedAt = savedPost.Post.CreatedAt,
                    PublishedAt = savedPost.Post.PublishedAt,
                    SavedAt = savedPost.CreatedAt
                });
            }

            return new PaginatedResponseDto<SavedPostSummaryDto>
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