namespace Yusr.Core.Abstractions.Entities
{
    public class Branch : BaseAddressableEntity
    {
        public string Name { get; private set; } = string.Empty;

        public static Branch Create(string name, long? cityId, string? street, string? district, string? buildingNumber, string? postalCode)
        {
            return new Branch
            {
                Name = name,
                CityId = cityId,
                Street = street,
                District = district,
                BuildingNumber = buildingNumber,
                PostalCode = postalCode,
            };
        }

        public Branch Update(string name, long? cityId, string? street, string? district, string? buildingNumber, string? postalCode)
        {
            Name = name;
            CityId = cityId;
            Street = street;
            District = district;
            BuildingNumber = buildingNumber;
            PostalCode = postalCode;
            return this;
        }
    }
}