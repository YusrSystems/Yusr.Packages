using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;

namespace Yusr.eInvoicing.Abstractions.Services.Csid
{
    public interface ICsidService<TCsidResult, TCsrResult> where TCsidResult : ICsidResult where TCsrResult : ICsrResult
    {
        Task<OperationResult<TCsidResult>> TryRequestComplianceCsidAsync(string otp, TCsrResult csrResult, bool Production);
        Task<OperationResult<TCsidResult>> TryRequestProductionCsidAsync(TCsidResult csidResponse, bool Production);
    }
}
