using BloggingSystem.Application.Commons.Specifications;
using BloggingSystem.Domain.Entities;

namespace BloggingSystem.Application.Features.Category
{
    public class CategorySpecification : BaseSpecification<Domain.Entities.Category>
    {
        public CategorySpecification()
        {
            ApplyOrderBy(c => c.Name);
            DisableTracking();
        }

        public CategorySpecification(int pageNumber, int pageSize)
            : this()
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }

    public class CategoryByIdSpecification : BaseSpecification<Domain.Entities.Category>
    {
        public CategoryByIdSpecification(long id)
            : base(c => c.Id == id)
        {
            AddInclude(c => c.PostCategories);
            AddInclude("PostCategories.Post");
            ApplyOrderBy(c => c.Name);
        }
    }
}