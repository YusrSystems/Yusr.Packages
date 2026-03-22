namespace Yusr.Core.Abstractions.Entities
{
    public class City : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public long CountryId { get; private set; }

        public virtual Country Country { get; set; } = null!;
    }
}
