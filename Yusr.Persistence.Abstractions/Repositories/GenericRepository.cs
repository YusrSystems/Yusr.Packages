using Microsoft.EntityFrameworkCore;
using Yusr.Bus.Core.Entities;
using Yusr.Bus.Core.Extensions;
using Yusr.Bus.Core.Interfaces.Repositories.Generics;
using Yusr.Bus.Core.Primitives;
using Yusr.Bus.Persistence.Context;

namespace Yusr.Persistence.Abstractions.Repositories
{
    public class GenericRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly YusrDbContext _context;
        protected DbSet<TEntity> _dbSet => _context.Set<TEntity>();

        public GenericRepository(YusrDbContext context)
        {
            _context = context;
        }

        public virtual async Task<FilterResponse<TEntity>> FilterAsync(int pageNumber, int rowsPerPage, FilterCondition? condition)
        {
            var query = _dbSet
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
            var query = _dbSet.AsQueryable();

            if (!track)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<List<TEntity>> GetByIdsAsync(List<long> ids, bool track = false)
        {
            if (!ids.Any())
                return new List<TEntity>();

            var query = _dbSet.AsQueryable();

            if (!track)
                query = query.AsNoTracking();

            return await query
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }

        public virtual async Task<TEntity?> FindAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public virtual async Task<int> DeleteAsync(long id)
        {
            return await _dbSet.Where(e => e.Id == id).ExecuteDeleteAsync();
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
