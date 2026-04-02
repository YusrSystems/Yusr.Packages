using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Dto;
using Yusr.eInvoicing.Abstractions.Entities;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Services.Send
{
    public interface IEInvoiceSendService
    {
        Task<OperationResult<EInvoiceStatus>> SendEInvoice(EInvoicePrepareDto eInvoicePrepareDto, EInvoicingEnvironmentType eInvoicingEnvironmentType, string binarySecurityToken, string secret);
        Task<OperationResult<EInvoiceStatus>> SendEInvoice(EInvoiceRequest invoiceRequest, EInvoicingEnvironmentType eInvoicingEnvironmentType, string binarySecurityToken, string secret, bool isSimplified);
        Task<OperationResult<EInvoiceStatus>> ResendEInvoiceAsync(IEInvoiceSetting setting, IEInvoiceInvoice invoice);
    }
}
