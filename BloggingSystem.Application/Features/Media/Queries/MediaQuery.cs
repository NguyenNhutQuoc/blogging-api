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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Features.Media.Queries
{
    #region Get Media Query

    public class GetMediaQuery : IRequest<PaginatedResponseDto<MediaDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetMediaQueryHandler : IRequestHandler<GetMediaQuery, PaginatedResponseDto<MediaDto>>
    {
        private readonly IRepository<Domain.Entities.Medium> _mediaRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetMediaQueryHandler> _logger;

        public GetMediaQueryHandler(
            IRepository<Domain.Entities.Medium> mediaRepository,
            IMapper mapper,
            ILogger<GetMediaQueryHandler> logger)
        {
            _mediaRepository = mediaRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<MediaDto>> Handle(GetMediaQuery request, CancellationToken cancellationToken)
        {
            // Create specification for media with pagination
            var spec = new GetMediaSpecification(request.PageNumber, request.PageSize);
            
            // Get count without pagination
            var countSpec = new GetMediaSpecification();
            var totalCount = await _mediaRepository.CountAsync(countSpec, cancellationToken);
            
            // Get media items with pagination
            var mediaItems = await _mediaRepository.ListAsync(spec, cancellationToken);
            
            // Map to DTOs
            var mediaDtos = _mapper.Map<List<MediaDto>>(mediaItems);
            
            return new PaginatedResponseDto<MediaDto>
            {
                Data = mediaDtos,
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion
    #region Get Media By Id Query

    public class GetMediaByIdQuery : IRequest<MediaDto>
    {
        public long Id { get; set; }
    }

    public class GetMediaByIdQueryHandler : IRequestHandler<GetMediaByIdQuery, MediaDto>
    {
        private readonly IRepository<Domain.Entities.Medium> _mediaRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetMediaByIdQueryHandler> _logger;

        public GetMediaByIdQueryHandler(
            IRepository<Domain.Entities.Medium> mediaRepository,
            IMapper mapper,
            ILogger<GetMediaByIdQueryHandler> logger)
        {
            _mediaRepository = mediaRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<MediaDto> Handle(GetMediaByIdQuery request, CancellationToken cancellationToken)
        {
            var media = await _mediaRepository.GetByIdAsync(request.Id, cancellationToken);
            if (media == null)
                throw new NotFoundException("Media", request.Id);

            return _mapper.Map<MediaDto>(media);
        }
    }

    #endregion
    
    #region Get Current User Media Query

    public class GetCurrentUserMediaQuery : IRequest<PaginatedResponseDto<MediaDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetCurrentUserMediaQueryHandler : IRequestHandler<GetCurrentUserMediaQuery, PaginatedResponseDto<MediaDto>>
    {
        private readonly IRepository<Domain.Entities.Medium> _mediaRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCurrentUserMediaQueryHandler> _logger;

        public GetCurrentUserMediaQueryHandler(
            IRepository<Domain.Entities.Medium> mediaRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetCurrentUserMediaQueryHandler> logger)
        {
            _mediaRepository = mediaRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponseDto<MediaDto>> Handle(GetCurrentUserMediaQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Create specification for user's media with pagination
            var spec = new GetMediaByUserIdSpecification(userId.Value, request.PageNumber, request.PageSize);
            
            // Get count without pagination
            var countSpec = new GetMediaByUserIdSpecification(userId.Value);
            var totalCount = await _mediaRepository.CountAsync(countSpec, cancellationToken);
            
            // Get media items with pagination
            var mediaItems = await _mediaRepository.ListAsync(spec, cancellationToken);
            
            // Map to DTOs
            var mediaDtos = _mapper.Map<List<MediaDto>>(mediaItems);
            
            return new PaginatedResponseDto<MediaDto>
            {
                Data = mediaDtos,
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }

    #endregion
    
    #region Upload File Request
    public class UploadFileRequest : IRequest<MediaDto>
    {
        public IFormFile File { get; set; }
    }
    
    #endregion
}