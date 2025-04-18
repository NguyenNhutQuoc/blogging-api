using System.Collections.Generic;
using System.Security.Claims;

namespace BloggingSystem.Application.Commons.Interfaces
{
    /// <summary>
    /// Service to access current authenticated user information
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Get current user ID
        /// </summary>
        long? UserId { get; }

        /// <summary>
        /// Check if user is authenticated
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Get user's username
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Get user's email
        /// </summary>
        string Email { get; }

        /// <summary>
        /// Get all user claims
        /// </summary>
        IEnumerable<Claim> Claims { get; }

        /// <summary>
        /// Check if user has a specific permission
        /// </summary>
        bool HasPermission(string permission);

        /// <summary>
        /// Check if user is in a specific role
        /// </summary>
        bool IsInRole(string role);
    }
}