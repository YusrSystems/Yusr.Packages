using System.Xml;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Services.Signing;
using ZATCA.EInvoice.SDK;
using ZATCA.EInvoice.SDK.Contracts.Models;

namespace Yusr.Infrastructure.eInvoicing.Zatca.Services
{
    public class SignService : IEInvoicingSignService
    {
        public OperationResult<XmlDocument> SignInvoice(XmlDocument xmlInvoice, string certificateContent, string privateKey)
        {
            try
            {
                EInvoiceSigner signer = new EInvoiceSigner();
                SignResult signResult = signer.SignDocument(xmlInvoice, certificateContent, privateKey);

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