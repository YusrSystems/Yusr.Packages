using Microsoft.EntityFrameworkCore;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Extensions;
using Yusr.Core.Abstractions.Interfaces;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Persistence.Context;

namespace Yusr.Persistence.Repositories
{
    public class CitiesRepository : ICitiesRepository
    {
        private readonly YusrDbContext _context;

        public CitiesRepository(YusrDbContext context)
        {
            _context = context;
        }

        public virtual async Task<FilterResponse<City>> FilterAsync(int pageNumber, int rowsPerPage, FilterCondition? condition)
        {
            var query = _context.Cities
                .AsNoTracking()
                .ApplyDynamicFilter(condition)
                .Include(c => c.Country)
                .OrderBy(t => t.Id);

            var totalCount = await query.CountAsync();

            var data = await query
                .ToListAsync();

            return new FilterResponse<City>
            {
                Data = data,
                Count = totalCount
            };
        }
    }
}
