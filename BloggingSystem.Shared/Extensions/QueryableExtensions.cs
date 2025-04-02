using System;
using System.Linq;
using System.Linq.Expressions;
using BloggingSystem.Shared.DTOs;
using BloggingSystem.Shared.Models;

namespace BloggingSystem.Shared.Extensions
{
    /// <summary>
    /// Extensions for IQueryable
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Apply paging to query
        /// </summary>
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, PaginationParams parameters)
        {
            return query
                .Skip((parameters.PageSize - 1) * parameters.PageSize)
                .Take(parameters.PageSize);
        }
        
        /// <summary>
        /// Apply sorting to query
        /// </summary>
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string orderBy, string orderDirection)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return query;
            }
            
            // Get property info
            var propertyInfo = typeof(T).GetProperty(orderBy, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            
            if (propertyInfo == null)
            {
                return query;
            }
            
            // Create parameter expression
            var parameter = Expression.Parameter(typeof(T), "x");
            
            // Create member expression
            var property = Expression.Property(parameter, propertyInfo);
            
            // Create lambda expression
            var lambda = Expression.Lambda(property, parameter);
            
            // Get method info for OrderBy or OrderByDescending
            var methodName = string.Equals(orderDirection, "desc", StringComparison.OrdinalIgnoreCase) 
                ? "OrderByDescending" 
                : "OrderBy";
                
            var method = typeof(Queryable).GetMethods()
                .Single(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), propertyInfo.PropertyType);
                
            // Apply ordering
            return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda });
        }
    }
}