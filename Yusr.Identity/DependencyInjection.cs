using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Yusr.Core.Abstractions.Entities;
using Yusr.Identity.Abstractions.Interfaces;
using Yusr.Identity.Abstractions.Services;
using Yusr.Identity.Services;

namespace Yusr.Identity
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddYusrIdentity<TSystemPermissions>(this IServiceCollection services) where TSystemPermissions : ISystemPermissions
        {
            services.TryAddScoped<ITokenService, TokenService>();
            services.TryAddScoped<IPasswordHasher<IUser>, PasswordHasher<IUser>>();
            services.TryAddScoped<IPasswordService, PasswordService>();
            services.TryAddScoped<IClaimsService, ClaimsService<TSystemPermissions>>();

            return services;
        }

        public static IServiceCollection AddMemoryRolePermissionService(this IServiceCollection services)
        {
            services.AddScoped<IRolePermissionService, MemoryRolePermissionService>();

            return services;
        }

        public static IServiceCollection AddDistributedRolePermissionService(this IServiceCollection services)
        {
            services.AddScoped<IRolePermissionService, DistributedRolePermissionService>();

            return services;
        }
    }
}