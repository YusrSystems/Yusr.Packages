using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yusr.eInvoicing.Abstractions.Services.Api;
using Yusr.eInvoicing.Abstractions.Services.Csid;
using Yusr.eInvoicing.Abstractions.Services.Csr;
using Yusr.eInvoicing.Abstractions.Services.Entities;
using Yusr.eInvoicing.Abstractions.Services.Initialization;
using Yusr.eInvoicing.Abstractions.Services.Mapper;
using Yusr.eInvoicing.Abstractions.Services.Qr;
using Yusr.eInvoicing.Abstractions.Services.Registration;
using Yusr.eInvoicing.Abstractions.Services.Send;
using Yusr.eInvoicing.Abstractions.Services.Signing;
using Yusr.eInvoicing.Abstractions.Services.Validation;
using Yusr.eInvoicing.Abstractions.Services.Xml;
using Yusr.eInvoicing.Zatca.Entities;
using Yusr.eInvoicing.Zatca.Services.Api;
using Yusr.eInvoicing.Zatca.Services.Csid;
using Yusr.eInvoicing.Zatca.Services.Csr;
using Yusr.eInvoicing.Zatca.Services.Initialization;
using Yusr.eInvoicing.Zatca.Services.Mapper;
using Yusr.eInvoicing.Zatca.Services.Qr;
using Yusr.eInvoicing.Zatca.Services.Registration;
using Yusr.eInvoicing.Zatca.Services.Send;
using Yusr.eInvoicing.Zatca.Services.Signing;
using Yusr.eInvoicing.Zatca.Services.Validation;
using Yusr.eInvoicing.Zatca.Services.Xml;

namespace Yusr.eInvoicing
{
    public static class DependencyInjection
    {
        private static readonly string _zatcaUrl = "https://gw-fatoora.zatca.gov.sa/e-invoicing/";
        public static IServiceCollection AddYusrEInvoicing(this IServiceCollection services)
        {
            // SINGLETON: One instance shared by EVERYONE for the life of the app.
            services.AddSingleton<ICsrService<ZatcaCsrResult>, CsrService>();
            services.AddSingleton<IEInvoiceQrService, QrService>();
            services.AddSingleton<IEInvoiceValidationService, ValidationService>();
            services.AddSingleton<IEInvoiceSignService, SignService>();

            // SCOPED: One instance per HTTP Request.
            services.AddScoped<IEInvoiceComplianceCheckService, ComplianceCheckService>();
            services.AddScoped<IEInvoiceInitializationService, EInvoicingInitializationService>();
            services.AddScoped<IInvoiceMapperService, InvoiceMapperService>();
            services.AddScoped<IEInvoiceRegistrationService, RegistrationService>();
            services.AddScoped<IEInvoiceSendService, InvoiceSendService>();
            services.AddScoped<IEInvoiceXmlService, XmlService>();

            // TRANSIENT: A fresh instance every single time it is requested.
            services.AddHttpClient<ICsidService<ZatcaCsidResult, ZatcaCsrResult>, CsidService>(client =>
            {
                client.BaseAddress = new Uri(_zatcaUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            services.AddHttpClient<IEInvoiceApiService, ZatcaApiService>(client =>
            {
                client.BaseAddress = new Uri(_zatcaUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            return services;
        }
    }
}