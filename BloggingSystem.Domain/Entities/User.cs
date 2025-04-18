using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Domain.Events;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.Constants;

namespace BloggingSystem.Domain.Entities;

public partial class User: BaseEntity
{
    // Các trường private để cho domain logic
    private string _username;
    private string _email;
    private string _passwordHash;
    private bool _isActive;
    
    // Collections - giữ nguyên từ scaffold nhưng chuyển sang private
    private readonly List<ApiToken> _apiTokens = new List<ApiToken>();
    private readonly List<Comment> _comments = new List<Comment>();
    private readonly List<Follower> _followerFollowerNavigations = new List<Follower>();
    private readonly List<Follower> _followerFollowings = new List<Follower>();
    private readonly List<Like> _likes = new List<Like>();
    private readonly List<Medium> _media = new List<Medium>();
    private readonly List<Newsletter> _newsletters = new List<Newsletter>();
    private readonly List<Notification> _notificationSenders = new List<Notification>();
    private readonly List<Notification> _notificationUsers = new List<Notification>();
    private readonly List<PollVote> _pollVotes = new List<PollVote>();
    private readonly List<Poll> _polls = new List<Poll>();
    private readonly List<Post> _posts = new List<Post>();
    private readonly List<Revision> _revisions = new List<Revision>();
    private readonly List<SavedPost> _savedPosts = new List<SavedPost>();
    private readonly List<Series> _series = new List<Series>();
    private readonly List<SocialAuth> _socialAuths = new List<SocialAuth>();
    private readonly List<UserPermission> _userPermissions = new List<UserPermission>();
    private readonly List<UserRole> _userRoles = new List<UserRole>();
    private readonly List<UserSession> _userSessions = new List<UserSession>();

    public string Username 
    { 
        get => _username; 
        private set 
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Username cannot be empty");
            _username = value;
        }
    }

    public string Email 
    { 
        get => _email; 
        private set 
        {
            if (string.IsNullOrWhiteSpace(value) || !IsValidEmail(value))
                throw new DomainException("Invalid email format");
            _email = value;
        }
    }
        
    public string PasswordHash 
    { 
        get => _passwordHash; 
        private set => _passwordHash = value; 
    }
        
    public bool IsActive 
    { 
        get => _isActive; 
        private set => _isActive = value; 
    }

    public virtual ICollection<ApiToken> ApiTokens { get; set; } = new List<ApiToken>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Follower> FollowerFollowerNavigations { get; set; } = new List<Follower>();

    public virtual ICollection<Follower> FollowerFollowings { get; set; } = new List<Follower>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Medium> Media { get; set; } = new List<Medium>();

    public virtual ICollection<Newsletter> Newsletters { get; set; } = new List<Newsletter>();

    public virtual ICollection<Notification> NotificationSenders { get; set; } = new List<Notification>();

    public virtual ICollection<Notification> NotificationUsers { get; set; } = new List<Notification>();

    public virtual ICollection<PollVote> PollVotes { get; set; } = new List<PollVote>();

    public virtual ICollection<Poll> Polls { get; set; } = new List<Poll>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Revision> Revisions { get; set; } = new List<Revision>();

    public virtual ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();

    public virtual ICollection<Series> Series { get; set; } = new List<Series>();

    public virtual ICollection<SocialAuth> SocialAuths { get; set; } = new List<SocialAuth>();

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

    public virtual UserProfile? UserProfile { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
    
    private User() {}
    
    // Constructor cho domain logic với tham số bắt buộc
    private User(string username, string email, string passwordHash)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = false; // Mặc định chưa active
        
        ValidateState();
    }
    
    // Factory method cho việc tạo User mới
    public static User Create(string username, string email, string passwordHash)
    {
        var user = new User(username, email, passwordHash);
        
        // Thêm domain event
        user.AddDomainEvent(new UserCreatedEvent(user.Id));
        
        return user;
    }
    
    // Domain methods
    public void Activate()
    {
        if (IsActive)
            throw new DomainException("User is already active");
            
        IsActive = true;
        SetModified();
        
        AddDomainEvent(new UserActivatedEvent(Id));
    }
    
    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("User is already inactive");
            
        IsActive = false;
        SetModified();
        
        AddDomainEvent(new UserDeactivatedEvent(Id));
    }
    
    public void ChangeEmail(string newEmail)
    {
        if (Email == newEmail)
            throw new DomainException("New email is the same as current email");
            
        Email = newEmail;
        SetModified();
        
        AddDomainEvent(new UserEmailChangedEvent(Id, newEmail));
    }
    
    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainException("Password hash cannot be empty");
            
        PasswordHash = newPasswordHash;
        SetModified();
        
        AddDomainEvent(new UserPasswordChangedEvent(Id));
    }
    
    // Helper methods
    private void ValidateState()
    {
        if (string.IsNullOrWhiteSpace(Username))
            throw new DomainException("Username cannot be empty");
            
        if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
            throw new DomainException("Invalid email format");
        
        if (Regex.IsMatch(Username, AppConstants.RegexPatterns.Username))
            throw new DomainException("Invalid username format");
    }
    
    private bool IsValidEmail(string email)
    {
        try {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch {
            return false;
        }
    }
    
    public void AssignRole(long userId, long roleId ) {
        AddDomainEvent(new UserRoleAssignedEvent(userId, roleId));
    }

    public void UnAssignRole(long userId, long roleId) {
        AddDomainEvent(new UserRoleRemovedEvent(userId, roleId));
    }
}
