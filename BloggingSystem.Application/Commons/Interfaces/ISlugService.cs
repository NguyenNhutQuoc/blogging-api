namespace BloggingSystem.Application.Commons.Interfaces
{
    /// <summary>
    /// Interface for slug generation service
    /// </summary>
    public interface ISlugService
    {
        /// <summary>
        /// Generate a URL-friendly slug from a string
        /// </summary>
        /// <param name="input">Input string to slugify</param>
        /// <returns>URL-friendly slug</returns>
        string GenerateSlug(string input);
    }
}