using Yusr.Core.Abstractions.Entities;

namespace Yusr.Core.Abstractions.Interfaces.Generics
{
    public interface IBaseRepository<TEntity> : IFilterableRepository<TEntity>, IRetrievableRepository<TEntity>, IAddableRepository<TEntity>, IUpdatableRepository<TEntity>, IDeletableRepository<TEntity>
        where TEntity : BaseEntity
    {
        public Task<List<TEntity>> GetByIdsAsync(List<long> ids, bool track = false);
        public Task<TEntity?> FindAsync(long id);
        public Task AddRangeAsync(IEnumerable<TEntity> entities);
        public Task UpdateRangeAsync(IEnumerable<TEntity> entities);
        public void RemoveRange(IEnumerable<TEntity> entities);
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
