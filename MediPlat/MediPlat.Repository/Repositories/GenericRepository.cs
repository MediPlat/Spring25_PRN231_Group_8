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
            _dbSet = _mediPlatDBContext.Set<T>();
        }

        public void Add(T model) => _dbSet.Add(model);

        public void AddRange(IEnumerable<T> model) => _dbSet.AddRange(model);
        public async Task AddAsync(T model)
        {
            await _dbSet.AddAsync(model);
        }

        public T? GetId(Guid id) => _dbSet.Find(id);

        public async Task<T?> GetIdAsync(Guid id) => await _dbSet.FindAsync(id);

        public T? Get(Expression<Func<T, bool>> predicate) => _dbSet.FirstOrDefault(predicate);

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate) => await _dbSet.FirstOrDefaultAsync(predicate);

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.Where(predicate).ToListAsync();
        }

        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.ToListAsync();
        }

        public int Count() => _dbSet.Count();

        public async Task<int> CountAsync() => await _dbSet.CountAsync();

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
                _dbSet.Attach(objModel);
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

        public void Remove(T objModel) => _dbSet.Remove(objModel);

        public void Dispose() => _mediPlatDBContext.Dispose();
    }
}
