using Microsoft.Extensions.DependencyInjection;
using Yusr.Erp.Application.Accounting.Interfaces;

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
