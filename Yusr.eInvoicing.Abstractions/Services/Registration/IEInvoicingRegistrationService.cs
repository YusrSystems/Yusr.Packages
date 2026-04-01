using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;

namespace Yusr.eInvoicing.Abstractions.Services.Registration
{
    public interface IEInvoicingRegistrationService
    {
        Task<OperationResult<bool>> Register(IEInvoicingSetting setting, string otp, EInvoicingEnvironmentType type);
    }
}