using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Interfaces;
using Yusr.Persistence.Context;

namespace Yusr.Persistence.Repositories
{
    public class RolesRepository : GenericRepository<Role>, IRolesRepository
    {
        public RolesRepository(YusrDbContext context) : base(context)
        {
        }
    }
}