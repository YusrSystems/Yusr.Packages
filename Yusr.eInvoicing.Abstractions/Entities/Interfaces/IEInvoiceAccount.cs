namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IEInvoiceAccount
    {
        public long Id { get; }
        public string Name { get; }
        public string? VatNumber { get; }
        public string? Crn { get; }
        public string? Street { get; }
        public string? District { get; }
        public string? BuildingNumber { get; }
        public string? PostalCode { get; }
    }
}
