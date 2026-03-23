using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Yusr.Core.Abstractions.Interfaces;
using Yusr.Core.Abstractions.Options;
using Yusr.Core.Abstractions.Services;
using Yusr.Persistence.Context;
using Yusr.Persistence.Repositories;
using Yusr.Persistence.Services;

namespace Yusr.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddYusrPersistence(this IServiceCollection services)
        {
            services.AddDbContext<YusrDbContext>((serviceProvider, options) =>
            {
                var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

                if (!string.IsNullOrEmpty(dbOptions.SqlConnection))
                {
                    options.UseNpgsql(dbOptions.SqlConnection)
                        .UseSnakeCaseNamingConvention();

                    options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information).EnableSensitiveDataLogging();
                }
            });
            services.AddSingleton<IExceptionService, ExceptionService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IBranchesRepository, BranchesRepository>();
            services.AddScoped<ICitiesRepository, CitiesRepository>();
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<ICurrenciesRepository, CurrenciesRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<ITenantsRepository, TenantsRepository>();

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(DependencyInjection))
                .AddClasses(classes => classes.AssignableTo(typeof(GenericRepository<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            return services;
        }
    }
}
