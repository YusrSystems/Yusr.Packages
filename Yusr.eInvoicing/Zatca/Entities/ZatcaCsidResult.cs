using Yusr.eInvoicing.Abstractions.Entities.Interfaces;

namespace Yusr.eInvoicing.Zatca.Entities
{
    public class ZatcaCsidResult : ICsidResult
    {
        public long RequestId { get; set; }
        public string DispositionMessage { get; set; } = string.Empty;
        public string BinarySecurityToken { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
    }
}
