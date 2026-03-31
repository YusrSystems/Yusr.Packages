namespace Yusr.eInvoicing.Abstractions.Entities
{
    public class EInvoiceRequest
    {
        public string InvoiceHash { get; set; }
        public string Uuid { get; set; }
        public string Invoice { get; set; }

        public EInvoiceRequest()
        {
            InvoiceHash = string.Empty;
            Uuid = string.Empty;
            Invoice = string.Empty;
        }

        public EInvoiceRequest(string invoiceHash, string uuid, string invoice)
        {
            InvoiceHash = invoiceHash;
            Uuid = uuid;
            Invoice = invoice;
        }
    }
}
