using System;
using System.Threading.Tasks;
using MediPlat.Model.Model;
using MediPlat.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace MediPlat.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MediPlatContext _context;
        private IDbContextTransaction _transaction;
        private readonly ILogger<UnitOfWork> _logger;

        public IGenericRepository<DoctorSubscription> DoctorSubscriptions { get; }
        public IGenericRepository<Subscription> Subscriptions { get; }
        public IGenericRepository<Patient> Patients { get; }
        public IGenericRepository<Doctor> Doctors { get; }
        public IGenericRepository<Experience> Experiences { get; }
        public IGenericRepository<AppointmentSlotMedicine> AppointmentSlotMedicines { get; }
        public IGenericRepository<Medicine> Medicines { get; }
        public IGenericRepository<AppointmentSlot> AppointmentsSlots { get; }
        public IGenericRepository<Slot> Slots { get; }
        public IGenericRepository<AppointmentSlot> AppointmentSlot { get; }

        public UnitOfWork(MediPlatContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
            DoctorSubscriptions = new GenericRepository<DoctorSubscription>(context);
            Subscriptions = new GenericRepository<Subscription>(context);
            Patients = new GenericRepository<Patient>(context);
            Doctors = new GenericRepository<Doctor>(context);
            Experiences = new GenericRepository<Experience>(context);
            AppointmentSlotMedicines = new GenericRepository<AppointmentSlotMedicine>(context);
            Medicines = new GenericRepository<Medicine>(context);
            Slots = new GenericRepository<Slot>(context);
            AppointmentSlot = new GenericRepository<AppointmentSlot>(context);
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
            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                _logger.LogError(ex, "Transaction commit failed. Rolling back...");
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
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
