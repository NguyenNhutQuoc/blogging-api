using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Events;

namespace BloggingSystem.Domain.Entities;

public partial class Revision: BaseEntity
{
    public long PostId { get; set; }

    public long UserId { get; set; }

    public string Content { get; set; } = null!;

    public int RevisionNumber { get; set; }

    [NotMapped]
    public DateTime? UpdatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    
    private Revision() {}
    
    public Revision(long postId, long userId, string content, int revisionNumber)
    {
        PostId = postId;
        UserId = userId;
        Content = content;
        RevisionNumber = revisionNumber;
    }
    
    public static Revision Create(long postId, long userId, string content, int revisionNumber)
    {
        var revision = new Revision(postId, userId, content, revisionNumber);

        revision.AddDomainEvent(new CreatedNewRevisionEvent(postId, userId, revisionNumber, "", content));

        return revision;
    }
}
