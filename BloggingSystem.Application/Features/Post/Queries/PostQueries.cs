using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.Post;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Queries
{
    #region Get Posts

    public class GetPostsQuery : IRequest<PaginatedResponseDto<PostSummaryDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetPostsQueryHandler : IRequestHandler<GetPostsQuery, PaginatedResponseDto<PostSummaryDto>>
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPostsQueryHandler> _logger;

        public GetPostsQueryHandler(
            IRepository<Post> postRepository,
            IMapper mapper,
            ILogger<GetPostsQueryHandler> logger)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<PostSummaryDto>> Handle(GetPostsQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new PostsSpecification(request.PageNumber, request.PageSize);
            var totalCount = await _postRepository.CountAsync(new PostsSpecification(), cancellationToken);
            var posts = await _postRepository.ListAsync(spec, cancellationToken);

            return new PaginatedResponseDto<PostSummaryDto>
            {
                Data = _mapper.Map<List<PostSummaryDto>>(posts),
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion

    #region Get Published Posts

    public class GetPublishedPostsQuery : IRequest<PaginatedResponseDto<PostSummaryDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class
        GetPublishedPostsQueryHandler : IRequestHandler<GetPublishedPostsQuery, PaginatedResponseDto<PostSummaryDto>>
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPublishedPostsQueryHandler> _logger;

        public GetPublishedPostsQueryHandler(
            IRepository<Post> postRepository,
            IMapper mapper,
            ILogger<GetPublishedPostsQueryHandler> logger)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<PostSummaryDto>> Handle(GetPublishedPostsQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new PublishedPostsSpecification(request.PageNumber, request.PageSize);
            var totalCount = await _postRepository.CountAsync(new PublishedPostsSpecification(), cancellationToken);
            var posts = await _postRepository.ListAsync(spec, cancellationToken);

            return new PaginatedResponseDto<PostSummaryDto>
            {
                Data = _mapper.Map<List<PostSummaryDto>>(posts),
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion

    #region Get Post By Id

    public class GetPostByIdQuery : IRequest<PostDto>
    {
        public long Id { get; set; }
    }

    public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, PostDto>
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPostByIdQueryHandler> _logger;

        public GetPostByIdQueryHandler(
            IRepository<Post> postRepository,
            IMapper mapper,
            ILogger<GetPostByIdQueryHandler> logger)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PostDto> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new PostSpecification(request.Id);
            var post = await _postRepository.FirstOrDefaultAsync(spec, cancellationToken);

            if (post == null)
                throw new NotFoundException(nameof(Post), request.Id);

            return _mapper.Map<PostDto>(post);
        }
    }

    #endregion

    #region Get Post By Slug

    public class GetPostBySlugQuery : IRequest<PostDto>
    {
        public string Slug { get; set; }
    }

    public class GetPostBySlugQueryHandler : IRequestHandler<GetPostBySlugQuery, PostDto>
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPostBySlugQueryHandler> _logger;

        public GetPostBySlugQueryHandler(
            IRepository<Post> postRepository,
            IMapper mapper,
            ILogger<GetPostBySlugQueryHandler> logger)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PostDto> Handle(GetPostBySlugQuery request, CancellationToken cancellationToken)
        {
            var spec = new PostBySlugSpecification(request.Slug);
            var post = await _postRepository.FirstOrDefaultAsync(spec, cancellationToken);

            if (post == null)
                throw new NotFoundException(nameof(Post), request.Slug);

            return _mapper.Map<PostDto>(post);
        }
    }

    #endregion

    #region Get Posts By Author

    public class GetPostsByAuthorQuery : IRequest<PaginatedResponseDto<PostSummaryDto>>
    {
        public long AuthorId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class
        GetPostsByAuthorQueryHandler : IRequestHandler<GetPostsByAuthorQuery, PaginatedResponseDto<PostSummaryDto>>
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPostsByAuthorQueryHandler> _logger;

        public GetPostsByAuthorQueryHandler(
            IRepository<Post> postRepository,
            IRepository<User> userRepository,
            IMapper mapper,
            ILogger<GetPostsByAuthorQueryHandler> logger)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<PostSummaryDto>> Handle(GetPostsByAuthorQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.AuthorId, cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(User), request.AuthorId);

            var spec = new PostsByAuthorSpecification(request.AuthorId, request.PageNumber, request.PageSize);
            var totalCount =
                await _postRepository.CountAsync(new PostsByAuthorSpecification(request.AuthorId), cancellationToken);
            var posts = await _postRepository.ListAsync(spec, cancellationToken);

            return new PaginatedResponseDto<PostSummaryDto>
            {
                Data = _mapper.Map<List<PostSummaryDto>>(posts),
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion

    #region Get Posts By Category

    public class GetPostsByCategoryQuery : IRequest<PaginatedResponseDto<PostSummaryDto>>
    {
        public long CategoryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class
        GetPostsByCategoryQueryHandler : IRequestHandler<GetPostsByCategoryQuery, PaginatedResponseDto<PostSummaryDto>>
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPostsByCategoryQueryHandler> _logger;

        public GetPostsByCategoryQueryHandler(
            IRepository<Post> postRepository,
            IRepository<Category> categoryRepository,
            IMapper mapper,
            ILogger<GetPostsByCategoryQueryHandler> logger)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<PostSummaryDto>> Handle(GetPostsByCategoryQuery request,
            CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null)
                throw new NotFoundException(nameof(Category), request.CategoryId);

            var spec = new PostsByCategorySpecification(request.CategoryId, request.PageNumber, request.PageSize);
            var totalCount = await _postRepository.CountAsync(new PostsByCategorySpecification(request.CategoryId),
                cancellationToken);
            var posts = await _postRepository.ListAsync(spec, cancellationToken);

            return new PaginatedResponseDto<PostSummaryDto>
            {
                Data = _mapper.Map<List<PostSummaryDto>>(posts),
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion

    #region Get Posts By Tag

    public class GetPostsByTagQuery : IRequest<PaginatedResponseDto<PostSummaryDto>>
    {
        public long TagId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetPostsByTagQueryHandler : IRequestHandler<GetPostsByTagQuery, PaginatedResponseDto<PostSummaryDto>>
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPostsByTagQueryHandler> _logger;

        public GetPostsByTagQueryHandler(
            IRepository<Post> postRepository,
            IRepository<Tag> tagRepository,
            IMapper mapper,
            ILogger<GetPostsByTagQueryHandler> logger)
        {
            _postRepository = postRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<PostSummaryDto>> Handle(GetPostsByTagQuery request,
            CancellationToken cancellationToken)
        {
            var tag = await _tagRepository.GetByIdAsync(request.TagId, cancellationToken);
            if (tag == null)
                throw new NotFoundException(nameof(Tag), request.TagId);

            var spec = new PostsByTagSpecification(request.TagId, request.PageNumber, request.PageSize);
            var totalCount =
                await _postRepository.CountAsync(new PostsByTagSpecification(request.TagId), cancellationToken);
            var posts = await _postRepository.ListAsync(spec, cancellationToken);

            return new PaginatedResponseDto<PostSummaryDto>
            {
                Data = _mapper.Map<List<PostSummaryDto>>(posts),
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion

    #region Search Posts

    public class SearchPostsQuery : IRequest<PaginatedResponseDto<PostSummaryDto>>
    {
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SearchPostsQueryHandler : IRequestHandler<SearchPostsQuery, PaginatedResponseDto<PostSummaryDto>>
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SearchPostsQueryHandler> _logger;

        public SearchPostsQueryHandler(
            IRepository<Post> postRepository,
            IMapper mapper,
            ILogger<SearchPostsQueryHandler> logger)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<PostSummaryDto>> Handle(SearchPostsQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new PostSearchSpecification(request.SearchTerm, request.PageNumber, request.PageSize);
            var totalCount =
                await _postRepository.CountAsync(new PostSearchSpecification(request.SearchTerm), cancellationToken);
            var posts = await _postRepository.ListAsync(spec, cancellationToken);

            return new PaginatedResponseDto<PostSummaryDto>
            {
                Data = _mapper.Map<List<PostSummaryDto>>(posts),
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion
}