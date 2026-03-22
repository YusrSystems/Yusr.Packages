using Yusr.Application.Abstractions.DTOs;
using Yusr.Application.Abstractions.Interfaces.Generics;

namespace Yusr.Application.Abstractions.Interfaces
{
    public interface ICountriesService : IFilterableService<CountryDto>
    {
    }
}
