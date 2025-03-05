using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Repository.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T objModel);
        void AddRange(IEnumerable<T> objModel);
        T? GetId(Guid id);
        Task<T?> GetIdAsync(Guid id);
        T? Get(Expression<Func<T, bool>> predicate);
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeProperties);
        Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
        int Count();
        Task<int> CountAsync();
        void Update(T objModel, params Expression<Func<T, object>>[] includeProperties);
        Task UpdateAsync(T objModel, params Expression<Func<T, object>>[] includeProperties);
        void UpdatePartial(T objModel, params Expression<Func<T, object>>[] updatedProperties);
        Task UpdatePartialAsync(T objModel, params Expression<Func<T, object>>[] updatedProperties);
        void Remove(T objModel);
        void Dispose();
    }
}