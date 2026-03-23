using Microsoft.Extensions.DependencyInjection;
using Yusr.Api.Abstractions.Services;
using Yusr.Application.Abstractions.Interfaces;
using Yusr.Application.Services;
using Yusr.Core.Abstractions.Services;
using Yusr.Identity.Abstractions.Interfaces;

namespace Yusr.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddYusrCommonServices<TSystemPermissions>(this IServiceCollection services) where TSystemPermissions : ISystemPermissions
        {
            services.AddYusrCommonServices();
            services.AddScoped<IRolesService, RolesService<TSystemPermissions>>();

            return services;
        }

        public static IServiceCollection AddYusrCommonServices(this IServiceCollection services)
        {
            services.AddScoped<ITenantService, HttpContextTenantService>();
            services.AddScoped<IBranchesService, BranchesService>();
            services.AddScoped<ICitiesService, CitiesService>();
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<ICurrenciesService, CurrenciesService>();
            services.AddScoped<IUsersService, UsersService>();

            return services;
        }
    }
}