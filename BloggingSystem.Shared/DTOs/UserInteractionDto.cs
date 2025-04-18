using System;
using BloggingSystem.Shared.DTOs;

namespace BloggingSystem.Shared.DTOs
{
    public class FollowDto
    {
        public long Id { get; set; }
        public long FollowerId { get; set; }
        public long FollowingId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class LikeDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string EntityType { get; set; }
        public long EntityId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SavedPostDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long PostId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

public class SavedPostSummaryDto : PostSummaryDto
{
    public DateTime SavedAt { get; set; }
}