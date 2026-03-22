using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Primitives;

namespace Yusr.Core.Abstractions.Interfaces.Generics
{
    public interface IFilterableRepository<TEntity> where TEntity : BaseEntity
    {
        public Task<FilterResponse<TEntity>> FilterAsync(int pageNumber, int rowsPerPage, FilterCondition? condition);
    }
}
