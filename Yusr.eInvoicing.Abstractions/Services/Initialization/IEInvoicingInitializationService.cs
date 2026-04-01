using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Dto;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;

namespace Yusr.eInvoicing.Abstractions.Services.Initialization
{
    public interface IEInvoicingInitializationService
    {
        OperationResult<EInvoicePrepareDto?> Init(IEInvoicingSetting setting, IInvoice invoice, IAccount actionAccount, List<IItem> dbItems, long? lastCounter, string? lastHash, bool ignoreWarnings);
    }
}
