using System;
using System.Collections.Generic;

namespace BloggingSystem.Shared.DTOs
{
    /// <summary>
    /// Data transfer object for User summary
    /// </summary>
    public class UserSummaryDto
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
        /// User avatar URL
        /// </summary>
        public string AvatarUrl { get; set; }
        
        /// <summary>
        /// The number of posts by this user
        /// </summary>
        public int PostCount { get; set; }
        
        /// <summary>
        /// Roles of the user
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();
    }
}