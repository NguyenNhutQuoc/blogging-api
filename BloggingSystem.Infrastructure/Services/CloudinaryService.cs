using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Infrastructure.Settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BloggingSystem.Infrastructure.Services
{
    public class CloudinaryService : IFileStorageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(
            IOptions<CloudinarySettings> options,
            ILogger<CloudinaryService> logger)
        {
            var account = new Account(
                options.Value.CloudName,
                options.Value.ApiKey,
                options.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
            _logger = logger;
        }

        public async Task<FileUploadResult> UploadAsync(IFormFile file, string folder = null)
        {
            if (file == null || file.Length == 0)
            {
                return new FileUploadResult
                {
                    IsSuccessful = false,
                    Error = "File is empty"
                };
            }

            try
            {
                using var stream = file.OpenReadStream();
                return await UploadToCloudinaryAsync(stream, file.FileName, file.ContentType, file.Length, folder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to Cloudinary: {FileName}", file.FileName);
                return new FileUploadResult
                {
                    IsSuccessful = false,
                    Error = ex.Message
                };
            }
        }

        public async Task<FileUploadResult> UploadAsync(byte[] fileBytes, string fileName, string contentType, string folder = null)
        {
            if (fileBytes == null || fileBytes.Length == 0)
            {
                return new FileUploadResult
                {
                    IsSuccessful = false,
                    Error = "File is empty"
                };
            }

            try
            {
                using var stream = new MemoryStream(fileBytes);
                return await UploadToCloudinaryAsync(stream, fileName, contentType, fileBytes.Length, folder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file bytes to Cloudinary: {FileName}", fileName);
                return new FileUploadResult
                {
                    IsSuccessful = false,
                    Error = ex.Message
                };
            }
        }

        public async Task<bool> DeleteAsync(string publicId)
        {
            try
            {
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);
                return result.Result == "ok";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file from Cloudinary: {PublicId}", publicId);
                return false;
            }
        }

        private async Task<FileUploadResult> UploadToCloudinaryAsync(
            Stream stream, 
            string fileName, 
            string contentType, 
            long length,
            string folder = null)
        {
            var extension = Path.GetExtension(fileName);
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            // Set folder if provided
            if (!string.IsNullOrEmpty(folder))
            {
                uploadParams.Folder = folder;
            }

            // Add transformation for image optimization
            uploadParams.Transformation = new Transformation()
                .Quality("auto")
                .FetchFormat("auto");

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            
            if (uploadResult.Error != null)
            {
                return new FileUploadResult
                {
                    IsSuccessful = false,
                    Error = uploadResult.Error.Message
                };
            }

            return new FileUploadResult
            {
                Url = uploadResult.Url.ToString(),
                SecureUrl = uploadResult.SecureUrl.ToString(),
                Format = uploadResult.Format,
                Width = uploadResult.Width,
                Height = uploadResult.Height,
                Length = length,
                FileName = fileName,
                IsSuccessful = true
            };
        }
    }
}