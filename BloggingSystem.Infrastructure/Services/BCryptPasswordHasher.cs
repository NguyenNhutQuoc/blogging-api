using System;
using BloggingSystem.Application.Commons.Interfaces;
using Microsoft.Extensions.Options;

namespace BloggingSystem.Infrastructure.Services
{
    /// <summary>
    /// Implementation of IPasswordHasher using BCrypt
    /// </summary>
    public class BCryptPasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasherOptions _options;
        
        public BCryptPasswordHasher(IOptions<PasswordHasherOptions> options = null)
        {
            _options = options?.Value ?? new PasswordHasherOptions();
        }
        
        /// <summary>
        /// Hash a password using BCrypt
        /// </summary>
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }
            
            string salt = BCrypt.Net.BCrypt.GenerateSalt(_options.WorkFactor);
            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }
        
        /// <summary>
        /// Verify a password against a hash using BCrypt
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
                return BCrypt.Net.BCrypt.Verify(providedPassword, passwordHash);
            }
            catch
            {
                // If the password hash is in an invalid format, return false
                return false;
            }
        }
    }
    
    /// <summary>
    /// Options for password hasher
    /// </summary>
    public class PasswordHasherOptions
    {
        /// <summary>
        /// Work factor for BCrypt (default is 12)
        /// Higher values make hashing more secure but slower
        /// </summary>
        public int WorkFactor { get; set; } = 12;
    }
}