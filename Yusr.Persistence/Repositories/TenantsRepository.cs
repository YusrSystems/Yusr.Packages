using Microsoft.EntityFrameworkCore;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Interfaces;
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
                .Include(t => t.Currency)
                .Where(c => c.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<Tenant?> GetTenantByRegistrationKeyAsync(string registrationKey)
        {
            return await _context.Tenants
                .AsNoTracking()
                .Include(t => t.Currency)
                .Where(c => c.RegistrationKey == registrationKey)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CheckEmailAvailability(string email)
        {
            var normalizedUsername = email.Trim().ToLower();
            return await _context.Tenants.AnyAsync(c => c.Email.ToLower().Equals(normalizedUsername));
        }
    }
}
