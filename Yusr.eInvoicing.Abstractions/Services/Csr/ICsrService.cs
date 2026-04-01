using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;

namespace Yusr.eInvoicing.Abstractions.Services.Csr
{
    public interface ICsrService<TCsrResult> where TCsrResult : ICsrResult
    {
        Task<OperationResult<TCsrResult>> TryGenerateCsrAsync(Tenant tenant, Branch branch, bool Production);
    }
}