using Yusr.Core.Abstractions.Entities;

namespace Yusr.Core.Abstractions.Interfaces.Generics
{
    public interface IUpdatableRepository<TEntity> where TEntity : BaseEntity
    {
        public Task UpdateAsync(TEntity entity);
    }
}
