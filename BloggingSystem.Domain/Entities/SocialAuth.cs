using System;
using System.Collections.Generic;
using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Domain.Entities;

public partial class SocialAuth: BaseEntity
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string Provider { get; set; } = null!;

    public string ProviderUserId { get; set; } = null!;

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
    
    private SocialAuth() {}
    
    public SocialAuth(long userId, string provider, string providerUserId, string? accessToken, string? refreshToken)
    {
        UserId = userId;
        Provider = provider;
        ProviderUserId = providerUserId;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
    
    public static SocialAuth Create(long userId, string provider, string providerUserId, string? accessToken, string? refreshToken)
    {
        var socialAuth = new SocialAuth
        {
            UserId = userId,
            Provider = provider,
            ProviderUserId = providerUserId,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        
        return socialAuth;
    }
    
    public void Update(string? accessToken, string? refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        
        SetModified();
    }
}
