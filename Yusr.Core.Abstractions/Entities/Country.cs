namespace Yusr.Core.Abstractions.Entities
{
    public class Country : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Code { get; private set; } = string.Empty;
    }
}
