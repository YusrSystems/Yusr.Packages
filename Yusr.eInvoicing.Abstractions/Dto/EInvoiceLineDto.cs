namespace Yusr.eInvoicing.Abstractions.Dto
{
    public class EInvoiceLineDto
    {
        public decimal Quantity { get; set; }
        public decimal NoTaxPrice { get; set; }
        public decimal NoTaxTotalPrice { get; set; }
        public decimal TaxPrice { get; set; }
        public decimal TaxTotalPrice { get; set; }
        public decimal TaxAmount { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TaxExemptionReasonCode { get; set; } = string.Empty;
        public string TaxExemptionReason { get; set; } = string.Empty;
        public bool Taxable { get; set; }
        public decimal TotalTaxPercent { get; set; }
        public decimal AllowanceChargeAmount { get; set; }
    }
}
