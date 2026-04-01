using System.Xml;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Dto;

namespace Yusr.eInvoicing.Abstractions.Services.Xml
{
    public interface IXmlService
    {
        OperationResult<XmlDocument> GenerateXmlEInvoice(EInvoiceDto eInvoice);
        OperationResult<(XmlDocument xmlInvoice, XmlDocument xmlSignedInvoice)> CreateFullXml(EInvoiceDto eInvoice, string certificate, string privateKey);
        string? ExtractValue(XmlDocument signedXml, string xpath);
    }
}