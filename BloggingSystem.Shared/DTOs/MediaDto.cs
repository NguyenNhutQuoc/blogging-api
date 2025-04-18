using System;

namespace BloggingSystem.Shared.DTOs
{
    public class MediaDto
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? FileType { get; set; }
        public string? MimeType { get; set; }
        public long? FileSize { get; set; }
        public string? PublicId { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string? AltText { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}