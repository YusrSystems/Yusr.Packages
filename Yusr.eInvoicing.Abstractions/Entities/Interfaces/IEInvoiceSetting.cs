using Yusr.Core.Abstractions.Entities;
using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IEInvoiceSetting
    {
        public string? CertificateContent { get; }
        public string? PrivateKey { get; }
        public EInvoicingEnvironmentType EInvoicingEnvironmentType { get; }
        public string? BinarySecurityToken { get; }
        public string? Secret { get; }
        public Tenant Tenant { get; }
        public Branch Branch { get; }
    }
}
