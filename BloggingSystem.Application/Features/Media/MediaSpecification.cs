using BloggingSystem.Application.Commons.Specifications;
using BloggingSystem.Domain.Entities;

namespace BloggingSystem.Application.Features.Media
{
    public class GetMediaSpecification : BaseSpecification<Medium>
    {
        public GetMediaSpecification()
            : base(m => true)
        {
            AddInclude(m => m.User);
            ApplyOrderByDescending(m => m.CreatedAt);
        }

        public GetMediaSpecification(int pageIndex, int pageSize)
            : base(m => true)
        {
            AddInclude(m => m.User);
            ApplyOrderByDescending(m => m.CreatedAt);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }

    public class GetMediaByUserIdSpecification : BaseSpecification<Medium>
    {
        public GetMediaByUserIdSpecification(long userId)
            : base(m => m.UserId == userId)
        {
            AddInclude(m => m.User);
            ApplyOrderByDescending(m => m.CreatedAt);
        }

        public GetMediaByUserIdSpecification(long userId, int pageIndex, int pageSize)
            : base(m => m.UserId == userId)
        {
            AddInclude(m => m.User);
            ApplyOrderByDescending(m => m.CreatedAt);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }

    public class GetMediaByTypeSpecification : BaseSpecification<Medium>
    {
        public GetMediaByTypeSpecification(string fileType)
            : base(m => m.FileType == fileType)
        {
            AddInclude(m => m.User);
            ApplyOrderByDescending(m => m.CreatedAt);
        }

        public GetMediaByTypeSpecification(string fileType, int pageIndex, int pageSize)
            : base(m => m.FileType == fileType)
        {
            AddInclude(m => m.User);
            ApplyOrderByDescending(m => m.CreatedAt);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }
}