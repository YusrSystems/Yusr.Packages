namespace Yusr.Core.Abstractions.Options
{
    public class DatabaseOptions
    {
        public const string SectionName = "DatabaseSettings";

        public string SqlConnection { get; set; } = string.Empty;
    }
}
