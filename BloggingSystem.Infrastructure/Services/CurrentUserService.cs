using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BloggingSystem.Application.Commons.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BloggingSystem.Infrastructure.Services
{
    /// <summary>
    /// Service to access current authenticated user information
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get current user ID
        /// </summary>
        public long? UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) 
                          ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");
                
                return !string.IsNullOrEmpty(userId) && long.TryParse(userId, out var id) 
                       ? id : null;
            }
        }

        /// <summary>
        /// Check if user is authenticated
        /// </summary>
        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        /// <summary>
        /// Get user's username
        /// </summary>
        public string Username => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

        /// <summary>
        /// Get user's email
        /// </summary>
        public string Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        /// <summary>
        /// Get all user claims
        /// </summary>
        public IEnumerable<Claim> Claims => _httpContextAccessor.HttpContext?.User?.Claims ?? Array.Empty<Claim>();

        /// <summary>
        /// Check if user has a specific permission
        /// </summary>
        public bool HasPermission(string permission)
        {
            return _httpContextAccessor.HttpContext?.User?.Claims
                .Any(c => c.Type == "permission" && c.Value == permission) ?? false;
        }

        /// <summary>
        /// Check if user is in a specific role
        /// </summary>
        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }
    }
}