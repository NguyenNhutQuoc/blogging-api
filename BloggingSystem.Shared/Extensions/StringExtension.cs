using System.Text.RegularExpressions;

namespace BloggingSystem.Shared.Extensions;

public static class StringExtensions
{
    public static string ToSlug(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
            
        // Convert to lowercase
        var slug = input.ToLowerInvariant();
        
        // Replace spaces with hyphens
        slug = Regex.Replace(slug, @"\s+", "-");
        
        // Remove invalid characters
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
        
        // Remove duplicate hyphens
        slug = Regex.Replace(slug, @"-{2,}", "-");
        
        // Trim hyphens from start and end
        slug = slug.Trim('-');
        
        return slug;
    }
    
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;
            
        try
        {
            // Use MailAddress to validate email
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value))
            return value;
            
        return value.Length <= maxLength ? 
            value : 
            value.Substring(0, maxLength - suffix.Length) + suffix;
    }
}