using System;
using System.Linq;
using System.Threading.Tasks;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
using AutoMapper;

namespace MediPlat.Service.Services
{
    public class AppointmentSlotMedicineService : IAppointmentSlotMedicineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentSlotMedicineService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IQueryable<AppointmentSlotMedicineResponse> GetAllAppointmentSlotMedicines()
        {
            return _unitOfWork.AppointmentSlotMedicines.GetAll().AsQueryable()
                .Select(m => _mapper.Map<AppointmentSlotMedicineResponse>(m));
        }

        public async Task<AppointmentSlotMedicineResponse?> GetAppointmentSlotMedicineByIdAsync(Guid appointmentSlotId, Guid medicineId, Guid patientId)
        {
            var entity = await _unitOfWork.AppointmentSlotMedicines.GetAsync(m => m.AppointmentSlotId == appointmentSlotId && m.MedicineId == medicineId && m.PatientId == patientId);
            return entity != null ? _mapper.Map<AppointmentSlotMedicineResponse>(entity) : null;
        }
        public async Task AddAppointmentSlotMedicineAsync(AppointmentSlotMedicineRequest request)
        {
            var exists = await _unitOfWork.AppointmentSlotMedicines.GetAsync(m =>
                m.AppointmentSlotId == request.AppointmentSlotId &&
                m.MedicineId == request.MedicineId);

            if (exists != null)
            {
                throw new InvalidOperationException("This medicine is already added to the appointment slot.");
            }

            var entity = _mapper.Map<AppointmentSlotMedicine>(request);
            _unitOfWork.AppointmentSlotMedicines.Add(entity);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateAppointmentSlotMedicineAsync(Guid appointmentSlotId, Guid medicineId, Guid patientId, AppointmentSlotMedicineRequest request)
        {
            var entity = await _unitOfWork.AppointmentSlotMedicines.GetAsync(m =>
                m.AppointmentSlotId == appointmentSlotId && m.MedicineId == medicineId);

            if (entity == null)
            {
                throw new KeyNotFoundException("AppointmentSlotMedicine not found.");
            }

            entity.Dosage = request.Dosage;
            entity.Instructions = request.Instructions;
            entity.Quantity = request.Quantity;

            await _unitOfWork.AppointmentSlotMedicines.UpdatePartialAsync(entity, e => e.Dosage, e => e.Instructions, e => e.Quantity);
        }

        public async Task DeleteAppointmentSlotMedicineAsync(Guid appointmentSlotId, Guid medicineId, Guid patientId)
        {
            var entity = await _unitOfWork.AppointmentSlotMedicines.GetAsync(m => m.AppointmentSlotId == appointmentSlotId && m.MedicineId == medicineId);
            if (entity != null)
            {
                _unitOfWork.AppointmentSlotMedicines.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
