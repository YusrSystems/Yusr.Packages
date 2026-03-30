using Yusr.Core.Abstractions.Entities;

namespace Yusr.Core.Abstractions.Models
{
    public record ReportContext(
        long UserId,
        string Username,
        long TenantId,
        string TenantEmail,
        string TenantName,
        string TenantPhone,
        string? TenantBusinessCategory,
        string? TenantCrn,
        string? TenantVatNumber,
        string? TenantLogo,
        Currency? Currency,
        Branch Branch,
        DateTime GeneratedAt
    );
}
