namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IEInvoiceItem
    {
        public long Id { get; protected set; }
        public string Name { get; protected set; }
        public bool TaxIncluded { get; protected set; }
        public bool Taxable { get; protected set; }
        public string? ExemptionReasonCode { get; protected set; }
        public string? ExemptionReason { get; protected set; }
    }
}
