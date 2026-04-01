using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;

namespace Yusr.eInvoicing.Abstractions.Services.Csid
{
    public interface ICsidStorage
    {
        Task<OperationResult<bool>> StoreCsid(ICsidResult csid, string certificateContent, bool production);
    }
}