using Yusr.Application.Abstractions.DTOs;
using Yusr.Core.Abstractions.Primitives;

namespace Yusr.Application.Abstractions.Interfaces.Generics
{
    public interface IFilterableService<TDto> where TDto : BaseDto
    {
        public Task<OperationResult<FilterResponse<TDto>>> FilterAsync(int pageNumber, int rowsPerPage, FilterCondition? condition);
    }
}
