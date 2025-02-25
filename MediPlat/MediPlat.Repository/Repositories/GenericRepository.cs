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
        protected readonly DbSet<T> _dbSet;
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
        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _mediPlatDBContext.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public IQueryable<T> GetList(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _mediPlatDBContext.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.Where(predicate);
        }
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return await GetList(predicate, includeProperties).ToListAsync(); // ✅ Thực thi truy vấn async
        }

        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _mediPlatDBContext.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public IQueryable<T> GetAllAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            return GetAll(includeProperties);
        }

        public int Count()
        {
            return _mediPlatDBContext.Set<T>().Count();
        }

        public async Task<int> CountAsync()
        {
            return await _mediPlatDBContext.Set<T>().CountAsync();
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
        }

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

        public void UpdatePartial(T objModel, params Expression<Func<T, object>>[] updatedProperties)
        {
            var entry = _mediPlatDBContext.Entry(objModel);
            entry.State = EntityState.Unchanged;

            foreach (var property in updatedProperties)
            {
                entry.Property(property).IsModified = true;
            }
        }
        public async Task UpdatePartialAsync(T objModel, params Expression<Func<T, object>>[] updatedProperties)
        {
            var entry = _mediPlatDBContext.Entry(objModel);
            if (entry.State == EntityState.Detached)
            {
                _mediPlatDBContext.Set<T>().Attach(objModel);
            }

            foreach (var property in updatedProperties)
            {
                var propertyEntry = entry.Property(property);
                if (!propertyEntry.IsModified)
                {
                    propertyEntry.IsModified = true;
                }
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
