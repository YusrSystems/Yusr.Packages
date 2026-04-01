using Yusr.Core.Abstractions.Entities;

namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IAccount
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? VatNumber { get; set; }
        public string? Crn { get; set; }
        public string? Street { get; set; }
        public string? District { get; set; }
        public string? BuildingNumber { get; set; }
        public string? PostalCode { get; set; }
        public City? City { get; set; }
    }
}
