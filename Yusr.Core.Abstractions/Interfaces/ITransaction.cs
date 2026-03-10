namespace Yusr.Core.Abstractions.Interfaces
{
    public interface ITransaction : IAsyncDisposable, IDisposable
    {
        Task CommitAsync();
        Task RollbackAsync();
    }
}
