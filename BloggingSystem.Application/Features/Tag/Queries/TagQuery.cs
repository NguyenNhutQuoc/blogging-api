using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.Tag;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Features.Tag.Queries
{
    #region Get All Tags Query

    public class GetAllTagsQuery : IRequest<List<TagDto>>
    {
    }

    public class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, List<TagDto>>
    {
        private readonly IRepository<Domain.Entities.Tag> _tagRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllTagsQueryHandler> _logger;

        public GetAllTagsQueryHandler(
            IRepository<Domain.Entities.Tag> tagRepository,
            IMapper mapper,
            ILogger<GetAllTagsQueryHandler> logger)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
        {
            var specification = new TagSpecification();
            var tags = await _tagRepository.ListAsync(specification, cancellationToken);
            return _mapper.Map<List<TagDto>>(tags);
        }
    }

    #endregion

    #region Get Tags Query

    public class GetTagsQuery : IRequest<PaginatedResponseDto<TagDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, PaginatedResponseDto<TagDto>>
    {
        private readonly IRepository<Domain.Entities.Tag> _tagRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTagsQueryHandler> _logger;

        public GetTagsQueryHandler(
            IRepository<Domain.Entities.Tag> tagRepository,
            IMapper mapper,
            ILogger<GetTagsQueryHandler> logger)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
        {
            var specification = new TagSpecification(request.PageNumber, request.PageSize);
            var tags = await _tagRepository.ListAsync(specification, cancellationToken);
            var totalItems = await _tagRepository.CountAsync(specification, cancellationToken);

            return new PaginatedResponseDto<TagDto>
            {
                Data = _mapper.Map<List<TagDto>>(tags),
                TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize),
                PageIndex = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }

    #endregion

    #region Get Tag By ID Query

    public class GetTagByIdQuery : IRequest<TagDto>
    {
        public long Id { get; set; }
    }

    public class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, TagDto>
    {
        private readonly IRepository<Domain.Entities.Tag> _tagRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTagByIdQueryHandler> _logger;

        public GetTagByIdQueryHandler(
            IRepository<Domain.Entities.Tag> tagRepository,
            IMapper mapper,
            ILogger<GetTagByIdQueryHandler> logger)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TagDto> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
        {
            var tag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (tag == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.Tag), request.Id);
            }

            return _mapper.Map<TagDto>(tag);
        }
    }

    #endregion
} 