using Microsoft.AspNetCore.Authorization;
using Yusr.Identity.Abstractions.Interfaces;

namespace Yusr.Api.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizePermissionAttribute : AuthorizeAttribute
    {
        public static Func<string, string, string> Formatter { get; set; } = (resource, action) => ISystemPermissions.Create(resource, action);

        public AuthorizePermissionAttribute(string resource, string action)
        {
            Policy = Formatter(resource, action);
        }
    }
}