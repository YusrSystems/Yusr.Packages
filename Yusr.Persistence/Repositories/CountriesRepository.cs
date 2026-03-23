using Microsoft.EntityFrameworkCore;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Extensions;
using Yusr.Core.Abstractions.Interfaces;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Persistence.Context;

namespace Yusr.Persistence.Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly YusrDbContext _context;

        public CountriesRepository(YusrDbContext context)
        {
            _context = context;
        }

        public virtual async Task<FilterResponse<Country>> FilterAsync(int pageNumber, int rowsPerPage, FilterCondition? condition)
        {
            var query = _context.Countries
                .AsNoTracking()
                .ApplyDynamicFilter(condition)
                .OrderBy(t => t.Id);

            var totalCount = await query.CountAsync();

            var data = await query
                .ToListAsync();

            return new FilterResponse<Country>
            {
                Data = data,
                Count = totalCount
            };
        }
    }
}
