using Yusr.Application.Abstractions.DTOs;
using Yusr.Core.Abstractions.Entities;

namespace Yusr.Application.Abstractions.Mappings
{
    public static class CityMappingExtensions
    {
        public static CityDto ToDto(this City entity)
        {
            if (entity == null) return null!;

            return new CityDto
            {
                Id = entity.Id,
                Name = entity.Name,
                CountryId = entity.CountryId,
                Country = entity.Country.ToDto(),
            };
        }

        public static List<CityDto> ToDtoList(this IEnumerable<City> entities)
        {
            return entities.Select(e => e.ToDto()).ToList();
        }
    }
}