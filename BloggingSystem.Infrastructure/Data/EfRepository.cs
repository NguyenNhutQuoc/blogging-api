using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Domain.Commons;
using BloggingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloggingSystem.Infrastructure.Data
{
    /// <summary>
    /// Generic repository implementation using Entity Framework Core
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly BloggingDbContext _dbContext;
        
        public EfRepository(BloggingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get entity by id
        /// </summary>
        public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().FindAsync(id, cancellationToken);
        }

        /// <summary>
        /// Get all entities
        /// </summary>
        public async Task<List<T>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get entities by specification
        /// </summary>
        public async Task<List<T?>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get first entity by specification
        /// </summary>
        public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Count entities by specification
        /// </summary>
        public async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).CountAsync(cancellationToken);
        }

        /// <summary>
        /// Check if entity exists by specification
        /// </summary>
        public async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Add entity
        /// </summary>
        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return entity;
        }

        /// <summary>
        /// Update entity
        /// </summary>
        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Apply specification to query
        /// </summary>
        private IQueryable<T?> ApplySpecification(ISpecification<T> specification)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), specification);
        }
    }
}