using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Services.Csid
{
    public interface ICsidService<TCsidResult, TCsrResult> where TCsidResult : ICsidResult where TCsrResult : ICsrResult
    {
        Task<OperationResult<TCsidResult>> TryRequestComplianceCsidAsync(string otp, TCsrResult csrResult, EInvoicingEnvironmentType type);
        Task<OperationResult<TCsidResult>> TryRequestProductionCsidAsync(TCsidResult csidResponse, EInvoicingEnvironmentType type);
    }
}
