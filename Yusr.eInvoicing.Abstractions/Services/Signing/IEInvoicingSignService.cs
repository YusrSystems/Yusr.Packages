using System.Xml;
using Yusr.Core.Abstractions.Primitives;

namespace Yusr.eInvoicing.Abstractions.Services.Signing
{
    public interface IEInvoicingSignService
    {
        OperationResult<XmlDocument> SignInvoice(XmlDocument xmlInvoice, string certificateContent, string privateKey);
    }
}
