using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Domain.Events;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Features.Media.Commands
{
    #region Upload Media Command

    public class UploadMediaCommand : IRequest<MediaDto>
    {
        public IFormFile? File { get; set; }
        public string? Folder { get; set; }
    }

    public class UploadMediaCommandHandler : IRequestHandler<UploadMediaCommand, MediaDto>
    {
        private readonly IRepository<Domain.Entities.Medium> _mediaRepository;
        private readonly IFileStorageService _cloudinaryService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<UploadMediaCommandHandler> _logger;

        public UploadMediaCommandHandler(
            IRepository<Domain.Entities.Medium> mediaRepository,
            IFileStorageService cloudinaryService,
            ICurrentUserService currentUserService,
            IDomainEventService domainEventService,
            ILogger<UploadMediaCommandHandler> logger)
        {
            _mediaRepository = mediaRepository;
            _cloudinaryService = cloudinaryService;
            _currentUserService = currentUserService;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<MediaDto> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Upload file to Cloudinary
            if (request.File == null)
                throw new ArgumentNullException(nameof(request.File), "File cannot be null");

            var uploadResult = await _cloudinaryService.UploadAsync(request.File, request.Folder);
            if (!uploadResult.IsSuccessful)
                throw new DomainException($"Failed to upload file: {uploadResult.Error}");

            // Create media entity
            var media = Medium.Create(

                userId.Value,
                Path.GetFileName(request.File.FileName),
                uploadResult.SecureUrl ?? throw new DomainException("Upload result does not contain a secure URL"),
                GetFileType(request.File.ContentType),
                request.File.ContentType,
                request.File.Length,
                "",
                uploadResult.Width > 0 ? uploadResult.Width : 0,
                uploadResult.Height > 0 ? uploadResult.Height : 0
            );

            // Save to database
            await _mediaRepository.AddAsync(media, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(media.DomainEvents);

            // Return DTO
            return new MediaDto
            {
                Id = media.Id,
                UserId = media.UserId,
                FileName = media.FileName,
                FilePath = media.FilePath,
                FileType = media.FileType,
                MimeType = media.MimeType,
                FileSize = media.FileSize,
                PublicId = media.PublicId,
                Width = media.Width,
                Height = media.Height,
                AltText = media.AltText,
                Description = media.Description,
                CreatedAt = media.CreatedAt,
            };
        }

        private string GetFileType(string mimeType)
        {
            if (mimeType.StartsWith("image/"))
                return "image";
            if (mimeType.StartsWith("video/"))
                return "video";
            if (mimeType.StartsWith("audio/"))
                return "audio";
            return "document";
        }
    }

    #endregion
    #region Delete Media Command

    public class DeleteMediaCommand : IRequest<bool>
    {
        public long Id { get; set; }
    }

    public class DeleteMediaCommandHandler : IRequestHandler<DeleteMediaCommand, bool>
    {
        private readonly IRepository<Domain.Entities.Medium> _mediaRepository;
        private readonly IFileStorageService _cloudinaryService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDomainEventService _domainEventService;
        private readonly ILogger<DeleteMediaCommandHandler> _logger;

        public DeleteMediaCommandHandler(
            IRepository<Domain.Entities.Medium> mediaRepository,
            IFileStorageService cloudinaryService,
            ICurrentUserService currentUserService,
            IDomainEventService domainEventService,
            ILogger<DeleteMediaCommandHandler> logger)
        {
            _mediaRepository = mediaRepository;
            _cloudinaryService = cloudinaryService;
            _currentUserService = currentUserService;
            _domainEventService = domainEventService;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteMediaCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Get media
            var media = await _mediaRepository.GetByIdAsync(request.Id, cancellationToken);
            if (media == null)
                throw new NotFoundException("Media", request.Id);

            // Check if user owns the media or is an admin
            bool isAdmin = _currentUserService.IsInRole("Admin");
            if (media.UserId != userId && !isAdmin)
                throw new ForbiddenException("You don't have permission to delete this media");

            // Add domain event before deleting
            media.DeletedMedia(userId.Value);

            // Save domain events to publish after deletion
            var domainEvents = media.DomainEvents;

            // Delete from Cloudinary
            if (!string.IsNullOrEmpty(media.PublicId))
            {
                var deleted = await _cloudinaryService.DeleteAsync(media.PublicId);
                if (!deleted)
                {
                    _logger.LogWarning("Failed to delete file from Cloudinary: {PublicId}", media.PublicId);
                    // Continue with database deletion even if Cloudinary deletion fails
                }
            }

            // Delete from database
            await _mediaRepository.DeleteAsync(media, cancellationToken);

            // Publish domain events
            await _domainEventService.PublishEventsAsync(domainEvents);

            return true;
        }
    }

    #endregion
    #region Update Media Metadata Command

    public class UpdateMediaMetadataCommand : IRequest<MediaDto>
    {
        public long Id { get; set; }
        public string? AltText { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateMediaMetadataCommandHandler : IRequestHandler<UpdateMediaMetadataCommand, MediaDto>
    {
        private readonly IRepository<Domain.Entities.Medium> _mediaRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpdateMediaMetadataCommandHandler> _logger;

        public UpdateMediaMetadataCommandHandler(
            IRepository<Domain.Entities.Medium> mediaRepository,
            ICurrentUserService currentUserService,
            IDomainEventService domainEventService,
            ILogger<UpdateMediaMetadataCommandHandler> logger)
        {
            _mediaRepository = mediaRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<MediaDto> Handle(UpdateMediaMetadataCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated");

            // Get media
            var media = await _mediaRepository.GetByIdAsync(request.Id, cancellationToken);
            if (media == null)
                throw new NotFoundException("Media", request.Id);

            // Check if user owns the media or is an admin
            bool isAdmin = _currentUserService.IsInRole("Admin");
            if (media.UserId != userId && !isAdmin)
                throw new ForbiddenException("You don't have permission to update this media");

            // Update metadata
            media.UpdateMetadata(request.AltText, request.Description);

            // Update in database
            await _mediaRepository.UpdateAsync(media, cancellationToken);

            // Return DTO
            return new MediaDto
            {
                Id = media.Id,
                UserId = media.UserId,
                FileName = media.FileName,
                FilePath = media.FilePath,
                FileType = media.FileType,
                MimeType = media.MimeType,
                FileSize = media.FileSize,
                PublicId = media.PublicId,
                Width = media.Width,
                Height = media.Height,
                AltText = media.AltText,
                Description = media.Description,
                CreatedAt = media.CreatedAt,
            };
        }
    }

    #endregion
}