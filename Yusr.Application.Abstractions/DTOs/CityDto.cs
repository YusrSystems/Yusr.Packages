namespace Yusr.Application.Abstractions.DTOs
{
    public class CityDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public long CountryId { get; set; }

        public CountryDto? Country { get; set; }
    }
}
