using System.Text;
using Yusr.eInvoicing.Abstractions.Services.Entities;
using ZATCA.EInvoice.SDK.Contracts.Models;

namespace Yusr.eInvoicing.Zatca.Entities
{
    public class ZatcaParams
    {
        public byte ZatcaStatus { get; set; }
        public string PrivateKey { get; set; } = "";
        public string BinarySecurityToken { get; set; } = "";
        public string CertificateContent { get; set; } = "";
        public string Secret { get; set; } = "";

        public ZatcaParams() { }

        public ZatcaParams(ZatcaCsidResult csid, ZatcaCsrResult csr)
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
