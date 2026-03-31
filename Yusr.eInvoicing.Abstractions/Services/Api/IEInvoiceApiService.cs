using System.Xml;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities;

namespace Yusr.eInvoicing.Abstractions.Services.Api
{
    public interface IEInvoiceApiService
    {
        Task<OperationResult<EInvoicingApiResponse>> SendComplianceCheckInvoice(XmlDocument signedEInvoice, string binarySecurityToken, string secret, bool Production = false);
        Task<OperationResult<EInvoicingApiResponse>> SendSimplifiedInvoice(XmlDocument signedEInvoice, string binarySecurityToken, string secret, bool Production = false);
        Task<OperationResult<EInvoicingApiResponse>> SendSimplifiedInvoice(EInvoiceRequest invoiceRequest, string binarySecurityToken, string secret, bool Production = false);
        Task<OperationResult<EInvoicingApiResponse>> SendStandardInvoice(XmlDocument signedEInvoice, string binarySecurityToken, string secret, bool Production = false);
        Task<OperationResult<EInvoicingApiResponse>> SendStandardInvoice(EInvoiceRequest invoiceRequest, string binarySecurityToken, string secret, bool Production = false);
    }
}
