using Microsoft.EntityFrameworkCore.Storage;
using Yusr.Bus.Core.Interfaces.Repositories;

namespace Yusr.Persistence.Abstractions.Repositories
{
    public class EfTransactionWrapper : ITransaction
    {
        private readonly IDbContextTransaction _transaction;

        public EfTransactionWrapper(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

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
        }

        public async ValueTask DisposeAsync()
        {
            await _transaction.DisposeAsync();
        }
    }
}