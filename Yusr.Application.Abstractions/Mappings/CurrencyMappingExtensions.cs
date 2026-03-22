using Yusr.Application.Abstractions.DTOs;
using Yusr.Core.Abstractions.Entities;

namespace Yusr.Application.Abstractions.Mappings
{
    public static class CurrencyMappingExtensions
    {
        public static CurrencyDto ToDto(this Currency entity)
        {
            if (entity == null) return null!;

            return new CurrencyDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                IsFeminine = entity.IsFeminine,
                Plural = entity.Plural,
                SubName = entity.SubName,
                SubIsFeminine = entity.SubIsFeminine,
                SubPlural = entity.SubPlural,
            };
        }

        public static List<CurrencyDto> ToDtoList(this IEnumerable<Currency> entities)
        {
            return entities.Select(e => e.ToDto()).ToList();
        }
    }
}