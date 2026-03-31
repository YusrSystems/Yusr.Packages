using Yusr.eInvoicing.Abstractions.Entities;

namespace Yusr.eInvoicing.Abstractions.Dto
{
    public class EInvoicePrepareDto
    {
        public EInvoiceRequest InvoiceRequest { get; set; } = new EInvoiceRequest();
        public string QrBase64 { get; set; } = string.Empty;
        public bool Simplified { get; set; }
    }
}
