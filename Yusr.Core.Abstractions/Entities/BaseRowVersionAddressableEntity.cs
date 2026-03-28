namespace Yusr.Core.Abstractions.Entities
{
    public abstract class BaseRowVersionAddressableEntity : BaseAddressableEntity
    {
        public uint RowVer { get; protected set; }
    }
}
