namespace Yusr.Core.Abstractions.Entities
{
    public abstract class BaseAddressableEntity : BaseTenantEntity
    {
        public long? CountryId { get; set; } = null;
        public long? CityId { get; set; } = null;
        public string? Street { get; set; } = null;
        public string? District { get; set; } = null;
        public string? BuildingNumber { get; set; } = null;
        public string? PostalCode { get; set; } = null;

        public virtual Country? Country { get; set; } = null!;
        public virtual City? City { get; set; } = null!;

        public string GetAddress()
        {
            var parts = new List<string?>();

            if (!string.IsNullOrWhiteSpace(Country?.Name)) parts.Add(Country.Name);
            if (!string.IsNullOrWhiteSpace(City?.Name)) parts.Add(City.Name);
            if (!string.IsNullOrWhiteSpace(Street)) parts.Add(Street);
            if (!string.IsNullOrWhiteSpace(District)) parts.Add(District);
            if (!string.IsNullOrWhiteSpace(BuildingNumber)) parts.Add(BuildingNumber);
            if (!string.IsNullOrWhiteSpace(PostalCode)) parts.Add(PostalCode);

            return string.Join(" - ", parts);
        }
    }
}
