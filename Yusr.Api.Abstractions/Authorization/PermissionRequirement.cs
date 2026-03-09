using Microsoft.AspNetCore.Authorization;

namespace Yusr.Api.Abstractions.Authorization
{
    public class PermissionRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}
