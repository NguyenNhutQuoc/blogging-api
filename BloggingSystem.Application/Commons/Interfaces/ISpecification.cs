using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BloggingSystem.Application.Commons.Interfaces
{
    /// <summary>
    /// Specification pattern interface
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    public interface ISpecification<T>
    {
        /// <summary>
        /// Criteria expression for filtering
        /// </summary>
        Expression<Func<T, bool>> Criteria { get; }
        
        /// <summary>
        /// Include expressions for eager loading
        /// </summary>
        List<Expression<Func<T, object>>> Includes { get; }
        
        /// <summary>
        /// String-based include statements
        /// </summary>
        List<string> IncludeStrings { get; }
        
        /// <summary>
        /// Order by expression
        /// </summary>
        Expression<Func<T, object>> OrderBy { get; }
        
        /// <summary>
        /// Order by descending expression
        /// </summary>
        Expression<Func<T, object>> OrderByDescending { get; }
        
        /// <summary>
        /// Additional order by expressions
        /// </summary>
        List<(Expression<Func<T, object>> KeySelector, bool Descending)> ThenByExpressions { get; }
        
        /// <summary>
        /// Number of records to skip
        /// </summary>
        int Skip { get; }
        
        /// <summary>
        /// Number of records to take
        /// </summary>
        int Take { get; }
        
        /// <summary>
        /// Whether pagination is enabled
        /// </summary>
        bool IsPagingEnabled { get; }
        
        /// <summary>
        /// Whether tracking is enabled
        /// </summary>
        bool AsNoTracking { get; }
    }
}