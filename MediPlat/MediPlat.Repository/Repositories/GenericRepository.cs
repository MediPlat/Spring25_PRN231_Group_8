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

        public IEnumerable<T> GetList(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _mediPlatDBContext.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.Where(predicate).ToList();
        }

        public async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _mediPlatDBContext.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.Where(predicate).ToListAsync();
        }


        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _mediPlatDBContext.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _mediPlatDBContext.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.ToListAsync();
        }


        public int Count()
        {
            return _mediPlatContext.Set<T>().Count();
        }

        public async Task<int> CountAsync()
        {
            return await _mediPlatContext.Set<T>().CountAsync();
        }

        public void Update(T objModel, params Expression<Func<T, object>>[] includeProperties)
        {
            var entry = _mediPlatDBContext.Entry(objModel);
            entry.State = EntityState.Modified;

            foreach (var includeProperty in includeProperties)
            {
                var property = entry.Property(includeProperty);
                if (!property.IsModified)
                {
                    property.IsModified = true;
                }
            }

            _mediPlatDBContext.SaveChanges();
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
