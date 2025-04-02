namespace BloggingSystem.Infrastructure.Identity
{
    public class JwtSettings
    {
        public string Secret { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int TokenExpiryInMinutes { get; set; }
        public int RefreshTokenExpiryInDays { get; set; }
    }

    public class SocialAuthSettings
    {
        public GoogleSettings Google { get; set; } = new GoogleSettings();
        public FacebookSettings Facebook { get; set; } = new FacebookSettings();
        public GitHubSettings GitHub { get; set; } = new GitHubSettings();
        
        public class GoogleSettings
        {
            public string ClientId { get; set; } = null!;
            public string ClientSecret { get; set; } = null!;
            public string RedirectUri { get; set; } = null!;
        }
        
        public class FacebookSettings
        {
            public string AppId { get; set; } = null!;
            public string AppSecret { get; set; } = null!;
            public string RedirectUri { get; set; } = null!;
        }
        
        public class GitHubSettings
        {
            public string ClientId { get; set; } = null!;
            public string ClientSecret { get; set; } = null!;
            public string RedirectUri { get; set; } = null!;
        }
    }
}