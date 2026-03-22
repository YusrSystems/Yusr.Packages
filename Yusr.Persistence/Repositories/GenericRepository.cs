using Microsoft.EntityFrameworkCore;
using Yusr.Core.Abstractions.Entities;
using Yusr.Core.Abstractions.Extensions;
using Yusr.Core.Abstractions.Interfaces.Generics;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Persistence.Context;

namespace Yusr.Persistence.Repositories
{
    public class GenericRepository<TEntity>(YusrDbContext context) : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly YusrDbContext _context = context;
        protected DbSet<TEntity> DbSet => _context.Set<TEntity>();

        public virtual async Task<FilterResponse<TEntity>> FilterAsync(int pageNumber, int rowsPerPage, FilterCondition? condition)
        {
            var query = DbSet
                .AsNoTracking()
                .ApplyDynamicFilter(condition);

            var totalCount = await query.CountAsync();

            var data = await query
                 .OrderByDescending(x => x.Id)
                 .Paginate(pageNumber, rowsPerPage)
                 .ToListAsync();

            return new FilterResponse<TEntity>
            {
                Data = data,
                Count = totalCount
            };
        }

        public virtual async Task<TEntity?> GetByIdAsync(long id, bool track = false)
        {
            var query = DbSet.AsQueryable();

            if (!track)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<List<TEntity>> GetByIdsAsync(List<long> ids, bool track = false)
        {
            if (ids.Count == 0)
                return [];

            var query = DbSet.AsQueryable();

            if (!track)
                query = query.AsNoTracking();

            return await query
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }

        public virtual async Task<TEntity?> FindAsync(long id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            DbSet.Update(entity);
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            DbSet.UpdateRange(entities);
        }

        public virtual async Task<int> DeleteAsync(long id)
        {
            return await DbSet.Where(e => e.Id == id).ExecuteDeleteAsync();
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }
    }
}