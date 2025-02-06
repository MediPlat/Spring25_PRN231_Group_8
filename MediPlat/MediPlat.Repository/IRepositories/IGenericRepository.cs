using System.Linq.Expressions;

namespace MediPlat.Repository.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T objModel);
        Task AddAsync(T objModel);
        void AddRange(IEnumerable<T> objModel);
        Task AddRangeAsync(IEnumerable<T> objModel);
        T? GetId(Guid id);
        Task<T?> GetIdAsync(Guid id);
        T? Get(Expression<Func<T, bool>> predicate);
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetList(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetAll();
        IQueryable<T> GetAllAsQueryable();
        Task<IEnumerable<T>> GetAllAsync();
        int Count();
        Task<int> CountAsync();
        void Update(T objModel);
        Task UpdateAsync(T objModel);
        void Remove(T objModel);
        Task DeleteAsync(Guid id);
        void Dispose();
    }
}