using Yusr.Core.Abstractions.Entities;
using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IEInvoicingSetting
    {
        public string? CertificateContent { get; set; }
        public string? PrivateKey { get; set; }
        public EInvoicingStatus EInvoicingStatus { get; set; }
        public string? BinarySecurityToken { get; set; }
        public string? Secret { get; set; }
        public Tenant Tenant { get; set; }
    }
}
