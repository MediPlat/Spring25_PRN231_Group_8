using Microsoft.EntityFrameworkCore;
using Nursing_Service_Platform.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nursing_Service_Platform.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private DBContext _context;

        public GenericRepository(DBContext dBContext)
        {
            _context = dBContext;
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T? GetById(object id)
        {
            return _context.Set<T>().Find(id);
        }

        public void Insert(T obj)
        {
            _context.Set<T>().Add(obj);
        }

        public void Update(T obj)
        {
            _context.Set<T>().Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }

        public void Delete(object id)
        {
            T? existing = _context.Set<T>().Find(id);
            if (existing is not null)
            {
                _context.Set<T>().Remove(existing);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
}
