using BloggingSystem.Application.Commons.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BloggingSystem.Infrastructure.Services
{
    /// <summary>
    /// Implementation of IPasswordHasher using ASP.NET Core Identity's password hasher
    /// </summary>
    public class IdentityPasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> _identityPasswordHasher;
        
        public IdentityPasswordHasher()
        {
            _identityPasswordHasher = new PasswordHasher<object>();
        }
        
        /// <summary>
        /// Hash a password using Identity's password hasher
        /// </summary>
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }
            
            // Using a dummy user object (not needed for the hash algorithm)
            return _identityPasswordHasher.HashPassword(new object(), password);
        }
        
        /// <summary>
        /// Verify a password against a hash using Identity's password hasher
        /// </summary>
        public bool VerifyPassword(string passwordHash, string providedPassword)
        {
            if (string.IsNullOrEmpty(passwordHash))
            {
                throw new ArgumentNullException(nameof(passwordHash));
            }
            
            if (string.IsNullOrEmpty(providedPassword))
            {
                return false;
            }
            
            try
            {
                // Using a dummy user object (not needed for the verification)
                var result = _identityPasswordHasher.VerifyHashedPassword(new object(), passwordHash, providedPassword);
                return result == PasswordVerificationResult.Success;
            }
            catch
            {
                // If the password hash is in an invalid format, return false
                return false;
            }
        }
    }
}