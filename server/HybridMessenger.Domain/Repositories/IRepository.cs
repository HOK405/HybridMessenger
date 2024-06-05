namespace HybridMessenger.Domain.Repositories
{
    /// <summary>
    /// Generic repository
    /// </summary>
    /// <typeparam name="T">T is a type of repository entity</typeparam>
    /// <typeparam name="TKey">TKey is a type of entity Id</typeparam>
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);

        Task<T> AddAsync(T entity);

        void Update(T entity);

        void Remove(T entity);

        Task<IQueryable<T>> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            string sortBy,
            Dictionary<string, object> filters,
            string searchValue,          
            bool ascending);
    }
}
