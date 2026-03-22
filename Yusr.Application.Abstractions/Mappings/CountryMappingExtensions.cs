using Yusr.Application.Abstractions.DTOs;
using Yusr.Core.Abstractions.Entities;

namespace Yusr.Application.Abstractions.Mappings
{
    public static class CountryMappingExtensions
    {
        public static CountryDto ToDto(this Country entity)
        {
            if (entity == null) return null!;

            return new CountryDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
            };
        }

        public static List<CountryDto> ToDtoList(this IEnumerable<Country> entities)
        {
            return entities.Select(e => e.ToDto()).ToList();
        }
    }
}