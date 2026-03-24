using Microsoft.Extensions.DependencyInjection;
using Yusr.Email.Abstractions.Interfaces;

namespace Yusr.Email
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddYusrEmail(this IServiceCollection services)
        {
            services.AddSingleton<IEmailService, EmailService>();

            return services;
        }
    }
}
