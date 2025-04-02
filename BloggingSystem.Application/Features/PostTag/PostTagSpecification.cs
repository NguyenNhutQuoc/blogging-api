using BloggingSystem.Application.Commons.Specifications;

namespace BloggingSystem.Application.Features.PostTag;

public class PostTagByPostIdSpecification: BaseSpecification<Domain.Entities.PostTag>
{
    public PostTagByPostIdSpecification(long postId) 
        : base(pt => pt.PostId == postId)
    {
        // Include post
        AddInclude(pt => pt.Post);
        
        // Include tag
        AddInclude(pt => pt.Tag);
    }
}