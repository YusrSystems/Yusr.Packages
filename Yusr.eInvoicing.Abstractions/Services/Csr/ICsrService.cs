using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Identity.Abstractions.Primitives;

namespace Yusr.eInvoicing.Abstractions.Services.Csr
{
    internal interface ICsrService
    {
        Task<OperationResult<ICsrService>> TryGenerateCsr(JwtClaims jwtClaims, Tenant tenant, Branch branch, bool Production);
    }
}