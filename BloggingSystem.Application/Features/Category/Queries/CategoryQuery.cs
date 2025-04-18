using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Application.Features.Category;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Features.Category.Queries
{
    #region Get All Categories Query

    public class GetAllCategoriesQuery : IRequest<List<CategoryDto>>
    {
    }

    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
    {
        private readonly IRepository<Domain.Entities.Category> _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllCategoriesQueryHandler> _logger;

        public GetAllCategoriesQueryHandler(
            IRepository<Domain.Entities.Category> categoryRepository,
            IMapper mapper,
            ILogger<GetAllCategoriesQueryHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var specification = new CategorySpecification();
            var categories = await _categoryRepository.ListAsync(specification, cancellationToken);
            return _mapper.Map<List<CategoryDto>>(categories);
        }
    }

    #endregion

    #region Get Categories Query

    public class GetCategoriesQuery : IRequest<PaginatedResponseDto<CategoryDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, PaginatedResponseDto<CategoryDto>>
    {
        private readonly IRepository<Domain.Entities.Category> _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCategoriesQueryHandler> _logger;

        public GetCategoriesQueryHandler(
            IRepository<Domain.Entities.Category> categoryRepository,
            IMapper mapper,
            ILogger<GetCategoriesQueryHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var specification = new CategorySpecification(request.PageNumber, request.PageSize);
            var categories = await _categoryRepository.ListAsync(specification, cancellationToken);
            var totalItems = await _categoryRepository.CountAsync(specification, cancellationToken);

            return new PaginatedResponseDto<CategoryDto>
            {
                Data = _mapper.Map<List<CategoryDto>>(categories),
                TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize),
                PageIndex = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }

    #endregion

    #region Get Category By ID Query

    public class GetCategoryByIdQuery : IRequest<CategoryDto>
    {
        public long Id { get; set; }
    }

    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly IRepository<Domain.Entities.Category> _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCategoryByIdQueryHandler> _logger;

        public GetCategoryByIdQueryHandler(
            IRepository<Domain.Entities.Category> categoryRepository,
            IMapper mapper,
            ILogger<GetCategoryByIdQueryHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (category == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.Category), request.Id);
            }

            return _mapper.Map<CategoryDto>(category);
        }
    }

    #endregion
}