using BloggingSystem.Application.Commons.Specifications;
using BloggingSystem.Domain.Entities;

namespace BloggingSystem.Application.Features.Tag
{
    public class TagSpecification : BaseSpecification<Domain.Entities.Tag>
    {
        public TagSpecification()
        {
            ApplyOrderBy(t => t.Name);
            DisableTracking();
        }

        public TagSpecification(int pageNumber, int pageSize)
            : this()
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }

    public class TagByIdSpecification : BaseSpecification<Domain.Entities.Tag>
    {
        public TagByIdSpecification(long id)
            : base(t => t.Id == id)
        {
            AddInclude(t => t.PostTags);
            AddInclude("PostTags.Post");
            ApplyOrderBy(t => t.Name);
        }
    }
} 