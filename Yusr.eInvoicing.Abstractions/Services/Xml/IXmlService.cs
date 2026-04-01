using System.Xml;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Dto;
using Yusr.Identity.Abstractions.Primitives;

namespace Yusr.eInvoicing.Abstractions.Services.Xml
{
    public interface IXmlService
    {
        OperationResult<XmlDocument> GenerateXmlEInvoice(EInvoiceDto eInvoice, JwtClaims jwtClaims, string Certificate, string PrivateKey);
        OperationResult<(XmlDocument xmlInvoice, XmlDocument xmlSignedInvoice)> CreateFullXml(EInvoiceDto eInvoice, JwtClaims jwtClaims, string Certificate, string PrivateKey);
        string? ExtractValue(XmlDocument signedXml, string xpath);
    }
}
