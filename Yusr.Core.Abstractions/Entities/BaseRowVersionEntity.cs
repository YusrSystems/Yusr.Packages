namespace Yusr.Core.Abstractions.Entities
{
    public abstract class BaseRowVersionEntity : BaseTenantEntity
    {
        public uint RowVer { get; protected set; }
    }
}
