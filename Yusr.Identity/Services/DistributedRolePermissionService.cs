using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Yusr.Core.Abstractions.Interfaces;
using Yusr.Identity.Abstractions.Services;

namespace Yusr.Identity.Services
{
    public class DistributedRolePermissionService : IRolePermissionService
    {
        private readonly IRolesRepository _rolesRepo;
        private readonly IDistributedCache _cache;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public DistributedRolePermissionService(IRolesRepository rolesRepo, IDistributedCache cache)
        {
            _rolesRepo = rolesRepo;
            _cache = cache;
        }

        public async Task<bool> HasPermissionAsync(long roleId, string permission)
        {
            string cacheKey = BuildCacheKey(roleId);
            HashSet<string>? permissions = null;

            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                permissions = JsonSerializer.Deserialize<HashSet<string>>(cachedData, _jsonOptions);
            }
            else
            {
                var role = await _rolesRepo.GetByIdAsync(roleId);
                permissions = role?.Permissions.ToHashSet() ?? new HashSet<string>();

                var serializedData = JsonSerializer.Serialize(permissions, _jsonOptions);
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };

                await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);
            }

            return permissions?.Contains(permission) ?? false;
        }

        public string BuildCacheKey(long roleId) => $"RolePermissions_{roleId}";
    }
}
