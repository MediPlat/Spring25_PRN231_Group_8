using MediPlat.Model;
using MediPlat.Repository.Entities;
using MediPlat.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private MediPlatContext _mediPlatDBContext;
        public GenericRepository(MediPlatContext mediPlatDBContext) 
        {
            _mediPlatDBContext = mediPlatDBContext;
        }


        public void Add(T model)
        {
            _mediPlatDBContext.Set<T>().Add(model);
            _mediPlatDBContext.SaveChanges();
        }

        public void AddRange(IEnumerable<T> model)
        {
            _mediPlatDBContext.Set<T>().AddRange(model);
            _mediPlatDBContext.SaveChanges();
        }

        public T? GetId(Guid id)
        {
            return _mediPlatDBContext.Set<T>().Find(id);
        }

        public async Task<T?> GetIdAsync(Guid id)
        {
            return await _mediPlatDBContext.Set<T>().FindAsync(id);
        }

        public T? Get(Expression<Func<T, bool>> predicate)
        {
            return _mediPlatDBContext.Set<T>().FirstOrDefault(predicate);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _mediPlatDBContext.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public IEnumerable<T> GetList(Expression<Func<T, bool>> predicate)
        {
            return _mediPlatDBContext.Set<T>().Where<T>(predicate).ToList();
        }

        public async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => _mediPlatDBContext.Set<T>().Where<T>(predicate));
        }

        public IEnumerable<T> GetAll()
        {
            return _mediPlatDBContext.Set<T>().ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.Run(() => _mediPlatDBContext.Set<T>());
        }

        public int Count()
        {
            return _mediPlatDBContext.Set<T>().Count();
        }

        public async Task<int> CountAsync()
        {
            return await _mediPlatDBContext.Set<T>().CountAsync();
        }

        public void Update(T objModel)
        {
            _mediPlatDBContext.Entry(objModel).State = EntityState.Modified;
            _mediPlatDBContext.SaveChanges();
        }

        public void Remove(T objModel)
        {
            _mediPlatDBContext.Set<T>().Remove(objModel);
            _mediPlatDBContext.SaveChanges();
        }

        public void Dispose()
        {
            _mediPlatDBContext.Dispose();
        }
    }
}
