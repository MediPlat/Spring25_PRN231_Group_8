using MediPlat.Model.Model;
using MediPlat.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MediPlat.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly MediPlatContext _mediPlatDBContext;

        public GenericRepository(MediPlatContext mediPlatDBContext)
        {
            _mediPlatDBContext = mediPlatDBContext;
        }

        public void Add(T model)
        {
            _mediPlatDBContext.Set<T>().Add(model);
        }

        public void AddRange(IEnumerable<T> model)
        {
            _mediPlatDBContext.Set<T>().AddRange(model);
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
            return _mediPlatDBContext.Set<T>().Count();
        }

        public async Task<int> CountAsync()
        {
            return await _mediPlatDBContext.Set<T>().CountAsync();
        }

        // Cập nhật toàn bộ đối tượng với các thuộc tính cụ thể
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
        }

        // Cập nhật toàn bộ đối tượng (async)
        public async Task UpdateAsync(T objModel, params Expression<Func<T, object>>[] includeProperties)
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

            await _mediPlatDBContext.SaveChangesAsync();
        }

        // Cập nhật từng thuộc tính cụ thể (Patch-style updates)
        public void UpdatePartial(T objModel, params Expression<Func<T, object>>[] updatedProperties)
        {
            var entry = _mediPlatDBContext.Entry(objModel);
            entry.State = EntityState.Unchanged;

            foreach (var property in updatedProperties)
            {
                entry.Property(property).IsModified = true;
            }
        }

        // Cập nhật từng thuộc tính cụ thể (async)
        public async Task UpdatePartialAsync(T objModel, params Expression<Func<T, object>>[] updatedProperties)
        {
            var entry = _mediPlatDBContext.Entry(objModel);
            entry.State = EntityState.Unchanged;

            foreach (var property in updatedProperties)
            {
                entry.Property(property).IsModified = true;
            }

            await _mediPlatDBContext.SaveChangesAsync();
        }

        public void Remove(T objModel)
        {
            _mediPlatDBContext.Set<T>().Remove(objModel);
        }

        public void Dispose()
        {
            _mediPlatDBContext.Dispose();
        }
    }
}
