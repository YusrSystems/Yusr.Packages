using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Dto;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;

namespace Yusr.eInvoicing.Abstractions.Services.Initialization
{
    public interface IEInvoiceInitializationService
    {
        OperationResult<EInvoicePrepareDto?> Init(IEInvoiceSetting setting, IEInvoiceInvoice invoice, IEInvoiceAccount actionAccount, List<IEInvoiceItem> dbItems, long? lastCounter, string? lastHash, bool ignoreWarnings);
    }
}
