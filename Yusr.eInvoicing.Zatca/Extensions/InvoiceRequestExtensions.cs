using Yusr.eInvoicing.Abstractions.Entities;
using ZATCA.EInvoice.SDK.Contracts.Models;

namespace Yusr.eInvoicing.Zatca.Extensions
{
    public static class InvoiceRequestExtensions
    {
        public static EInvoiceRequest ToEInvoiceRequest(this InvoiceRequest invoiceRequest)
        {
            return new EInvoiceRequest(invoiceRequest.InvoiceHash, invoiceRequest.Uuid, invoiceRequest.Invoice);
        }
    }
}
