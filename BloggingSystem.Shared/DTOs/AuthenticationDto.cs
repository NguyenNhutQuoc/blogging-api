namespace BloggingSystem.Shared.DTOs;

public class AuthenticationResult
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public int ExpiresIn { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public UserDto User { get; set; } = null!;
}

public class SocialAuthDto
{
    public string Provider { get; set; } = null!;
}

