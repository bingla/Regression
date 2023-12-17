namespace Regression.Data.Interfaces
{
    public interface IGeneralRepository<T> where T : class
    {
        IQueryable<T> Get(Func<IQueryable<T>, IQueryable<T>> func);
        Task<T?> GetAsync(object Id);
        Task<T> CreateAsync(T entity);
        Task CreateAsync(IEnumerable<T> entities);
        Task<T?> UpdateAsync(T entity);
        Task<T?> DeleteAsync(T entity);
    }
}
