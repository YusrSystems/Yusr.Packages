using System.Text;
using ZATCA.EInvoice.SDK.Contracts.Models;

namespace Yusr.eInvoicing.Zatca.Entities
{
    public class ZatcaParams
    {
        public byte zatcaStatus { get; set; }
        public string privateKey { get; set; } = "";
        public string binarySecurityToken { get; set; } = "";
        public string certificateContent { get; set; } = "";
        public string secret { get; set; } = "";

        public ZatcaParams() { }

        public ZatcaParams(CsidResponse csid, CsrResult csr)
        {
            byte[] csidCertBytes = Convert.FromBase64String(csid.binarySecurityToken);
            certificateContent = Encoding.UTF8.GetString(csidCertBytes);
            binarySecurityToken = csid.binarySecurityToken;
            secret = csid.secret;
            privateKey = csr.PrivateKey;
            zatcaStatus = 0;
        }
    }
}
