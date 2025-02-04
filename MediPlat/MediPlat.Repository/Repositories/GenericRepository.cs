using MediPlat.Model;
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
        private MediPlatContext _mediPlatContext;
        public GenericRepository(MediPlatContext mediPlatContext) 
        {
            _mediPlatContext = mediPlatContext;
        }


        public void Add(T model)
        {
            _mediPlatContext.Set<T>().Add(model);
            _mediPlatContext.SaveChanges();
        }

        public void AddRange(IEnumerable<T> model)
        {
            _mediPlatContext.Set<T>().AddRange(model);
            _mediPlatContext.SaveChanges();
        }

        public T? GetId(Guid id)
        {
            return _mediPlatContext.Set<T>().Find(id);
        }

        public async Task<T?> GetIdAsync(Guid id)
        {
            return await _mediPlatContext.Set<T>().FindAsync(id);
        }

        public T? Get(Expression<Func<T, bool>> predicate)
        {
            return _mediPlatContext.Set<T>().FirstOrDefault(predicate);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _mediPlatContext.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public IEnumerable<T> GetList(Expression<Func<T, bool>> predicate)
        {
            return _mediPlatContext.Set<T>().Where<T>(predicate).ToList();
        }

        public async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => _mediPlatContext.Set<T>().Where<T>(predicate));
        }

        public IEnumerable<T> GetAll()
        {
            return _mediPlatContext.Set<T>().ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.Run(() => _mediPlatContext.Set<T>());
        }

        public int Count()
        {
            return _mediPlatContext.Set<T>().Count();
        }

        public async Task<int> CountAsync()
        {
            return await _mediPlatContext.Set<T>().CountAsync();
        }

        public void Update(T objModel)
        {
            _mediPlatContext.Entry(objModel).State = EntityState.Modified;
            _mediPlatContext.SaveChanges();
        }

        public void Remove(T objModel)
        {
            _mediPlatContext.Set<T>().Remove(objModel);
            _mediPlatContext.SaveChanges();
        }

        public void Dispose()
        {
            _mediPlatContext.Dispose();
        }
    }
}
