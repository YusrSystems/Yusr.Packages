using Yusr.Application.Abstractions.DTOs;
using Yusr.Core.Abstractions.Entities;

namespace Yusr.Application.Abstractions.Mappings
{
    public static class RoleMappingExtensions
    {
        public static RoleDto ToDto(this Role entity)
        {
            if (entity == null) return null!;

            return new RoleDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Permissions = entity.Permissions
            };
        }

        public static List<RoleDto> ToDtoList(this IEnumerable<Role> entities)
        {
            return [.. entities.Select(e => e.ToDto())];
        }
    }
}