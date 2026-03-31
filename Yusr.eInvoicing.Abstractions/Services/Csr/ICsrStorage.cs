using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities;
using Yusr.Identity.Abstractions.Primitives;

namespace Yusr.eInvoicing.Abstractions.Services.Csr
{
    internal interface ICsrStorage
    {
        Task<OperationResult<bool>> StoreCsr(JwtClaims jwtClaims, ICsrResult csrResult);
    }
}
