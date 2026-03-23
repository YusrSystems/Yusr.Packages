using Microsoft.EntityFrameworkCore;
using Yusr.Core.Abstractions.Entities;
using Yusr.Persistence.Context;

namespace Yusr.Persistence.Repositories
{
    public class TenantsRepository : GenericRepository<Tenant>, ITenantsRepository
    {
        public TenantsRepository(YusrDbContext context) : base(context)
        {
        }

        public async Task<Tenant?> GetTenantByEmailAsync(string email)
        {
            return await _context.Tenants
                .AsNoTracking()
                .Where(c => c.Email == email)
                .FirstOrDefaultAsync();
        }
    }
}
