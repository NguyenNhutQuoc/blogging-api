using System;
using System.Collections.Generic;
using System.Text.Json;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Events;
using BloggingSystem.Domain.Exceptions;

namespace BloggingSystem.Domain.Entities;

public partial class UserProfile : BaseEntity
{
    // Thuộc tính private để encapsulation 
    private string? _displayName;
    private string? _bio;
    private string? _avatarUrl;
    private string? _website;
    private string? _socialLinks;
    private string? _location;
    
    // Thuộc tính public với getter/setter được kiểm soát
    public long UserId { get; private set; }

    public string? DisplayName 
    { 
        get => _displayName; 
        private set 
        {
            if (!string.IsNullOrEmpty(value) && value.Length > 100)
                throw new DomainException("Display name cannot exceed 100 characters");
            _displayName = value; 
        }
    }

    public string? Bio 
    { 
        get => _bio; 
        private set 
        {
            if (!string.IsNullOrEmpty(value) && value.Length > 500)
                throw new DomainException("Bio cannot exceed 500 characters");
            _bio = value; 
        }
    }

    public string? AvatarUrl 
    { 
        get => _avatarUrl; 
        private set 
        {
            if (!string.IsNullOrEmpty(value) && !IsValidUrl(value))
                throw new DomainException("Invalid avatar URL format");
            _avatarUrl = value; 
        }
    }

    public string? Website 
    { 
        get => _website; 
        private set 
        {
            if (!string.IsNullOrEmpty(value) && !IsValidUrl(value))
                throw new DomainException("Invalid website URL format");
            _website = value; 
        }
    }

    public string? SocialLinks 
    { 
        get => _socialLinks; 
        private set 
        {
            if (!string.IsNullOrEmpty(value) && !IsValidJson(value))
                throw new DomainException("Social links must be in valid JSON format");
            _socialLinks = value; 
        }
    }

    public string? Location 
    { 
        get => _location; 
        private set 
        {
            if (!string.IsNullOrEmpty(value) && value.Length > 100)
                throw new DomainException("Location cannot exceed 100 characters");
            _location = value; 
        }
    }

    public virtual User User { get; private set; } = null!;
    
    // Constructors
    private UserProfile() {}
    
    private UserProfile(long userId, string displayName)
    {
        UserId = userId;
        DisplayName = displayName;
        ValidateState();
    }
    
    // Factory method
    public static UserProfile Create(long userId, string displayName)
    {
        var profile = new UserProfile(userId, displayName);
        profile.AddDomainEvent(new UserProfileCreatedEvent(profile.Id, userId));
        return profile;
    }
    
    // Domain methods
    public void UpdateBasicInfo(string displayName, string bio, string location)
    {
        DisplayName = displayName;
        Bio = bio;
        Location = location;
        SetModified();
        
        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId));
    }
    
    public void UpdateAvatarUrl(string avatarUrl)
    {
        AvatarUrl = avatarUrl;
        SetModified();
        
        AddDomainEvent(new UserAvatarUpdatedEvent(Id, UserId));
    }
    
    public void UpdateWebsite(string website)
    {
        Website = website;
        SetModified();
        
        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId));
    }
    
    public void UpdateSocialLinks(Dictionary<string, string> socialLinks)
    {
        // Convert Dictionary to JSON
        SocialLinks = JsonSerializer.Serialize(socialLinks);
        SetModified();
        
        AddDomainEvent(new UserProfileUpdatedEvent(Id, UserId));
    }
    
    public Dictionary<string, string> GetSocialLinksAsDictionary()
    {
        if (string.IsNullOrEmpty(SocialLinks))
            return new Dictionary<string, string>();
            
        return JsonSerializer.Deserialize<Dictionary<string, string>>(SocialLinks) 
               ?? new Dictionary<string, string>();
    }
    
    public bool HasSocialNetwork(string networkName)
    {
        var links = GetSocialLinksAsDictionary();
        return links.ContainsKey(networkName) && !string.IsNullOrEmpty(links[networkName]);
    }
    
    // Helper methods
    private void ValidateState()
    {
        if (UserId <= 0)
            throw new DomainException("User ID must be specified");
            
        if (string.IsNullOrWhiteSpace(DisplayName))
            throw new DomainException("Display name cannot be empty");
    }
    
    private bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) 
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
    
    private bool IsValidJson(string json)
    {
        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}