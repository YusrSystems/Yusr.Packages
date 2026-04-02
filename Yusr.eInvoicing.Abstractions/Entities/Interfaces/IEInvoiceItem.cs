namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IEInvoiceItem
    {
        public long Id { get; }
        public string Name { get; }
        public bool TaxIncluded { get; }
        public bool Taxable { get; }
        public string? ExemptionReasonCode { get; }
        public string? ExemptionReason { get; }
    }
}
