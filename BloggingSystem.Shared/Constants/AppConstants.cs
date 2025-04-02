namespace BloggingSystem.Shared.Constants;

public static class AppConstants
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 50;
    public const string DefaultSortColumn = "CreatedAt";
    public const string DefaultSortDirection = "desc";
    
    public static class CacheKeys
    {
        public const string CategoryList = "CategoryList";
        public const string PopularTags = "PopularTags";
        public const string FeaturedPosts = "FeaturedPosts";
        
        public static string PostDetail(long id) => $"Post_{id}";
        public static string UserProfile(long id) => $"User_{id}";
    }
    
    public static class RegexPatterns
    {
        public const string Slug = "^[a-z0-9]+(?:-[a-z0-9]+)*$";
        public const string Username = "^[a-zA-Z0-9_-]{3,20}$";
        public const string StrongPassword = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$";
        public const string Email = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        public const string PhoneNumber = @"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$";
        public const string WebsiteUrl = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";
        public const string Markdown = @"^(#{1,6})([^\n]+)\n?((=|[\-]{3,})\s*[^\n]+\n?)*$";
        public const string Latitude = @"^[-+]?([1-8]?\d(\.\d+)?|90\.0)$";
        public const string Longitude = @"^[-+]?(180(\.0+)?|((1[0-7]\d)|(\d{1,2}))(\.\d+)?)$";
        public const string IpAddress = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
        public const string Hashtag = @"#[a-zA-Z0-9_-]+";
    }
}