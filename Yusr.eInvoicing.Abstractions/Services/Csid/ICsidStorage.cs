using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Services.Csid
{
    public interface ICsidStorage
    {
        Task<OperationResult<bool>> StoreCsid(ICsidResult csid, string certificateContent, EInvoicingEnvironmentType type);
    }
}