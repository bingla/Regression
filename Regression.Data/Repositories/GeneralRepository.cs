using Microsoft.EntityFrameworkCore;
using Regression.Data.Interfaces;
using Regression.Domain;

namespace Regression.Data.Repositories
{
    public class GeneralRepository<T> : IGeneralRepository<T> where T : class
    {
        protected readonly RegressionContext _context;
        protected readonly DbSet<T> _dbSet;

        public GeneralRepository(RegressionContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> Get(Func<IQueryable<T>, IQueryable<T>> func)
        {
            return func.Invoke(_dbSet.AsQueryable());
        }

        public async Task<T?> GetAsync(object Id)
        {
            return await _dbSet.FindAsync(Id);
        }

        public async Task<T> CreateAsync(T entity)
        {
            var tracked = await _dbSet.AddAsync(entity);
            _ = await _context.SaveChangesAsync();
            return tracked.Entity;
        }

        public async Task CreateAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            _ = await _context.SaveChangesAsync();
            return;
        }

        public async Task<T?> UpdateAsync(T entity)
        {
            var tracker = _dbSet.Update(entity);
            _ = await _context.SaveChangesAsync();
            return tracker.Entity ?? default;
        }

        public async Task<T?> DeleteAsync(T entity)
        {
            var tracker = _dbSet.Remove(entity);
            _ = await _context.SaveChangesAsync();
            return tracker.Entity ?? default;
        }
    }
}
