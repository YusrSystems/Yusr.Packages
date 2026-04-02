using Yusr.Core.Abstractions.Primitives;
using Yusr.eInvoicing.Abstractions.Entities.Interfaces;
using Yusr.eInvoicing.Abstractions.Enums;
using Yusr.eInvoicing.Abstractions.Services.Csr;
using Yusr.eInvoicing.Zatca.Entities;
using ZATCA.EInvoice.SDK;
using ZATCA.EInvoice.SDK.Contracts.Models;

namespace Yusr.eInvoicing.Zatca.Services.Csr
{
    public class CsrService : ICsrService<ZatcaCsrResult>
    {
        public async Task<OperationResult<ZatcaCsrResult>> TryGenerateCsrAsync(IEInvoiceSetting eInvoicingSetting, EInvoicingEnvironmentType type)
        {
            CsrGenerationDto dto = new(
                $"yusrsys-{Guid.NewGuid()}-{eInvoicingSetting.Tenant.VatNumber}",
                $"1-{Guid.NewGuid()}|2-{Guid.NewGuid()}|3-{Guid.NewGuid()}",
                eInvoicingSetting.Tenant.VatNumber,
                eInvoicingSetting.Branch.Name,
                eInvoicingSetting.Tenant.Name,
                "SA",
                "1100", // 1100 = Standard + Simplified, 1000 = Standard only, 0100 = Simplified only
                eInvoicingSetting.Branch.City?.Country?.Name + " - " + eInvoicingSetting.Branch.City?.Name,
                eInvoicingSetting.Tenant.CompanyBusinessCategory
            );

            CsrGenerator gen = new();
            CsrResult csrResult = gen.GenerateCsr(dto, (EnvironmentType)type, false);

            if (!csrResult.IsValid)
                return OperationResult<ZatcaCsrResult>.InternalError("لم يتم اصدار الرقمية بنجاح", csrResult.ErrorMessages[0]);

            return OperationResult<ZatcaCsrResult>.Ok(new ZatcaCsrResult(csrResult));
        }
    }
}