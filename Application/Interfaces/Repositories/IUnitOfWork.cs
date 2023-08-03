using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Interfaces.Repositories
{

    public interface IUnitOfWork<TId> : IDisposable
    {
        Task<int> Commit(CancellationToken cancellationToken);

        Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

        Task Rollback();

        IDbContextTransaction BeginTransaction();

        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}