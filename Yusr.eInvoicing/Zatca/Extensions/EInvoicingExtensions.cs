

using Microsoft.Extensions.DependencyInjection;
using Yusr.eInvoicing.Abstractions.Services;

namespace Yusr.Infrastructure.eInvoicing.Zatca.Extensions
{
    public static class EInvoicingExtensions
    {
        public static IServiceCollection AddEInvoicing(this IServiceCollection services)
        {
            services.AddScoped<IEInvoicingService, ZatcaService>();

            return services;
        }
    }
}
