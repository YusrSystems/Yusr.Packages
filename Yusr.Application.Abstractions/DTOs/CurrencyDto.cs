namespace Yusr.Application.Abstractions.DTOs
{
    public class CurrencyDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool? IsFeminine { get; set; }
        public string? Plural { get; set; }
        public string? SubName { get; set; }
        public bool? SubIsFeminine { get; set; }
        public string? SubPlural { get; set; }
    }
}
