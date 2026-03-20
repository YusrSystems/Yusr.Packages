namespace Yusr.Identity.Abstractions.Services
{
    public interface IRolePermissionService
    {
        Task<bool> HasPermissionAsync(long roleId, string permission);
        string BuildCacheKey(long roleId);
    }
}
