using System;
using System.Collections.Generic;

namespace BloggingSystem.Shared.DTOs
{
    /// <summary>
    /// DTO for comment responses
    /// </summary>
    public class CommentDto
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long UserId { get; set; }
        public long? ParentId { get; set; }
        public string? Content { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public PostSummaryDto? Post { get; set; }
        public UserSummaryDto? User { get; set; }
        public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
    }
    
    /// <summary>
    /// DTO for comment creation
    /// </summary>
    public class CreateCommentDto
    {
        public long PostId { get; set; }
        public long? ParentId { get; set; }
        public string? Content { get; set; }
    }
    
    /// <summary>
    /// DTO for comment update
    /// </summary>
    public class UpdateCommentDto
    {
        public string? Content { get; set; }
    }
    
    /// <summary>
    /// DTO for comment moderation
    /// </summary>
    public class ModerateCommentDto
    {
        public string? Status { get; set; }
        public string? ModeratorNote { get; set; }
    }
}