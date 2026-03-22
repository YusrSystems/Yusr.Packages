using Yusr.Core.Abstractions.Entities;

namespace Yusr.Core.Abstractions.Interfaces.Generics
{
    public interface IAddableRepository<TEntity> where TEntity : BaseEntity
    {
        public Task<TEntity> AddAsync(TEntity entity);
    }
}
