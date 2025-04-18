using Microsoft.AspNetCore.Http;

namespace BloggingSystem.Application.Commons.Interfaces
{
    public interface ICloudinaryService
    {
        Task<CloudinaryUploadResult> UploadAsync(IFormFile file, string folder);
        Task<CloudinaryUploadResult> UploadAsync(byte[] fileBytes, string fileName, string contentType, string folder);
        Task<bool> DeleteAsync(string publicId);
    }

    public class CloudinaryUploadResult
    {
        public string? PublicId { get; set; }
        public string? Url { get; set; }
        public string? SecureUrl { get; set; }
        public string? Format { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public long Length { get; set; }
        public string? FileName { get; set; }
        public bool IsSuccessful { get; set; }
        public string? Error { get; set; }
    }
    public interface IFileStorageService
    {
        Task<FileUploadResult> UploadAsync(IFormFile? file, string? folder = null);
        Task<FileUploadResult> UploadAsync(byte[] fileBytes, string? fileName, string? contentType, string? folder = null);
        Task<bool> DeleteAsync(string? publicId);
    }

    
    public class FileUploadResult
    {
        public string? FileId { get; set; }
        public string? Url { get; set; }
        public string? SecureUrl { get; set; }
        public string? Format { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public long Length { get; set; }
        public string? FileName { get; set; }
        public bool IsSuccessful { get; set; }
        public string? Error { get; set; }
    }
}