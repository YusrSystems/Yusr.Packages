using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Services.Csr
{
    public interface ICsrService<TCsrResult> where TCsrResult : ICsrResult
    {
        Task<OperationResult<TCsrResult>> TryGenerateCsrAsync(IEInvoicingSetting eInvoicingSetting, EInvoicingEnvironmentType type);
    }
}