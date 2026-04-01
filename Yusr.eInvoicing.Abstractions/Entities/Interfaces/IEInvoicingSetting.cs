using Yusr.eInvoicing.Abstractions.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Yusr.eInvoicing.Abstractions.Entities.Interfaces
{
    public interface IEInvoicingSetting
    {
        public string? CertificateContent { get; set; }
        public string? PrivateKey { get; set; }
        public EInvoicingStatus EInvoicingStatus { get; set; }
        public string? BinarySecurityToken { get; set; }
        public string? certificateContent { get; set; }

        public string? Secret { get; set; }

    }
}
