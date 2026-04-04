using Microsoft.Extensions.DependencyInjection;
using Yusr.Api.Abstractions.Attributes;

namespace Yusr.Api.Abstractions
{
    public static class DependencyInjection
    {
        public static IServiceCollection ChangeFormatFunc(this IServiceCollection services, Func<string, string, string> formatFunc)
        {
            AuthorizePermissionAttribute.Formatter = formatFunc;

            return services;
        }
    }
}