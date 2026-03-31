namespace Yusr.eInvoicing.Abstractions.Dto
{
    public class EInvoiceAddressDto
    {
        public string StreetName { get; set; } = string.Empty;
        public string BuildingNumber { get; set; } = string.Empty;
        public string CitySubdivisionName { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public string PostalZone { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
    }
}
