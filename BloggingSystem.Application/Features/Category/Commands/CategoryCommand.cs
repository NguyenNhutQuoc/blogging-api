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
using AutoMapper;

namespace BloggingSystem.Application.Features.Category.Commands
{
    #region Create Category Command

    public class CreateCategoryCommand : IRequest<CategoryDto>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long ParentId { get; set; }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly IRepository<Domain.Entities.Category> _categoryRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ISlugService _slugService;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;

        public CreateCategoryCommandHandler(
            IRepository<Domain.Entities.Category> categoryRepository,
            IDomainEventService domainEventService,
            ISlugService slugService,
            ILogger<CreateCategoryCommandHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _domainEventService = domainEventService;
            _slugService = slugService;
            _logger = logger;
        }

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Domain.Entities.Category
            {
                Name = request.Name,
                Description = request.Description,
                ParentId = request.ParentId,
                Slug = _slugService.GenerateSlug(request.Name)
            };

            await _categoryRepository.AddAsync(category, cancellationToken);
            await _domainEventService.PublishEventAsync(new CategoryCreatedEvent(category));

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug,
                ParentId = category.ParentId
            };
        }
    }

    #endregion

    #region Update Category Command

    public class UpdateCategoryCommand : IRequest<CategoryDto>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? ParentId { get; set; }
    }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly IRepository<Domain.Entities.Category> _categoryRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ISlugService _slugService;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;

        public UpdateCategoryCommandHandler(
            IRepository<Domain.Entities.Category> categoryRepository,
            IDomainEventService domainEventService,
            ISlugService slugService,
            ILogger<UpdateCategoryCommandHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _domainEventService = domainEventService;
            _slugService = slugService;
            _logger = logger;
        }

        public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (category == null)
            {
                throw new EntityNotFoundException(nameof(Domain.Entities.Category), request.Id);
            }

            category.Name = request.Name;
            category.Description = request.Description;
            category.ParentId = request.ParentId;
            category.Slug = _slugService.GenerateSlug(request.Name);

            await _categoryRepository.UpdateAsync(category, cancellationToken);
            await _domainEventService.PublishEventAsync(new CategoryUpdatedEvent(category));

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug,
                ParentId = category.ParentId
            };
        }
    }

    #endregion

    #region Delete Category Command

    public class DeleteCategoryCommand : IRequest<CategoryDto>
    {
        public long Id { get; set; }
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, CategoryDto>
    {
        private readonly IRepository<Domain.Entities.Category> _categoryRepository;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;
        private readonly IMapper _mapper;

        public DeleteCategoryCommandHandler(
            IRepository<Domain.Entities.Category> categoryRepository,
            IDomainEventService domainEventService,
            ILogger<DeleteCategoryCommandHandler> logger,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _domainEventService = domainEventService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<CategoryDto> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (category == null)
            {
                throw new EntityNotFoundException(nameof(Domain.Entities.Category), request.Id);
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            await _categoryRepository.DeleteAsync(category, cancellationToken);
            await _domainEventService.PublishEventAsync(new CategoryDeletedEvent(category));

            return categoryDto;
        }
    }

    #endregion
} 