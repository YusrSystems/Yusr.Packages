using Microsoft.EntityFrameworkCore;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Extensions;
using Yusr.Core.Abstractions.Interfaces;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Persistence.Context;

namespace Yusr.Persistence.Repositories
{
    public class UsersRepository : GenericRepository<User>, IUsersRepository
    {
        public UsersRepository(YusrDbContext context) : base(context)
        {
        }

        public override async Task<FilterResponse<User>> FilterAsync(int pageNumber, int rowsPerPage, FilterCondition? condition)
        {
            var query = DbSet
                .AsNoTracking()
                .Include(u => u.Branch).ThenInclude(b => b.City)
                .ApplyDynamicFilter(condition);

            var totalCount = await query.CountAsync();

            var data = await query
                 .OrderByDescending(x => x.Id)
                 .Paginate(pageNumber, rowsPerPage)
                 .ToListAsync();

            return new FilterResponse<User>
            {
                Data = data,
                Count = totalCount
            };
        }

        public override async Task<User?> GetByIdAsync(long id, bool track = false)
        {
            var query = DbSet
                .Include(u => u.Branch).ThenInclude(b => b.City)
                .Include(u => u.Role)
                .AsQueryable();

            if (!track)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        // Check of hashing passwords will be in Application Layer
        public async Task<User?> GetUserByCredentialsAsync(string username)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Branch).ThenInclude(b => b.City)
                .Include(u => u.Role)
                .Where(c => c.Username == username)
                .FirstOrDefaultAsync();
        }
    }
}
