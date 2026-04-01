using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yusr.eInvoicing.Abstractions.Services;
using Yusr.Infrastructure.eInvoicing.Zatca;

namespace Yusr.Infrastructure.eInvoicing
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddYusrEInvoicing(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IEInvoicingService, ZatcaService>();

            return services;
        }
    }
}
