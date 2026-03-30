using Microsoft.EntityFrameworkCore;
using Yusr.Core.Abstractions.Models;
using Yusr.Core.Abstractions.Services;
using Yusr.Persistence.Context;

namespace Yusr.Persistence.Services
{
    public class ReportContextService(YusrDbContext context) : IReportContextService
    {
        protected readonly YusrDbContext _context = context;

        public async Task<ReportContext?> GetContextAsync(long userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new ReportContext(
                    u.Id,
                    u.Username,
                    u.TenantId,
                    u.Tenant.Name,
                    u.Tenant.Phone,
                    u.Tenant.Logo,
                    u.Branch,
                    DateTime.UtcNow
                ))
                .SingleOrDefaultAsync(cancellationToken);

            return user;
        }
    }
}
