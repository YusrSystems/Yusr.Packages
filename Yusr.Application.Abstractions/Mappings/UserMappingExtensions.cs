using Yusr.Application.Abstractions.DTOs;
using Yusr.Core.Abstractions.Entities;

namespace Yusr.Application.Abstractions.Mappings
{
    public static class UserMappingExtensions
    {
        public static UserDto ToDto(this User entity)
        {
            if (entity == null) return null!;

            return new UserDto
            {
                Id = entity.Id,
                Username = entity.Username,
                Password = entity.Password,
                IsActive = entity.IsActive,
                RoleId = entity.RoleId,
                BranchId = entity.BranchId,
                Role = entity.Role?.ToDto(),
                Branch = entity.Branch?.ToDto()
            };
        }

        public static List<UserDto> ToDtoList(this IEnumerable<User> entities)
        {
            return entities.Select(e => e.ToDto()).ToList();
        }
    }
}