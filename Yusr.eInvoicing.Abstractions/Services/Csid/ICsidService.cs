using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;

namespace Yusr.eInvoicing.Abstractions.Services.Csid
{
    internal interface ICsidService
    {
        Task<OperationResult<ICsidResult?>> TryRequestComplianceCsidAsync(string otp, ICsrResult csrResult, bool Production);
        Task<OperationResult<ICsidResult?>> TryRequestProductionCsidAsync(ICsidResult csidResponse, bool Production);
    }
}
