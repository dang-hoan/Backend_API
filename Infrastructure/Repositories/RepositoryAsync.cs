using Application.Interfaces.Repositories;
using Domain.Contracts;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class RepositoryAsync<T, TId> : IRepositoryAsync<T, TId> where T : AuditableBaseEntity<TId>
    {
        private readonly ApplicationDbContext _dbContext;

        public RepositoryAsync(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IQueryable<T> Entities => _dbContext.Set<T>();

        public virtual async Task<T?> GetByIdAsync(TId id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetByCondition(Expression<Func<T, bool>> includeProperties)
        {
            return await _dbContext
                .Set<T>()
                .Where(includeProperties)
                .ToListAsync();
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> includeProperties)
        {
            return await _dbContext
                .Set<T>()
                .Where(includeProperties)
                .SingleOrDefaultAsync();
        }

        public async Task<long> CountByCondition(Expression<Func<T, bool>> includeProperties)
        {
            return await _dbContext.Set<T>().Where(includeProperties).CountAsync();
        }

        public async Task<IReadOnlyList<T>> GetPagedResponseAsync(int pageNumber, int pageSize)
        {
            return await _dbContext
                .Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task<List<T>> AddRangeAsync(List<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            return entities;
        }

        public Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync(List<T> entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            entity.IsDeleted = true;
            _dbContext.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteRange(List<T> entity)
        {
            entity.ForEach(x => x.IsDeleted = true);
            _dbContext.Set<T>().UpdateRange(entity);
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext
                 .Set<T>()
                 .Where(x => !x.IsDeleted)
                 .ToListAsync();
        }

        public async Task<bool> IsUnique(Expression<Func<T, bool>> includeProperties)
        {
            return await _dbContext.Set<T>().AnyAsync(includeProperties);
        }

        public async Task<List<T>> SearchAsync(Expression<Func<T, bool>> includeProperties)
        {
            return await _dbContext.Set<T>().Where(includeProperties).ToListAsync();
        }

        public Task RemoveRangeAsync(List<T> entity)
        {
            _dbContext.RemoveRange(entity);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(T entity)
        {
            _dbContext.Remove(entity);
            return Task.CompletedTask;
        }
    }
}