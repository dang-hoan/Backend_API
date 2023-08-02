using Domain.Contracts;
using System.Linq.Expressions;

namespace Application.Interfaces.Repositories
{
    public interface IRepositoryAsync<T, in TId> where T : class, IEntity<TId>
    {
        IQueryable<T> Entities { get; }

        Task<T?> GetByIdAsync(TId id);

        Task<IReadOnlyList<T>> GetAllAsync();

        Task<IReadOnlyList<T>> GetByCondition(Expression<Func<T, bool>> includeProperties);

        Task<T?> FindAsync(Expression<Func<T, bool>> includeProperties);

        Task<long> CountByCondition(Expression<Func<T, bool>> includeProperties);

        Task<IReadOnlyList<T>> GetPagedResponseAsync(int pageNumber, int pageSize);

        Task<T> AddAsync(T entity);

        Task<List<T>> AddRangeAsync(List<T> entities);

        Task UpdateAsync(T entity);

        Task UpdateRangeAsync(List<T> entities);

        Task DeleteAsync(T entity);

        Task DeleteRange(List<T> entity);

        Task RemoveAsync(T entity);

        Task RemoveRangeAsync(List<T> entity);

        Task<bool> IsUnique(Expression<Func<T, bool>> includeProperties);
    }
}