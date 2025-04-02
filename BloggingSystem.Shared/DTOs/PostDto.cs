namespace BloggingSystem.Shared.DTOs;

public class PostDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string? Excerpt { get; set; }
    public string Content { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public string Status { get; set; }
    public string CommentStatus { get; set; }
    public long ViewsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Simplified post DTO for list views
/// </summary>
public class PostSummaryDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string? Excerpt { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public string Status { get; set; }
    public long ViewsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
        
    public AuthorDto Author { get; set; }
}