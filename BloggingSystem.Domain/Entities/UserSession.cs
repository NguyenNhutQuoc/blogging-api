using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Events;

namespace BloggingSystem.Domain.Entities;

public partial class UserSession: BaseEntity
{
    [NotMapped]
    public DateTime? UpdatedAt { get; protected set; }
    public long UserId { get; set; }

    public string SessionToken { get; set; } = null!;

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime ExpiresAt { get; set; }

    public virtual User User { get; set; } = null!;
    
    private UserSession() {}
    
    public UserSession(long userId, string sessionToken, string ipAddress, string userAgent, DateTime expiresAt)
    {
        UserId = userId;
        SessionToken = sessionToken;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        ExpiresAt = expiresAt;
    }
    
    public static UserSession Create(long userId, string sessionToken, string ipAddress, string userAgent, DateTime expiresAt)
    {
        var session = new UserSession
        {
            UserId = userId,
            SessionToken = sessionToken,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            ExpiresAt = expiresAt
        };
        
        session.AddDomainEvent(new UserSessionCreatedEvent(session.Id, userId, sessionToken, ipAddress, userAgent, expiresAt));
        
        return session;
    }
    
    public void Update(string sessionToken, string ipAddress, string userAgent, DateTime expiresAt)
    {
        SessionToken = sessionToken;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        ExpiresAt = expiresAt;
        
        SetModified();
        
        AddDomainEvent(new UserSessionUpdatedEvent(Id, UserId, sessionToken, ipAddress, userAgent, expiresAt));
    }
    
    public void Delete()
    {
        SetDeleted();
        
        AddDomainEvent(new UserSessionDeletedEvent(Id, UserId));
    }
}
