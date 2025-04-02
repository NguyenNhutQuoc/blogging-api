using BloggingSystem.Application.Commons.Specifications;

namespace BloggingSystem.Application.Features.SocialAuth;

public class SocialAuthByProviderAndProviderUserIdSpecification: BaseSpecification<Domain.Entities.SocialAuth>
{
    public SocialAuthByProviderAndProviderUserIdSpecification(string provider, string providerUserId) 
        : base(sa => sa.Provider == provider && sa.ProviderUserId == providerUserId)
    {
        // Include user
        AddInclude(sa => sa.User);
    }
}

public class SocialAuthByUserIdSpecification: BaseSpecification<Domain.Entities.SocialAuth>
{
    public SocialAuthByUserIdSpecification(long userId) 
        : base(sa => sa.UserId == userId)
    {
        // Include user
        AddInclude(sa => sa.User);
    }
}