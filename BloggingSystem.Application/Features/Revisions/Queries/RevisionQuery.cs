using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Commons.Specifications;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Shared.Exceptions;
using DiffPlex;
using DiffPlex.Chunkers;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Features.Revisions.Queries
{
    #region Get Revisions By Post

    public class GetRevisionsByPostQuery : IRequest<PaginatedResponseDto<RevisionDto>>
    {
        public long PostId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetRevisionsByPostQueryHandler : IRequestHandler<GetRevisionsByPostQuery, PaginatedResponseDto<RevisionDto>>
    {
        private readonly IRepository<Revision> _revisionRepository;
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetRevisionsByPostQueryHandler> _logger;

        public GetRevisionsByPostQueryHandler(
            IRepository<Revision> revisionRepository,
            IRepository<Domain.Entities.Post> postRepository,
            IMapper mapper,
            ILogger<GetRevisionsByPostQueryHandler> logger)
        {
            _revisionRepository = revisionRepository;
            _postRepository = postRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<RevisionDto>> Handle(GetRevisionsByPostQuery request, CancellationToken cancellationToken)
        {
            // Validate post exists
            var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
            if (post == null)
                throw new NotFoundException(nameof(Post), request.PostId);

            // Create specification for revisions by post with pagination
            var spec = new RevisionsByPostSpecification(request.PostId, request.PageNumber, request.PageSize);
            
            // Create specification for count without pagination
            var countSpec = new RevisionsByPostSpecification(request.PostId);
            
            // Get total count
            var totalCount = await _revisionRepository.CountAsync(countSpec, cancellationToken);
            
            // Get revisions with pagination
            var revisions = await _revisionRepository.ListAsync(spec, cancellationToken);
            
            // Map to DTOs
            var revisionDtos = _mapper.Map<List<RevisionDto>>(revisions);
            
            return new PaginatedResponseDto<RevisionDto>
            {
                Data = revisionDtos,
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion
    #region Get Revision Count By Post

    public class GetRevisionCountByPostQuery : IRequest<int>
    {
        public long PostId { get; set; }
    }

    public class GetRevisionCountByPostQueryHandler : IRequestHandler<GetRevisionCountByPostQuery, int>
    {
        private readonly IRepository<Domain.Entities.Revision> _revisionRepository;
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly ILogger<GetRevisionCountByPostQueryHandler> _logger;

        public GetRevisionCountByPostQueryHandler(
            IRepository<Domain.Entities.Revision> revisionRepository,
            IRepository<Domain.Entities.Post> postRepository,
            ILogger<GetRevisionCountByPostQueryHandler> logger)
        {
            _revisionRepository = revisionRepository;
            _postRepository = postRepository;
            _logger = logger;
        }

        public async Task<int> Handle(GetRevisionCountByPostQuery request, CancellationToken cancellationToken)
        {
            // Verify post exists
            var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
            if (post == null)
                throw new NotFoundException("Post", request.PostId);

            // Get revision count
            var count = await _revisionRepository.CountAsync(new RevisionsByPostSpecification(request.PostId), cancellationToken);

            return count;
        }
    }

    #endregion
    #region Get Revision By Id

    public class GetRevisionByIdQuery : IRequest<RevisionDto>
    {
        public long Id { get; set; }
    }

    public class GetRevisionByIdQueryHandler : IRequestHandler<GetRevisionByIdQuery, RevisionDto>
    {
        private readonly IRepository<Revision> _revisionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetRevisionByIdQueryHandler> _logger;

        public GetRevisionByIdQueryHandler(
            IRepository<Revision> revisionRepository,
            IMapper mapper,
            ILogger<GetRevisionByIdQueryHandler> logger)
        {
            _revisionRepository = revisionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RevisionDto> Handle(GetRevisionByIdQuery request, CancellationToken cancellationToken)
        {
            // Create specification for revision including related entities
            var spec = new RevisionByIdSpecification(request.Id);
            var revision = await _revisionRepository.FirstOrDefaultAsync(spec, cancellationToken);

            if (revision == null)
                throw new NotFoundException(nameof(Revision), request.Id);

            return _mapper.Map<RevisionDto>(revision);
        }
    }

    #endregion
    #region Compare Revisions

    public class CompareRevisionsQuery : IRequest<RevisionComparisonDto>
    {
        public long SourceRevisionId { get; set; }
        public long TargetRevisionId { get; set; }
    }

    public class CompareRevisionsQueryHandler : IRequestHandler<CompareRevisionsQuery, RevisionComparisonDto>
    {
        private readonly IRepository<Revision> _revisionRepository;
        private readonly ILogger<CompareRevisionsQueryHandler> _logger;

        public CompareRevisionsQueryHandler(
            IRepository<Revision> revisionRepository,
            ILogger<CompareRevisionsQueryHandler> logger)
        {
            _revisionRepository = revisionRepository;
            _logger = logger;
        }

        public async Task<RevisionComparisonDto> Handle(CompareRevisionsQuery request, CancellationToken cancellationToken)
        {
            // Get source revision
            var sourceSpec = new RevisionByIdWithDetailsSpecification(request.SourceRevisionId);
            var sourceRevision = await _revisionRepository.FirstOrDefaultAsync(sourceSpec, cancellationToken);

            if (sourceRevision == null)
                throw new NotFoundException("Source Revision", request.SourceRevisionId);

            // Get target revision
            var targetSpec = new RevisionByIdWithDetailsSpecification(request.TargetRevisionId);
            var targetRevision = await _revisionRepository.FirstOrDefaultAsync(targetSpec, cancellationToken);

            if (targetRevision == null)
                throw new NotFoundException("Target Revision", request.TargetRevisionId);

            // Ensure revisions belong to the same post
            if (sourceRevision.PostId != targetRevision.PostId)
                throw new InvalidOperationException("Cannot compare revisions from different posts");

            // Create diff
            var diffBuilder = new InlineDiffBuilder(new Differ());
            var diff = diffBuilder.BuildDiffModel(sourceRevision.Content, targetRevision.Content, 
                ignoreWhitespace: false, ignoreCase: false, new WordChunker());

            // Build list of changes
            var changes = new List<RevisionDiffLineDto>();
            
            foreach (var line in diff.Lines)
            {
                changes.Add(new RevisionDiffLineDto
                {
                    Type = line.Type.ToString(),
                    Text = line.Text,
                    Position = line.Position ?? 0,
                    SubPieces = line.SubPieces?.Select(sp => new RevisionDiffSubPieceDto
                    {
                        Type = sp.Type.ToString(),
                        Text = sp.Text,
                        Position = sp.Position ?? 0
                    }).ToList()
                });
            }

            // Create comparison result
            return new RevisionComparisonDto
            {
                SourceRevisionId = sourceRevision.Id,
                TargetRevisionId = targetRevision.Id,
                SourceRevisionNumber = sourceRevision.RevisionNumber,
                TargetRevisionNumber = targetRevision.RevisionNumber,
                SourceUser = sourceRevision.User.Username,
                TargetUser = targetRevision.User.Username,
                SourceCreatedAt = sourceRevision.CreatedAt,
                TargetCreatedAt = targetRevision.CreatedAt,
                PostId = sourceRevision.PostId,
                Changes = changes,
                LinesAdded = diff.Lines.Count(l => l.Type == ChangeType.Inserted),
                LinesRemoved = diff.Lines.Count(l => l.Type == ChangeType.Deleted),
                LinesChanged = diff.Lines.Count(l => l.Type == ChangeType.Modified)
            };
        }
    }

    #endregion

    #region Compare Revisions By Post
    
    public class CompareRevisionsByPostQuery : IRequest<RevisionComparisonDto>
    {
        public long PostId { get; set; }
        public long SourceRevisionId { get; set; }
    }

    public class
        CompareRevisionsByPostQueryHandler : IRequestHandler<CompareRevisionsByPostQuery, RevisionComparisonDto>
    {
        private readonly IRepository<Revision> _revisionRepository;
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly ILogger<CompareRevisionsByPostQueryHandler> _logger;

        public CompareRevisionsByPostQueryHandler(
            IRepository<Revision> revisionRepository,
            IRepository<Domain.Entities.Post> postRepository,
            ILogger<CompareRevisionsByPostQueryHandler> logger)
        {
            _revisionRepository = revisionRepository;
            _postRepository = postRepository;
            _logger = logger;
        }

        public async Task<RevisionComparisonDto> Handle(CompareRevisionsByPostQuery request,
            CancellationToken cancellationToken)
        {
            // Verify post exists
            var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
            if (post == null)
                throw new NotFoundException(nameof(Post), request.PostId);

            // Create specification for source revision
            var sourceSpec = new RevisionByIdWithDetailsSpecification(request.SourceRevisionId);
            var targetRevision = await _revisionRepository.FirstOrDefaultAsync(sourceSpec, cancellationToken);

            if (targetRevision == null)
                throw new NotFoundException("Source Revision", request.SourceRevisionId);

            // Create specification for target revision

            // Ensure revisions belong to the same post
            if (targetRevision.PostId != post.Id)
                throw new InvalidOperationException("Cannot compare revisions from different posts");

            // Create diff
            var diffBuilder = new InlineDiffBuilder(new Differ());
            var diff = diffBuilder.BuildDiffModel(post.Content, targetRevision.Content,
                ignoreWhitespace: false, ignoreCase: false, new WordChunker());

            // Build list of changes
            var changes = new List<RevisionDiffLineDto>();

            foreach (var line in diff.Lines)
            {
                changes.Add(new RevisionDiffLineDto
                {
                    Type = line.Type.ToString(),
                    Text = line.Text,
                    Position = line.Position ?? 0,
                    SubPieces = line.SubPieces?.Select(sp => new RevisionDiffSubPieceDto
                    {
                        Type = sp.Type.ToString(),
                        Text = sp.Text,
                        Position = sp.Position ?? 0
                    }).ToList()
                });
            }

            // Create comparison result
            return new RevisionComparisonDto
            {
                SourceRevisionId = post.Id,
                TargetRevisionId = targetRevision.Id,
                SourceRevisionNumber = 0,
                TargetRevisionNumber = targetRevision.RevisionNumber,
                SourceUser = post.Author.Username,
                TargetUser = targetRevision.User.Username,
                SourceCreatedAt = post.CreatedAt,
                TargetCreatedAt = targetRevision.CreatedAt,
                PostId = post.Id,
                Changes = changes,
                LinesAdded = diff.Lines.Count(l => l.Type == ChangeType.Inserted),
                LinesRemoved = diff.Lines.Count(l => l.Type == ChangeType.Deleted),
                LinesChanged = diff.Lines.Count(l => l.Type == ChangeType.Modified)
            };
        }
    }

    #endregion
    #region Get Latest Revision By Post

    public class GetLatestRevisionByPostQuery : IRequest<RevisionDto>
    {
        public long PostId { get; set; }
    }

    public class GetLatestRevisionByPostQueryHandler : IRequestHandler<GetLatestRevisionByPostQuery, RevisionDto>
    {
        private readonly IRepository<Revision> _revisionRepository;
        private readonly IRepository<Domain.Entities.Post> _postRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetLatestRevisionByPostQueryHandler> _logger;

        public GetLatestRevisionByPostQueryHandler(
            IRepository<Revision> revisionRepository,
            IRepository<Domain.Entities.Post> postRepository,
            IMapper mapper,
            ILogger<GetLatestRevisionByPostQueryHandler> logger)
        {
            _revisionRepository = revisionRepository;
            _postRepository = postRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RevisionDto> Handle(GetLatestRevisionByPostQuery request, CancellationToken cancellationToken)
        {
            // Verify post exists
            var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
            if (post == null)
                throw new NotFoundException(nameof(Post), request.PostId);

            // Create specification for latest revision
            var spec = new LatestRevisionByPostSpecification(request.PostId);
            var revision = await _revisionRepository.FirstOrDefaultAsync(spec, cancellationToken);

            if (revision == null)
                throw new NotFoundException($"No revisions found for Post {request.PostId}");

            return _mapper.Map<RevisionDto>(revision);
        }
    }

    #endregion
}