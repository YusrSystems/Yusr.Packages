using Microsoft.EntityFrameworkCore;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Extensions;
using Yusr.Core.Abstractions.Interfaces;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Persistence.Context;

namespace Yusr.Persistence.Repositories
{
    public class BranchesRepository(YusrDbContext context) : GenericRepository<Branch>(context), IBranchesRepository
    {
        public override async Task<FilterResponse<Branch>> FilterAsync(int pageNumber, int rowsPerPage, FilterCondition? condition)
        {
            var query = DbSet
                 .AsNoTracking()
                 .AsQueryable()
                 .ApplyDynamicFilter(condition)
                 .Include(x => x.City);

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Id)
                .Paginate(pageNumber, rowsPerPage)
                .ToListAsync();

            return new FilterResponse<Branch>
            {
                Data = data,
                Count = totalCount
            };
        }

        public override async Task<Branch?> GetByIdAsync(long id, bool track = false)
        {
            var query = DbSet
                .Include(b => b.City).ThenInclude(c => c!.Country)
                .AsQueryable();

            if (!track)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
