using Yusr.Application.Abstractions.DTOs;
using Yusr.Core.Abstractions.Entities;

namespace Yusr.Application.Abstractions.Mappings
{
    public static class BranchMappingExtensions
    {
        public static BranchDto ToDto(this Branch entity)
        {
            if (entity == null) return null!;

            return new BranchDto
            {
                Id = entity.Id,
                Name = entity.Name,
                CityId = entity.CityId,
                District = entity.District,
                Street = entity.Street,
                BuildingNumber = entity.BuildingNumber,
                PostalCode = entity.PostalCode,
                CityName = entity.City?.Name ?? "",
            };
        }

        public static List<BranchDto> ToDtoList(this IEnumerable<Branch> entities)
        {
            return entities.Select(e => e.ToDto()).ToList();
        }
    }
}