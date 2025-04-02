using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Commons;
using Microsoft.EntityFrameworkCore;

namespace BloggingSystem.Infrastructure.Data
{
    /// <summary>
    /// Specification evaluator for applying specifications to IQueryable
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    public static class SpecificationEvaluator<T> where T : BaseEntity
    {
        /// <summary>
        /// Apply specification to query
        /// </summary>
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

            // Apply criteria
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Apply order by
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            
            // Apply order by descending
            if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }
            
            // Apply additional ordering
            if (specification.ThenByExpressions != null)
            {
                foreach (var thenBy in specification.ThenByExpressions)
                {
                    if (thenBy.Descending)
                    {
                        query = ((IOrderedQueryable<T>)query).ThenByDescending(thenBy.KeySelector);
                    }
                    else
                    {
                        query = ((IOrderedQueryable<T>)query).ThenBy(thenBy.KeySelector);
                    }
                }
            }

            // Apply paging
            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }

            // Apply includes
            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
            
            // Apply string includes
            query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            // Apply tracking
            if (specification.AsNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }
    }
}