using Yusr.Core.Abstractions.Entities;

namespace Yusr.Core.Abstractions.Models
{
    public record ReportContext(
        long UserId,
        string Username,
        long TenantId,
        string TenantName,
        string TenantPhone,
        string? TenantLogo,
        Branch Branch,
        DateTime GeneratedAt
    );
}
