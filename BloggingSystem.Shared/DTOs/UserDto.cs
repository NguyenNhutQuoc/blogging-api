using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BloggingSystem.Shared.DTOs;

namespace BloggingSystem.Shared.DTOs
{
    /// <summary>
    /// Data transfer object for User
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Whether the user is active
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime? CreatedAt { get; set; }
        
        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// User bio
        /// </summary>
        public string Bio { get; set; }
        
        /// <summary>
        /// Website URL
        /// </summary>
        public string Website { get; set; }
        
        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; }
        
        /// <summary>
        /// Avatar URL
        /// </summary>
        public string AvatarUrl { get; set; }
        
        /// <summary>
        /// User roles
        /// </summary>
        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
        public List<SocialAuthDto> SocialAuths { get; set; } = new List<SocialAuthDto>();
    }
    
    /// <summary>
    /// Data transfer object for UserProfile
    /// </summary>
    public class UserProfileDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        public long UserId { get; set; }
        
        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// User bio
        /// </summary>
        public string Bio { get; set; }
        
        /// <summary>
        /// Website URL
        /// </summary>
        public string Website { get; set; }
        
        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; }
        
        /// <summary>
        /// Avatar URL
        /// </summary>
        public string AvatarUrl { get; set; }
        
        /// <summary>
        /// Social links (JSON)
        /// </summary>
        public string SocialLinks { get; set; }
    }
    
    /// <summary>
    /// Data transfer object for User login
    /// </summary>
    public class UserLoginDto
    {
        /// <summary>
        /// Username or email
        /// </summary>
        public string UsernameOrEmail { get; set; }
        
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }
    
    /// <summary>
    /// Data transfer object for User token
    /// </summary>
    public class UserTokenDto
    {
        /// <summary>
        /// Access token
        /// </summary>
        public string AccessToken { get; set; }
        
        /// <summary>
        /// Refresh token
        /// </summary>
        public string RefreshToken { get; set; }
        
        /// <summary>
        /// Token type
        /// </summary>
        public string TokenType { get; set; } = "Bearer";
        
        /// <summary>
        /// Expiration time in seconds
        /// </summary>
        public int ExpiresIn { get; set; }
        
        /// <summary>
        /// User data
        /// </summary>
        public UserDto User { get; set; }
    }
}

public class AuthorDto : UserDto
{
    [JsonIgnore]
    public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
    [JsonIgnore]
    public List<SocialAuthDto> SocialAuths { get; set; } = new List<SocialAuthDto>();
}