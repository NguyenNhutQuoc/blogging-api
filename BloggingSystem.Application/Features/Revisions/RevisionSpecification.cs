using System;
using System.Linq;
using BloggingSystem.Application.Commons.Specifications;
using BloggingSystem.Domain.Entities;

namespace BloggingSystem.Application.Features.Revisions
{
    public class RevisionsByPostSpecification : BaseSpecification<Revision>
    {
        public RevisionsByPostSpecification(long postId)
            : base(r => r.PostId == postId)
        {
            AddInclude(r => r.User);
            AddInclude(r => r.Post);
            ApplyOrderByDescending(r => r.RevisionNumber);
        }

        public RevisionsByPostSpecification(long postId, int pageIndex, int pageSize)
            : base(r => r.PostId == postId)
        {
            AddInclude(r => r.User);
            AddInclude(r => r.Post);
            ApplyOrderByDescending(r => r.RevisionNumber);
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }

    public class RevisionByIdSpecification : BaseSpecification<Revision>
    {
        public RevisionByIdSpecification(long id)
            : base(r => r.Id == id)
        {
            AddInclude(r => r.User);
            AddInclude(r => r.Post);
        }
    }

    public class RevisionByIdWithDetailsSpecification : BaseSpecification<Revision>
    {
        public RevisionByIdWithDetailsSpecification(long id)
            : base(r => r.Id == id)
        {
            AddInclude(r => r.User);
            AddInclude(r => r.Post);
        }
    }

    public class LatestRevisionByPostSpecification : BaseSpecification<Revision>
    {
        public LatestRevisionByPostSpecification(long postId)
            : base(r => r.PostId == postId)
        {
            AddInclude(r => r.User);
            AddInclude(r => r.Post);
            ApplyOrderByDescending(r => r.RevisionNumber);
            ApplyPaging(0, 1);
        }
    }

    public class MaxRevisionNumberByPostSpecification : BaseSpecification<Revision>
    {
        public MaxRevisionNumberByPostSpecification(long postId)
            : base(r => r.PostId == postId)
        {
            ApplyOrderByDescending(r => r.RevisionNumber);
            ApplyPaging(0, 1);
        }
    }
}