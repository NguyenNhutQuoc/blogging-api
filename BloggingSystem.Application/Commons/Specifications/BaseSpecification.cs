using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BloggingSystem.Application.Commons.Interfaces;

namespace BloggingSystem.Application.Commons.Specifications
{
    /// <summary>
    /// Base implementation of specification pattern
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        protected BaseSpecification()
        {
        }
        
        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        
        public Expression<Func<T, bool>> Criteria { get; private set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public List<(Expression<Func<T, object>> KeySelector, bool Descending)> ThenByExpressions { get; } = new List<(Expression<Func<T, object>>, bool)>();
        public int Skip { get; private set; }
        public int Take { get; private set; }
        public bool IsPagingEnabled { get; private set; }
        public bool AsNoTracking { get; private set; } = false;
        
        /// <summary>
        /// Set criteria for specification
        /// </summary>
        protected void ApplyCriteria(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        
        /// <summary>
        /// Add include expression
        /// </summary>
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        
        /// <summary>
        /// Add string-based include
        /// </summary>
        protected void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }
        
        protected void ThenInclude(Expression<Func<T, object>> includeExpression, bool descending)
        {
            ThenByExpressions.Add((includeExpression, descending));
        }
        
        /// <summary>
        /// Apply ordering
        /// </summary>
        protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }
        
        /// <summary>
        /// Apply descending ordering
        /// </summary>
        protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }
        
        /// <summary>
        /// Add additional ordering
        /// </summary>
        protected void ApplyThenBy(Expression<Func<T, object>> thenByExpression)
        {
            ThenByExpressions.Add((thenByExpression, false));
        }
        
        /// <summary>
        /// Add additional descending ordering
        /// </summary>
        protected void ApplyThenByDescending(Expression<Func<T, object>> thenByDescendingExpression)
        {
            ThenByExpressions.Add((thenByDescendingExpression, true));
        }
        
        /// <summary>
        /// Apply pagination
        /// </summary>
        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }
        
        /// <summary>
        /// Disable entity tracking
        /// </summary>
        protected void DisableTracking()
        {
            AsNoTracking = true;
        }
    }
}