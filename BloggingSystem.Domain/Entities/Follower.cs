using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Events;

namespace BloggingSystem.Domain.Entities;

public partial class Follower: BaseEntity
{
    public long FollowerId { get; set; }

    public long FollowingId { get; set; }

    [NotMapped]
    public DateTime? UpdatedAt { get; set; }

    public virtual User FollowerNavigation { get; set; } = null!;

    public virtual User Following { get; set; } = null!;
    
    private Follower() {}
    
    public Follower(long followerId, long followingId)
    {
        FollowerId = followerId;
        FollowingId = followingId;
    }
    
    public Follower(long id, long followerId, long followingId)
    {
        Id = id;
        FollowerId = followerId;
        FollowingId = followingId;
    }
    
    public static Follower Create(long followerId, long followingId)
    {
        var follower = new Follower(followerId, followingId);
        // Add domain event
        follower.AddDomainEvent(new UserFollowedEvent(
            followerId,
            followingId));

        return follower;
    }

    public void UnFollow(long followerId, long followingId) {   
        // Add domain event before deleting
        AddDomainEvent(new UserUnfollowedEvent(
            followerId,
            followingId));
    }
}
