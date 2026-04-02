using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Services.Api
{
    public interface IEInvoiceComplianceCheckService
    {
        Task<OperationResult<bool>> GenerateFullCheck(IEInvoiceSetting setting, EInvoicingEnvironmentType type);
        Task<OperationResult<bool>> SendInvoice(IEInvoiceSetting setting, EInvoiceType eInvoiceType, bool simplified, EInvoicingEnvironmentType type);
    }
}