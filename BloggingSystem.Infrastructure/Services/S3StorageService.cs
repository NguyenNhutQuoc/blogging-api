using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.Runtime;

namespace BloggingSystem.Infrastructure.Services
{
    public class S3StorageService : IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3Settings _settings;
        private readonly ILogger<S3StorageService> _logger;

        public S3StorageService(
            IOptions<S3Settings> settings,
            ILogger<S3StorageService> logger)
        {
            _settings = settings.Value;
            
            // Configure the client
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_settings.Region),
                ServiceURL = _settings.Endpoint,
                ForcePathStyle = true,
                UseHttp = false,
                SignatureVersion = "2",

            };
            
            _s3Client = new AmazonS3Client(
                _settings.AccessKey,
                _settings.SecretKey,
                config);
            
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
                return await UploadToS3Async(stream, file.FileName, file.ContentType, file.Length, folder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to S3: {FileName}", file.FileName);
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
                return await UploadToS3Async(stream, fileName, contentType, fileBytes.Length, folder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file bytes to S3: {FileName}", fileName);
                return new FileUploadResult
                {
                    IsSuccessful = false,
                    Error = ex.Message
                };
            }
        }

        public async Task<bool> DeleteAsync(string fileId)
        {
            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _settings.BucketName,
                    Key = fileId
                };

                var response = await _s3Client.DeleteObjectAsync(deleteRequest);
                return response.HttpStatusCode == HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file from S3: {FileId}", fileId);
                return false;
            }
        }

        public string GetUrl(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
                return null;

            if (!string.IsNullOrEmpty(_settings.Endpoint))
            {
                // Use CDN URL if configured
                return $"{_settings.Endpoint.TrimEnd('/')}/{_settings.BucketName}/{fileId}";
            }
            else
            {
                // Use S3 URL
                var regionEndpoint = RegionEndpoint.GetBySystemName(_settings.Region);
                return $"https://{_settings.BucketName}.s3.{regionEndpoint.SystemName}.amazonaws.com/{fileId}";
            }
        }

        private async Task<FileUploadResult> UploadToS3Async(
            Stream stream,
            string fileName,
            string contentType,
            long length,
            string folder = null)
        {
            try
            {
                // Generate a unique key for the file
                string fileKey = GenerateUniqueFileKey(fileName, folder);

                // Create TransferUtility for easy uploads
                using var transferUtility = new TransferUtility(_s3Client);

                // Configure upload request
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    BucketName = _settings.BucketName,
                    Key = fileKey,
                    ContentType = contentType,
                    CannedACL = S3CannedACL.PublicRead // Make the file publicly accessible
                };

                // Upload the file
                await transferUtility.UploadAsync(uploadRequest);

                // Get file metadata if it's an image
                int width = 0;
                int height = 0;
                string format = Path.GetExtension(fileName).TrimStart('.');

                // Create final URL
                string url = GetUrl(fileKey);

                return new FileUploadResult
                {
                    FileId = fileKey,
                    Url = url,
                    SecureUrl = url,
                    Format = format,
                    Width = width,
                    Height = height,
                    Length = length,
                    FileName = fileName,
                    IsSuccessful = true
                };
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to S3: {FileName}", fileName);
                return new FileUploadResult
                {
                    IsSuccessful = false,
                    Error = ex.Message
                };
            }
        }

        private string GenerateUniqueFileKey(string fileName, string folder)
        {
            // Create a unique filename
            var extension = Path.GetExtension(fileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var sanitizedFileName = SanitizeFileName(fileNameWithoutExtension);
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            
            var uniqueFileName = $"{sanitizedFileName}-{timestamp}-{uniqueId}{extension}";
            
            // Add folder prefix if provided
            if (!string.IsNullOrEmpty(folder))
            {
                var sanitizedFolder = folder.Trim('/');
                return $"{sanitizedFolder}/{uniqueFileName}";
            }
            
            return uniqueFileName;
        }

        private string SanitizeFileName(string fileName)
        {
            // Replace invalid characters with hyphens
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = string.Join("", fileName.Split(invalidChars));
            
            // Replace spaces with hyphens
            sanitized = sanitized.Replace(" ", "-");
            
            // Remove consecutive hyphens
            while (sanitized.Contains("--"))
            {
                sanitized = sanitized.Replace("--", "-");
            }
            
            // Trim and lowercase
            sanitized = sanitized.Trim('-').ToLowerInvariant();
            
            // Ensure the filename is not empty
            if (string.IsNullOrEmpty(sanitized))
            {
                sanitized = "file";
            }
            
            return sanitized;
        }
    }
}