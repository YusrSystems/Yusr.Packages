namespace Yusr.eInvoicing.Abstractions.Entities
{
    public class EInvoicingApiResponse
    {
        public string ClearedInvoice { get; set; } = string.Empty;
        public List<string> ErrorMessages { get; set; } = [];
        public List<string> WarningMessages { get; set; } = [];
    }
}
