using Yusr.Core.Abstractions.Entities;
using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IEInvoiceSetting
    {
        public string? CertificateContent { get; protected set; }
        public string? PrivateKey { get; protected set; }
        public EInvoicingEnvironmentType EInvoicingEnvironmentType { get; protected set; }
        public string? BinarySecurityToken { get; protected set; }
        public string? Secret { get; protected set; }
        public Tenant Tenant { get; protected set; }
        public Branch Branch { get; protected set; }
    }
}
