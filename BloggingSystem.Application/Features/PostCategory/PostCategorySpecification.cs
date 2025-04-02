using BloggingSystem.Application.Commons.Specifications;

namespace BloggingSystem.Application.Features.PostCategory;

public class PostCategoryByPostIdSpecification: BaseSpecification<Domain.Entities.PostCategory>
{
    public PostCategoryByPostIdSpecification(long postId) 
        : base(pc => pc.PostId == postId)
    {
        // Include post
        AddInclude(pc => pc.Post);
        
        // Include category
        AddInclude(pc => pc.Category);
    }
}