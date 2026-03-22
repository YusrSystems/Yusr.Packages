using Yusr.Application.Abstractions.DTOs;

namespace Yusr.Application.Abstractions.Interfaces.Generics
{
    public interface IBaseService<TDto> : IFilterableService<TDto>,
                                                IRetrievableService<TDto>,
                                                IAddableService<TDto>,
                                                IUpdatableService<TDto>,
                                                IDeletableService
        where TDto : BaseDto, new()
    {
    }
}
