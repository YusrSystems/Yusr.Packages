using Microsoft.AspNetCore.Authorization;
using Yusr.Core.Abstractions.Constants;

namespace Yusr.Api.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizePermissionAttribute : AuthorizeAttribute
    {
        public AuthorizePermissionAttribute(string resource, string action)
        {
            Policy = BaseSystemPermissions.Create(resource, action);
        }
    }
}