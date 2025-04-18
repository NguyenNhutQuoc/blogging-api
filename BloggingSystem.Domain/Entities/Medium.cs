using System;
using System.Collections.Generic;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Events;

namespace BloggingSystem.Domain.Entities;

public partial class Medium: BaseEntity
{
    public long UserId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public string MimeType { get; set; } = null!;

    public long FileSize { get; set; }

    public string? AltText { get; set; }

    public string? Description { get; set; }
    public string? PublicId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public virtual ICollection<PostMedium> PostMedia { get; set; } = new List<PostMedium>();

    public virtual User User { get; set; } = null!;
    
    private Medium() {}
    
    public Medium(
        long userId,
        string fileName,
        string filePath,
        string fileType,
        string mimeType,
        long fileSize,
        string? publicId = null,
        int width = 0,
        int height = 0,
        string? altText = null,
        string? description = null)
    {
        UserId = userId;
        FileName = fileName;
        FilePath = filePath;
        FileType = fileType;
        MimeType = mimeType;
        FileSize = fileSize;
        PublicId = publicId;
        Width = width;
        Height = height;
        AltText = altText;
        Description = description;
    }

    // Factory method
    public static Medium Create(
        long userId,
        string fileName,
        string filePath,
        string fileType,
        string mimeType,
        long fileSize,
        string? publicId = null,
        int width = 0,
        int height = 0,
        string? altText = null,
        string? description = null)
    {
        var media = new Medium(
            userId,
            fileName,
            filePath,
            fileType,
            mimeType,
            fileSize,
            publicId,
            width,
            height,
            altText,
            description);

        
        // Add domain event
        media.AddDomainEvent(new MediaUploadedEvent(media.Id, userId, media.FileName));
        
        return media;
    }

    // Methods
    public void UpdateMetadata(string? altText, string? description)
    {
        AltText = altText;
        Description = description;
        SetModified();
    }

    public void DeletedMedia(long userId) {
        AddDomainEvent(new MediaDeletedEvent(Id, userId, FileName));
    }
}
