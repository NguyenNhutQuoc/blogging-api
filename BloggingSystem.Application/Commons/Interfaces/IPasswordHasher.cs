namespace BloggingSystem.Application.Commons.Interfaces
{
    /// <summary>
    /// Interface for password hashing service
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hash a password
        /// </summary>
        /// <param name="password">Password to hash</param>
        /// <returns>Hashed password</returns>
        string HashPassword(string password);
        
        /// <summary>
        /// Verify a password against a hash
        /// </summary>
        /// <param name="passwordHash">Stored password hash</param>
        /// <param name="providedPassword">Password to verify</param>
        /// <returns>True if password matches, false otherwise</returns>
        bool VerifyPassword(string passwordHash, string providedPassword);
    }
}