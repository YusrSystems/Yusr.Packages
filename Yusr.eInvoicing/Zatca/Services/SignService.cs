using System.Xml;
using Yusr.Identity.Abstractions.Primitives;
using ZATCA.EInvoice.SDK;
using ZATCA.EInvoice.SDK.Contracts.Models;

namespace Yusr.Infrastructure.eInvoicing.Zatca.Services
{
    public class SignService
    {
        public static (bool IsValid, string ErrorMessage, XmlDocument? SignedEInvoice) SignInvoice(JwtClaims jwtClaims, XmlDocument xmlInvoice, string certificateContent, string PrivateKey)
        {
            EInvoiceSigner signer = new EInvoiceSigner();
            SignResult signResult = signer.SignDocument(xmlInvoice, certificateContent, PrivateKey);

            return (signResult.IsValid, signResult.ErrorMessage, signResult.SignedEInvoice);
        }
    }
}
