using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.Identity.Abstractions.Primitives;

namespace Yusr.eInvoicing.Abstractions.Services.Csid
{
    public interface ICsidStorage
    {
        Task<OperationResult<bool>> StoreCsid(JwtClaims jwtClaims, ICsidResult csid, string certificateContent, bool production);
    }
}
