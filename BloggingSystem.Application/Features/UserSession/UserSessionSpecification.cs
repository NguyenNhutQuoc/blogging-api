using BloggingSystem.Application.Commons.Specifications;

namespace BloggingSystem.Application.Features.UserSession;

public class UserSessionByTokenSpecification : BaseSpecification<Domain.Entities.UserSession>
{
    public UserSessionByTokenSpecification(string token) 
        : base(us => us.SessionToken == token)
    {
        // Include user
        AddInclude(us => us.User);
    }
}