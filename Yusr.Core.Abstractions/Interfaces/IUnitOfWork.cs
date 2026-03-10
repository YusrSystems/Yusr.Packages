namespace Yusr.Core.Abstractions.Interfaces
{
    public interface IUnitOfWork
    {
        Task<ITransaction> BeginTransactionAsync();
    }
}
