using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Yusr.Bus.Presentation.Api.Extensions
{
    public static class CorsExtensions
    {
        private static string CorsName = "AllowFrontend";

        public static IServiceCollection AddYusrCors(this IServiceCollection services, params string[] origins)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CorsName, policy =>
                {
                    policy.WithOrigins(origins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            return services;
        }

        public static IApplicationBuilder UseYusrCors(this IApplicationBuilder app)
        {
            return app.UseCors(CorsName);
        }
    }
}
