using System.Text;
using Yusr.eInvoicing.Abstractions.Services.Entities;

namespace Yusr.eInvoicing.Zatca.Entities
{
    public class ZatcaSigningContext
    {
        public byte ZatcaStatus { get; set; }
        public string PrivateKey { get; set; } = string.Empty;
        public string BinarySecurityToken { get; set; } = string.Empty;
        public string CertificateContent { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;

        public ZatcaSigningContext(ZatcaCsidResult csid, ZatcaCsrResult csr)
        {
            byte[] csidCertBytes = Convert.FromBase64String(csid.BinarySecurityToken);
            CertificateContent = Encoding.UTF8.GetString(csidCertBytes);
            BinarySecurityToken = csid.BinarySecurityToken;
            Secret = csid.Secret;
            PrivateKey = csr.PrivateKey;
            ZatcaStatus = 0;
        }
    }
}
