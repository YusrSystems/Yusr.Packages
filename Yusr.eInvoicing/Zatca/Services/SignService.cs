using System.Xml;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Services.Signing;
using Yusr.Identity.Abstractions.Primitives;
using ZATCA.EInvoice.SDK;
using ZATCA.EInvoice.SDK.Contracts.Models;

namespace Yusr.Infrastructure.eInvoicing.Zatca.Services
{
    public class SignService : ISignService
    {
        public OperationResult<XmlDocument> SignInvoice(JwtClaims jwtClaims, XmlDocument xmlInvoice, string certificateContent, string PrivateKey)
        {
            try
            {
                EInvoiceSigner signer = new EInvoiceSigner();
                SignResult signResult = signer.SignDocument(xmlInvoice, certificateContent, PrivateKey);

                if (!signResult.IsValid)
                    return OperationResult<XmlDocument>.InternalError("لم يتم توقيع الشهادة بشكل صحيح", signResult.ErrorMessage);

                return OperationResult<XmlDocument>.Ok(signResult.SignedEInvoice);
            }
            catch (Exception ex)
            {
                return OperationResult<XmlDocument>.InternalError("لم يتم توقيع الشهادة بشكل صحيح", ex.Message);
            }
        }
    }
}
