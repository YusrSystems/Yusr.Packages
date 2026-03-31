using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;
using Yusr.Identity.Abstractions.Primitives;

namespace Yusr.eInvoicing.Abstractions.Services.Api
{
    public interface IComplianceCheckService
    {
        Task<OperationResult<bool>> GenerateFullCheck(JwtClaims jwtClaims, Tenant tenant, IEInvoicingSetting setting, Branch branch, bool Production = false);
        Task<OperationResult<bool>> SendInvoice(JwtClaims jwtClaims, Tenant tenant, IEInvoicingSetting setting, Branch branch, EInvoiceType type, bool simplified, bool Production = false);
    }
}
