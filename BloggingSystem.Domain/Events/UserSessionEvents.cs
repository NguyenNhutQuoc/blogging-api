using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Events;

public class UserSessionCreatedEvent : DomainEvent
{
    public long SessionId { get; }
    public long UserId { get; }
    public string SessionToken { get; }
    public string IpAddress { get; }
    public string UserAgent { get; }
    public DateTime ExpiresAt { get; }
    
    public UserSessionCreatedEvent(long sessionId, long userId, string sessionToken, string ipAddress, string userAgent, DateTime expiresAt)
    {
        SessionId = sessionId;
        UserId = userId;
        SessionToken = sessionToken;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        ExpiresAt = expiresAt;
    }
}

public class UserSessionUpdatedEvent : DomainEvent
{
    public long SessionId { get; }
    public long UserId { get; }
    public string SessionToken { get; }
    public string IpAddress { get; }
    public string UserAgent { get; }
    public DateTime ExpiresAt { get; }
    
    public UserSessionUpdatedEvent(long sessionId, long userId, string sessionToken, string ipAddress, string userAgent, DateTime expiresAt)
    {
        SessionId = sessionId;
        UserId = userId;
        SessionToken = sessionToken;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        ExpiresAt = expiresAt;
    }
}

public class UserSessionDeletedEvent : DomainEvent
{
    public long SessionId { get; }
    public long UserId { get; }
    
    public UserSessionDeletedEvent(long sessionId, long userId)
    {
        SessionId = sessionId;
        UserId = userId;
    }
}