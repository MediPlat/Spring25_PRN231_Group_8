using System;
using System.Threading.Tasks;
using MediPlat.Model.Model;
using MediPlat.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MediPlat.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MediPlatContext _context;
        private IDbContextTransaction _transaction;

        public IGenericRepository<DoctorSubscription> DoctorSubscriptions { get; }
        public IGenericRepository<Subscription> Subscriptions { get; }
        public IGenericRepository<Patient> Patients { get; }
        public IGenericRepository<Doctor> Doctors { get; }


        public UnitOfWork(MediPlatContext context)
        {
            _context = context;
            DoctorSubscriptions = new GenericRepository<DoctorSubscription>(context);
            Subscriptions = new GenericRepository<Subscription>(context);
            Patients = new GenericRepository<Patient>(context);
            Doctors = new GenericRepository<Doctor>(context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }
        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
