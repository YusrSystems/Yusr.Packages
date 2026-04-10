namespace Yusr.Application.Abstractions.DTOs
{
    public class BranchDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public long? CityId { get; set; }
        public string? Street { get; set; } = null;
        public string? District { get; set; } = null;
        public string? BuildingNumber { get; set; } = null;
        public string? PostalCode { get; set; } = null;


        public CityDto? City { get; set; }
    }
}
