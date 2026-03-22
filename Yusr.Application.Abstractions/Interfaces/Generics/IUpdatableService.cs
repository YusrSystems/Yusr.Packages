using Yusr.Application.Abstractions.DTOs;
using Yusr.Core.Abstractions.Primitives;

namespace Yusr.Application.Abstractions.Interfaces.Generics
{
    public interface IUpdatableService<TDto> where TDto : BaseDto, new()
    {
        public Task<OperationResult<TDto>> UpdateAsync(TDto dto);
    }
}