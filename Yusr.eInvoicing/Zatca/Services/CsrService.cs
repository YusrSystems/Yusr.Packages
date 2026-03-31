using Microsoft.EntityFrameworkCore;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Enums;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Identity.Abstractions.Primitives;
using Yusr.Infrastructure.Persistence.Context;
using ZATCA.EInvoice.SDK;
using ZATCA.EInvoice.SDK.Contracts.Models;

namespace Yusr.Infrastructure.eInvoicing.Zatca.Services
{
    public class CsrService(YusrErpDbContext context)
    {
        private readonly YusrErpDbContext _context = context;

        public async Task<(bool IsValid, string? ErrorMessage, CsrResult? csrResult)> TryGenerateCsr(JwtClaims jwtClaims, Tenant tenant, Branch branch, bool Production)
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
            {
                return (false, csrResult.ErrorMessages[0] ?? string.Empty, null);
            }

            OperationResult<bool> storeResult = await StoreCsr(jwtClaims, csrResult);

            if (storeResult.ResultType != ResultType.Ok)
            {
                return (false, storeResult.ErrorMessage, null);
            }

            return (true, null, csrResult);
        }

        public async Task<OperationResult<bool>> StoreCsr(JwtClaims jwtClaims, CsrResult csrResult)
        {
            try
            {
                var setting = await _context.Settings.FirstOrDefaultAsync();
                if (setting == null)
                    return OperationResult<bool>.NotFound();

                setting.UpdateCsr(csrResult.Csr, csrResult.PrivateKey);
                await _context.SaveChangesAsync();
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.InternalError("Could not save Csr", ex.Message);
            }
        }
    }
}
