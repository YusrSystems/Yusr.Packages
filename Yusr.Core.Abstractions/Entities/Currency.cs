namespace Yusr.Core.Abstractions.Entities
{
    public class Currency : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Code { get; private set; } = string.Empty;
        public bool? IsFeminine { get; private set; }
        public string? Plural { get; private set; }
        public string? SubName { get; private set; }
        public bool? SubIsFeminine { get; private set; }
        public string? SubPlural { get; private set; }
    }
}
