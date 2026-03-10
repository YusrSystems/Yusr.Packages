using Microsoft.EntityFrameworkCore.Storage;
using Yusr.Core.Abstractions.Interfaces;

namespace Yusr.Persistence.Repositories
{
    public class EfTransactionWrapper(IDbContextTransaction transaction) : ITransaction
    {
        private readonly IDbContextTransaction _transaction = transaction;

        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }

        public void Dispose()
        {
            _transaction.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await _transaction.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}