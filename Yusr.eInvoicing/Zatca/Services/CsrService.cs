using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Services.Csr;
using Yusr.eInvoicing.Abstractions.Services.Entities;
using ZATCA.EInvoice.SDK;
using ZATCA.EInvoice.SDK.Contracts.Models;

namespace Yusr.Infrastructure.eInvoicing.Zatca.Services
{
    public class CsrService : ICsrService<ZatcaCsrResult>
    {
        public async Task<OperationResult<ZatcaCsrResult>> TryGenerateCsrAsync(Tenant tenant, Branch branch, bool Production)
        {
            CsrGenerationDto dto = new CsrGenerationDto(
                $"yusrsys-{Guid.NewGuid().ToString()}-{tenant.VatNumber}",
                $"1-{Guid.NewGuid().ToString()}|2-{Guid.NewGuid().ToString()}|3-{Guid.NewGuid().ToString()}",
                tenant.VatNumber,
                branch.Name,
                tenant.Name,
                "SA",
                "1100", // 1100 = Standard + Simplified, 1000 = Standard only, 0100 = Simplified only
                branch.City?.Country?.Name + " - " + branch.City?.Name,
                tenant.CompanyBusinessCategory
            );

            CsrGenerator gen = new CsrGenerator();
            CsrResult csrResult = gen.GenerateCsr(dto, Production ? EnvironmentType.Production : EnvironmentType.Simulation, false);

            if (!csrResult.IsValid)
                return OperationResult<ZatcaCsrResult>.InternalError("لم يتم اصدار الرقمية بنجاح", csrResult.ErrorMessages[0]);

            return OperationResult<ZatcaCsrResult>.Ok(new ZatcaCsrResult(csrResult));
        }
    }
}
