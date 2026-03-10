using Microsoft.AspNetCore.Authorization;
using Yusr.Core.Abstractions.Interfaces;

namespace Yusr.Api.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizePermissionAttribute : AuthorizeAttribute
    {
        public AuthorizePermissionAttribute(string resource, string action)
        {
            Policy = ISystemPermissions.Create(resource, action);
        }
    }
}