using System.Xml;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities;
using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Services.Api
{
    public interface IEInvoiceApiService
    {
        Task<OperationResult<EInvoicingApiResponse>> SendComplianceCheckInvoice(XmlDocument signedEInvoice, string binarySecurityToken, string secret, EInvoicingEnvironmentType type);
        Task<OperationResult<EInvoicingApiResponse>> SendSimplifiedInvoice(XmlDocument signedEInvoice, string binarySecurityToken, string secret, EInvoicingEnvironmentType type);
        Task<OperationResult<EInvoicingApiResponse>> SendSimplifiedInvoice(EInvoiceRequest invoiceRequest, string binarySecurityToken, string secret, EInvoicingEnvironmentType type);
        Task<OperationResult<EInvoicingApiResponse>> SendStandardInvoice(XmlDocument signedEInvoice, string binarySecurityToken, string secret, EInvoicingEnvironmentType type);
        Task<OperationResult<EInvoicingApiResponse>> SendStandardInvoice(EInvoiceRequest invoiceRequest, string binarySecurityToken, string secret, EInvoicingEnvironmentType type);
    }
}
