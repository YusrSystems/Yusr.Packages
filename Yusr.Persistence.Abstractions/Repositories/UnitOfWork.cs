using Yusr.Bus.Core.Interfaces.Repositories;
using Yusr.Bus.Persistence.Context;

namespace Yusr.Persistence.Abstractions.Repositories
{
    public class UnitOfWork(YusrDbContext context) : IUnitOfWork
    {
        private readonly YusrDbContext _context = context;

        public async Task<ITransaction> BeginTransactionAsync()
        {
            var efTransaction = await _context.Database.BeginTransactionAsync();
            return new EfTransactionWrapper(efTransaction);
        }
    }
}
