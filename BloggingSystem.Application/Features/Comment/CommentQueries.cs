using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.Comment;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Queries
{
    #region Get Comments

    public class GetCommentsQuery : IRequest<PaginatedResponseDto<CommentDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, PaginatedResponseDto<CommentDto>>
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCommentsQueryHandler> _logger;

        public GetCommentsQueryHandler(
            IRepository<Comment> commentRepository,
            IMapper mapper,
            ILogger<GetCommentsQueryHandler> logger)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<CommentDto>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            var spec = new CommentsSpecification(request.PageNumber, request.PageSize);
            var totalCount = await _commentRepository.CountAsync(new CommentsSpecification(), cancellationToken);
            var comments = await _commentRepository.ListAsync(spec, cancellationToken);

            return new PaginatedResponseDto<CommentDto>
            {
                Data = _mapper.Map<List<CommentDto>>(comments),
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion

    #region Get Comment By Id

    public class GetCommentByIdQuery : IRequest<CommentDto>
    {
        public long Id { get; set; }
    }

    public class GetCommentByIdQueryHandler : IRequestHandler<GetCommentByIdQuery, CommentDto>
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCommentByIdQueryHandler> _logger;

        public GetCommentByIdQueryHandler(
            IRepository<Comment> commentRepository,
            IMapper mapper,
            ILogger<GetCommentByIdQueryHandler> logger)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CommentDto> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new CommentByIdSpecification(request.Id);
            var comment = await _commentRepository.FirstOrDefaultAsync(spec, cancellationToken);

            if (comment == null)
                throw new NotFoundException(nameof(Comment), request.Id);

            return _mapper.Map<CommentDto>(comment);
        }
    }

    #endregion

    #region Get Comments By Post

    public class GetCommentsByPostQuery : IRequest<PaginatedResponseDto<CommentDto>>
    {
        public long PostId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IncludeReplies { get; set; } = true;
    }

    public class GetCommentsByPostQueryHandler : IRequestHandler<GetCommentsByPostQuery, PaginatedResponseDto<CommentDto>>
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Post> _postRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCommentsByPostQueryHandler> _logger;

        public GetCommentsByPostQueryHandler(
            IRepository<Comment> commentRepository,
            IRepository<Post> postRepository,
            IMapper mapper,
            ILogger<GetCommentsByPostQueryHandler> logger)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<CommentDto>> Handle(GetCommentsByPostQuery request, CancellationToken cancellationToken)
        {
            // Check post exists
            var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
            if (post == null)
                throw new NotFoundException(nameof(Post), request.PostId);

            // If include replies is false, get only top-level comments
            ISpecification<Comment> countSpec;
            ISpecification<Comment> pagedSpec;

            if (!request.IncludeReplies)
            {
                countSpec = new TopLevelCommentsByPostSpecification(request.PostId);
                pagedSpec = new TopLevelCommentsByPostSpecification(request.PostId, request.PageNumber, request.PageSize);
            }
            else
            {
                countSpec = new CommentsByPostSpecification(request.PostId);
                pagedSpec = new CommentsByPostSpecification(request.PostId, request.PageNumber, request.PageSize);
            }

            var totalCount = await _commentRepository.CountAsync(countSpec, cancellationToken);
            var comments = await _commentRepository.ListAsync(pagedSpec, cancellationToken);

            var commentDtos = _mapper.Map<List<CommentDto>>(comments);

            // If we're only getting top-level comments and including replies, get the replies separately
            if (!request.IncludeReplies)
            {
                foreach (var commentDto in commentDtos)
                {
                    var repliesSpec = new CommentRepliesSpecification(commentDto.Id);
                    var replies = await _commentRepository.ListAsync(repliesSpec, cancellationToken);
                    commentDto.Replies = _mapper.Map<List<CommentDto>>(replies);
                }
            }

            return new PaginatedResponseDto<CommentDto>
            {
                Data = commentDtos,
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion

    #region Get Comments By User

    public class GetCommentsByUserQuery : IRequest<PaginatedResponseDto<CommentDto>>
    {
        public long UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetCommentsByUserQueryHandler : IRequestHandler<GetCommentsByUserQuery, PaginatedResponseDto<CommentDto>>
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCommentsByUserQueryHandler> _logger;

        public GetCommentsByUserQueryHandler(
            IRepository<Comment> commentRepository,
            IRepository<User> userRepository,
            IMapper mapper,
            ILogger<GetCommentsByUserQueryHandler> logger)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<CommentDto>> Handle(GetCommentsByUserQuery request, CancellationToken cancellationToken)
        {
            // Check user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(User), request.UserId);

            var spec = new CommentsByUserSpecification(request.UserId, request.PageNumber, request.PageSize);
            var totalCount = await _commentRepository.CountAsync(new CommentsByUserSpecification(request.UserId), cancellationToken);
            var comments = await _commentRepository.ListAsync(spec, cancellationToken);

            return new PaginatedResponseDto<CommentDto>
            {
                Data = _mapper.Map<List<CommentDto>>(comments),
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion

    #region Get Comments By Status

    public class GetCommentsByStatusQuery : IRequest<PaginatedResponseDto<CommentDto>>
    {
        public string Status { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetCommentsByStatusQueryHandler : IRequestHandler<GetCommentsByStatusQuery, PaginatedResponseDto<CommentDto>>
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCommentsByStatusQueryHandler> _logger;

        public GetCommentsByStatusQueryHandler(
            IRepository<Comment> commentRepository,
            IMapper mapper,
            ILogger<GetCommentsByStatusQueryHandler> logger)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<CommentDto>> Handle(GetCommentsByStatusQuery request, CancellationToken cancellationToken)
        {
            var spec = new CommentsByStatusSpecification(request.Status, request.PageNumber, request.PageSize);
            var totalCount = await _commentRepository.CountAsync(spec, cancellationToken);
            var comments = await _commentRepository.ListAsync(spec, cancellationToken);

            return new PaginatedResponseDto<CommentDto>
            {
                Data = _mapper.Map<List<CommentDto>>(comments),
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }
    #endregion

    #region Get Comment Replies

    public class GetCommentRepliesQuery : IRequest<PaginatedResponseDto<CommentDto>>
    {
        public long CommentId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetCommentRepliesQueryHandler : IRequestHandler<GetCommentRepliesQuery, PaginatedResponseDto<CommentDto>>
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IMapper _mapper;

        public GetCommentRepliesQueryHandler(IRepository<Comment> commentRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResponseDto<CommentDto>> Handle(GetCommentRepliesQuery request, CancellationToken cancellationToken)
        {
            var spec = new CommentRepliesSpecification(request.CommentId);
            
            var totalCount = await _commentRepository.CountAsync(spec, cancellationToken);
            
            if (totalCount == 0)
                throw new NotFoundException(nameof(Comment), request.CommentId);
            
            // Get replies
            spec = new CommentRepliesSpecification(request.CommentId, request.PageNumber, request.PageSize);
            var replies = await _commentRepository.ListAsync(spec, cancellationToken);
            return new PaginatedResponseDto<CommentDto>
            {
                Data = _mapper.Map<List<CommentDto>>(replies),
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion

    #region Search Comments
    
    public class SearchCommentsQuery : IRequest<PaginatedResponseDto<CommentDto>>
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SearchCommentsQueryHandler : IRequestHandler<SearchCommentsQuery, PaginatedResponseDto<CommentDto>>
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SearchCommentsQueryHandler> _logger;
        public SearchCommentsQueryHandler(
            IRepository<Comment> commentRepository,
            IMapper mapper,
            ILogger<SearchCommentsQueryHandler> logger)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<PaginatedResponseDto<CommentDto>> Handle(SearchCommentsQuery request, CancellationToken cancellationToken)
        {
            var spec = new CommentSearchSpecification(request.SearchTerm, request.PageNumber, request.PageSize);
            var totalCount = await _commentRepository.CountAsync(new CommentSearchSpecification(request.SearchTerm), cancellationToken);
            var comments = await _commentRepository.ListAsync(spec, cancellationToken);

            return new PaginatedResponseDto<CommentDto>
            {
                Data = _mapper.Map<List<CommentDto>>(comments),
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }
    #endregion
    
    #region Get Comment Count by Status
    public class GetCommentCountByStatusQuery : IRequest<int>
    {
        public CommentStatus Status { get; set; }
    }

    public class
        GetCommentCountByStatusQueryHandler : IRequestHandler<GetCommentCountByStatusQuery, int>
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCommentCountByStatusQueryHandler> _logger;

        public GetCommentCountByStatusQueryHandler(
            IRepository<Comment> commentRepository,
            IMapper mapper,
            ILogger<GetCommentCountByStatusQueryHandler> logger)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(GetCommentCountByStatusQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new CommentCountByStatusSpecification(request.Status.ToString());
            var totalCount = await _commentRepository.CountAsync(spec, cancellationToken);
            return totalCount;
        }
    }
    #endregion
}