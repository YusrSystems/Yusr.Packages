namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool TaxIncluded { get; set; }
        public bool Taxable { get; set; }
        public string? ExemptionReasonCode { get; set; }
        public string? ExemptionReason { get; set; }
    }
}
