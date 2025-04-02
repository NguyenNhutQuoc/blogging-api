using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BloggingSystem.Application.Commons.Interfaces;

namespace BloggingSystem.Infrastructure.Services
{
    /// <summary>
    /// Implementation of slug generation service
    /// </summary>
    public class SlugService : ISlugService
    {
        /// <summary>
        /// Generate a URL-friendly slug from a string
        /// </summary>
        public string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Normalize the string (decompose Unicode characters)
            var normalizedString = input.Normalize(NormalizationForm.FormD);
            
            // Remove diacritics (accents)
            var stringBuilder = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            
            // Convert to lowercase
            var slug = stringBuilder.ToString().ToLowerInvariant();
            
            // Replace spaces with hyphens
            slug = Regex.Replace(slug, @"\s+", "-");
            
            // Remove special characters
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", string.Empty);
            
            // Remove consecutive hyphens
            slug = Regex.Replace(slug, @"-{2,}", "-");
            
            // Trim hyphens from the start and end
            slug = slug.Trim('-');
            
            // Ensure the slug isn't too long (most databases have column limits)
            var maxLength = 100;
            if (slug.Length > maxLength)
            {
                slug = slug.Substring(0, maxLength).TrimEnd('-');
            }
            
            return slug;
        }
    }
}