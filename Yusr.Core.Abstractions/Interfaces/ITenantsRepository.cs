using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Interfaces.Generics;

namespace Yusr.Core.Abstractions.Interfaces
{
    public interface ITenantsRepository : IBaseRepository<Tenant>
    {
        Task<Tenant?> GetTenantByEmailAsync(string email);
    }
}
