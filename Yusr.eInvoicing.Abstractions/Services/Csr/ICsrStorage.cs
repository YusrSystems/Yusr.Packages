using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;

namespace Yusr.eInvoicing.Abstractions.Services.Csr
{
    public interface ICsrStorage
    {
        Task<OperationResult<bool>> StoreCsr(ICsrResult csrResult);
    }
}