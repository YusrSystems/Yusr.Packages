using Microsoft.Extensions.Caching.Memory;
using Yusr.Core.Abstractions.Interfaces;
using Yusr.Identity.Abstractions.Services;

namespace Yusr.Identity.Services
{
    public class MemoryRolePermissionService : IRolePermissionService
    {
        private readonly IRolesRepository _rolesRepo;
        private readonly IMemoryCache _cache;

        public MemoryRolePermissionService(IRolesRepository rolesRepo, IMemoryCache cache)
        {
            _rolesRepo = rolesRepo;
            _cache = cache;
        }

        public async Task<bool> HasPermissionAsync(long roleId, string permission)
        {
            string cacheKey = BuildCacheKey(roleId);

            var cachedPermissions = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                var role = await _rolesRepo.GetByIdAsync(roleId);
                return role?.Permissions.ToHashSet() ?? new HashSet<string>();
            });

            return cachedPermissions?.Contains(permission) ?? false;
        }

        public string BuildCacheKey(long roleId) => $"RolePermissions_{roleId}";
    }
}
