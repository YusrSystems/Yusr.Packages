namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IAccount
    {
        public long Id { get; protected set; }
        public string Name { get; protected set; }
        public string? VatNumber { get; protected set; }
        public string? Crn { get; protected set; }
        public string? Street { get; protected set; }
        public string? District { get; protected set; }
        public string? BuildingNumber { get; protected set; }
        public string? PostalCode { get; protected set; }
    }
}
