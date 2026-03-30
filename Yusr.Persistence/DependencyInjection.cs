using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Yusr.Core.Abstractions.Interfaces;
using Yusr.Core.Abstractions.Interfaces.Generics;
using Yusr.Core.Abstractions.Services;
using Yusr.Infrastructure.Persistence.Interceptors;
using Yusr.Persistence.Repositories;
using Yusr.Persistence.Services;

namespace Yusr.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddYusrCommonRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IExceptionService, ExceptionService>();
            services.AddSingleton<IReportContextService, ReportContextService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<SlowQueryInterceptor>();
            services.AddScoped<TenantRowLevelSecurityInterceptor>();

            services.AddScoped(typeof(IBaseRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IBranchesRepository, BranchesRepository>();
            services.AddScoped<ICitiesRepository, CitiesRepository>();
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<ICurrenciesRepository, CurrenciesRepository>();
            services.AddScoped<IRolesRepository, RolesRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<ITenantsRepository, TenantsRepository>();

            return services;
        }

        public static DbContextOptionsBuilder AddYusrInterceptors(this DbContextOptionsBuilder optionsBuilder, IServiceProvider serviceProvider)
        {
            optionsBuilder.AddInterceptors(
                serviceProvider.GetRequiredService<SlowQueryInterceptor>(),
                serviceProvider.GetRequiredService<TenantRowLevelSecurityInterceptor>()
            );

            return optionsBuilder;
        }
    }
}