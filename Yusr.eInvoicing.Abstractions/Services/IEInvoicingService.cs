using System.Xml;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Dto;
using Yusr.eInvoicing.Abstractions.Entities;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;
using Yusr.Identity.Abstractions.Primitives;

namespace Yusr.eInvoicing.Abstractions.Services
{
    public interface IEInvoicingService
    {
        OperationResult<EInvoicePrepareDto> PrepareEInvoice(EInvoiceDto eInvoice, string certificateContent, string privateKey, bool ignoreWarnings, JwtClaims jwtClaims);
        OperationResult<bool> ValidateEInvoice(EInvoiceDto eInvoice, bool IgnoreWarnings, JwtClaims jwtClaims);
        Task<OperationResult<EInvoiceStatus>> SendEInvoice(EInvoicePrepareDto eInvoicePrepareDto, EInvoicingStatus eInvoicingStatus, string binarySecurityToken, string secret, JwtClaims jwtClaims);
        Task<OperationResult<EInvoiceStatus>> SendEInvoice(EInvoiceRequest invoiceRequest, EInvoicingStatus eInvoicingStatus, string binarySecurityToken, string secret, bool isSimplified, JwtClaims jwtClaims);
        string? GenerateQrBase64(string CompanyName, string CompanyVatNumber, string TimeStamp, string TotalWithVat, string VatAmount);
        string? ExtractValueFromXml(XmlDocument signedXml, string xpath);
        byte[] GenerateQrCode(string base64Tlv);
        Task<OperationResult<bool>> LinkEInvoicing(string OTP, bool Production, JwtClaims jwtClaims);
        EInvoiceDto GetEInvoiceData(IInvoice invoice, Tenant tenant, Branch branch, IAccount customer, List<IItem> dbItems, long? lastCounter, string? lastHash);
        Task<OperationResult<EInvoicePrepareDto?>> PrepareInvoiceAsync(Tenant tenant, IEInvoicingSetting setting, Branch branch, IInvoice invoice, IAccount actionAccount, List<IItem> dbItems, long? lastCounter, string? lastHash, bool ignoreWarnings, JwtClaims jwtClaims);
        Task<OperationResult<EInvoiceStatus>> ResendEInvoiceAsync(IInvoice invoice, JwtClaims jwtClaims);
    }
}
