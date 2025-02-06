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
        IEnumerable<T> GetList(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
        int Count();
        Task<int> CountAsync();
        void Update(T objModel, params Expression<Func<T, object>>[] includeProperties);
        void Remove(T objModel);
        Task DeleteAsync(Guid id);
        void Dispose();
    }
}