using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using BloggingSystem.Domain.Commons;

namespace BloggingSystem.Application.Commons.Interfaces
{
    /// <summary>
    /// Generic repository interface
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Get entity by id
        /// </summary>
        Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Get all entities
        /// </summary>
        Task<List<T>> ListAllAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Get entities by specification
        /// </summary>
        Task<List<T?>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Get first entity by specification
        /// </summary>
        Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Count entities by specification
        /// </summary>
        Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Check if entity exists by specification
        /// </summary>
        Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Add entity
        /// </summary>
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Update entity
        /// </summary>
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Delete entity
        /// </summary>
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }
}