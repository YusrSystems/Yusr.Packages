using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;
using Yusr.eInvoicing.Abstractions.Services.Csr;
using Yusr.eInvoicing.Abstractions.Services.Entities;
using ZATCA.EInvoice.SDK;
using ZATCA.EInvoice.SDK.Contracts.Models;

namespace Yusr.Infrastructure.eInvoicing.Zatca.Services
{
    public class CsrService : ICsrService<ZatcaCsrResult>
    {
        public async Task<OperationResult<ZatcaCsrResult>> TryGenerateCsrAsync(IEInvoicingSetting eInvoicingSetting, EInvoicingEnvironmentType type)
        {
            CsrGenerationDto dto = new CsrGenerationDto(
                $"yusrsys-{Guid.NewGuid().ToString()}-{eInvoicingSetting.Tenant.VatNumber}",
                $"1-{Guid.NewGuid().ToString()}|2-{Guid.NewGuid().ToString()}|3-{Guid.NewGuid().ToString()}",
                eInvoicingSetting.Tenant.VatNumber,
                eInvoicingSetting.Branch.Name,
                eInvoicingSetting.Tenant.Name,
                "SA",
                "1100", // 1100 = Standard + Simplified, 1000 = Standard only, 0100 = Simplified only
                eInvoicingSetting.Branch.City?.Country?.Name + " - " + eInvoicingSetting.Branch.City?.Name,
                eInvoicingSetting.Tenant.CompanyBusinessCategory
            );

            CsrGenerator gen = new CsrGenerator();
            CsrResult csrResult = gen.GenerateCsr(dto, (EnvironmentType)type, false);

            if (!csrResult.IsValid)
                return OperationResult<ZatcaCsrResult>.InternalError("لم يتم اصدار الرقمية بنجاح", csrResult.ErrorMessages[0]);

            return OperationResult<ZatcaCsrResult>.Ok(new ZatcaCsrResult(csrResult));
        }
    }
}