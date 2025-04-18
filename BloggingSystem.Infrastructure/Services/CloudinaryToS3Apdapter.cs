using BloggingSystem.Application.Commons.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BloggingSystem.Infrastructure.Services
{
    /// <summary>
    /// Adapter class that implements ICloudinaryService using the new IFileStorageService
    /// This allows for a smooth transition from Cloudinary to S3 without changing existing code
    /// </summary>
    public class CloudinaryToS3Adapter : ICloudinaryService
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<CloudinaryToS3Adapter> _logger;

        public CloudinaryToS3Adapter(
            IFileStorageService fileStorageService,
            ILogger<CloudinaryToS3Adapter> logger)
        {
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        public async Task<CloudinaryUploadResult> UploadAsync(IFormFile file, string folder = null)
        {
            var result = await _fileStorageService.UploadAsync(file, folder);
            return MapToCloudinaryResult(result);
        }

        public async Task<CloudinaryUploadResult> UploadAsync(byte[] fileBytes, string fileName, string contentType, string folder = null)
        {
            var result = await _fileStorageService.UploadAsync(fileBytes, fileName, contentType, folder);
            return MapToCloudinaryResult(result);
        }

        public async Task<bool> DeleteAsync(string publicId)
        {
            return await _fileStorageService.DeleteAsync(publicId);
        }

        private CloudinaryUploadResult MapToCloudinaryResult(FileUploadResult fileResult)
        {
            if (!fileResult.IsSuccessful)
            {
                return new CloudinaryUploadResult
                {
                    IsSuccessful = false,
                    Error = fileResult.Error
                };
            }

            return new CloudinaryUploadResult
            {
                PublicId = fileResult.FileId,
                Url = fileResult.Url,
                SecureUrl = fileResult.SecureUrl,
                Format = fileResult.Format,
                Width = fileResult.Width,
                Height = fileResult.Height,
                Length = fileResult.Length,
                FileName = fileResult.FileName,
                IsSuccessful = true
            };
        }
    }
}