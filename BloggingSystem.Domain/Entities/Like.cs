using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Events;
using BloggingSystem.Shared.DTOs;

namespace BloggingSystem.Domain.Entities;

public partial class Like: BaseEntity
{
    public long UserId { get; set; }

    public string EntityType { get; set; } = null!;

    public long EntityId { get; set; }

    [NotMapped]
    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
    
    private Like() {}
    
    public Like(long userId, string entityType, long entityId)
    {
        UserId = userId;
        EntityType = entityType;
        EntityId = entityId;
    }
    
    public static Like Create(long userId, string entityType, long entityId)
    {
        var likeObject = new Like(userId, entityType, entityId);

        likeObject.AddDomainEvent(new EntityLikedEvent(userId, entityType, entityId));

        return likeObject;
    }

    public void UnLiked(long userId, string entityType, long entityId) {
        AddDomainEvent(new EntityUnlikedEvent(userId, entityType, entityId));
    }
}

public enum LikeEntityType
{
    Post,
    Comment
}
