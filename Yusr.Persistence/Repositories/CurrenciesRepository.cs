using Microsoft.EntityFrameworkCore;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Extensions;
using Yusr.Core.Abstractions.Interfaces;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Persistence.Context;

namespace Yusr.Persistence.Repositories
{
    public class CurrenciesRepository : ICurrenciesRepository
    {
        private readonly YusrDbContext _context;

        public CurrenciesRepository(YusrDbContext context)
        {
            _context = context;
        }

        public virtual async Task<FilterResponse<Currency>> FilterAsync(int pageNumber, int rowsPerPage, FilterCondition? condition)
        {
            var query = _context.Currencies
                .AsNoTracking()
                .ApplyDynamicFilter(condition)
                .OrderBy(t => t.Id);

            var totalCount = await query.CountAsync();

            var data = await query
                .ToListAsync();

            return new FilterResponse<Currency>
            {
                Data = data,
                Count = totalCount
            };
        }
    }
}
