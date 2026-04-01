using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Services.Api
{
    public interface IEInvoicingComplianceCheckService
    {
        Task<OperationResult<bool>> GenerateFullCheck(IEInvoicingSetting setting, EInvoicingEnvironmentType type);
        Task<OperationResult<bool>> SendInvoice(IEInvoicingSetting setting, EInvoiceType eInvoiceType, bool simplified, EInvoicingEnvironmentType type);
    }
}