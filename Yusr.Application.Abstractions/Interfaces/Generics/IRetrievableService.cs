using Yusr.Application.Abstractions.DTOs;
using Yusr.Core.Abstractions.Primitives;

namespace Yusr.Application.Abstractions.Interfaces.Generics
{
    public interface IRetrievableService<TDto> where TDto : BaseDto, new()
    {
        public Task<OperationResult<TDto>> GetByIdAsync(long id);
    }
}
