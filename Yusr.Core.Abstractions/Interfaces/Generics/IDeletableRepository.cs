using Yusr.Core.Abstractions.Entities;

namespace Yusr.Core.Abstractions.Interfaces.Generics
{
    public interface IDeletableRepository<TEntity> where TEntity : BaseEntity
    {
        public Task<int> DeleteAsync(long id);
    }
}
