using BloggingSystem.Domain.Entities;

namespace BloggingSystem.Application.Commons.Interfaces;

public interface IJwtGenerator
{
    (string accessToken, string refreshToken) GenerateTokens(
        User user, 
        IEnumerable<Role> roles, 
        IEnumerable<Permission> permissions);
}