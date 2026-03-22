using Yusr.Application.Abstractions.DTOs;
using Yusr.Core.Abstractions.Primitives;

namespace Yusr.Application.Abstractions.Interfaces.Generics
{
    public interface IAddableService<TDto> where TDto : BaseDto, new()
    {
        public Task<OperationResult<TDto>> AddAsync(TDto dto);
    }
}
