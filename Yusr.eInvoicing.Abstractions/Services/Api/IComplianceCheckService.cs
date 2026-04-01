using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Services.Api
{
    public interface IComplianceCheckService
    {
        Task<OperationResult<bool>> GenerateFullCheck(IEInvoicingSetting setting, Branch branch, bool Production = false);
        Task<OperationResult<bool>> SendInvoice(IEInvoicingSetting setting, Branch branch, EInvoiceType type, bool simplified, bool Production = false);
    }
}
