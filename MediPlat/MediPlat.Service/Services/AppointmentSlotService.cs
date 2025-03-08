using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
using Microsoft.EntityFrameworkCore;

namespace MediPlat.Service.Services
{
    public class AppointmentSlotService : IAppointmentSlotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AppointmentSlotService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AppointmentSlotResponse?> CreateAppointmentSlot(AppointmentSlotRequest appointmentSlotRequest)
        {
            try
            {
                var appointmentSlot = _mapper.Map<AppointmentSlot>(appointmentSlotRequest);
                _unitOfWork.AppointmentSlots.Add(appointmentSlot);
                await _unitOfWork.SaveChangesAsync();

                // Tạo danh sách thuốc nếu có
                foreach (var medicine in appointmentSlotRequest.Medicines)
                {
                    var appointmentSlotMedicine = new AppointmentSlotMedicine
                    {
                        AppointmentSlotMedicineId = Guid.NewGuid(),
                        AppointmentSlotId = appointmentSlot.Id,
                        MedicineId = medicine.MedicineId,
                        Dosage = medicine.Dosage,
                        Instructions = medicine.Instructions,
                        Quantity = medicine.Quantity
                    };

                    _unitOfWork.AppointmentSlotMedicines.Add(appointmentSlotMedicine);
                }

                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<AppointmentSlotResponse>(appointmentSlot);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteAppointmentSlot(Guid appointmentSlotId)
        {
            var entity = await _unitOfWork.AppointmentSlots.GetAsync(am => am.Id == appointmentSlotId);
            if (entity == null)
            {
                throw new InvalidOperationException("Slot đã được kích hoạt, không thể xóa.");
            }

            _unitOfWork.AppointmentSlots.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public IQueryable<AppointmentSlotResponse> GetAppointmentSlot()
        {
            try
            {
                var appointmentSlots = _unitOfWork.AppointmentSlots.GetAll().ToList()
                    .Select(s => _mapper.Map<AppointmentSlotResponse>(s)).AsQueryable();
                return appointmentSlots;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new NotImplementedException();
        }

        public async Task<List<AppointmentSlotResponse>> GetAppointmentSlotsForDoctorAsync(Guid doctorId)
        {
            var slots = await _unitOfWork.AppointmentSlots
                .GetAll(s => s.Profile)
                .Where(s => s.Slot.DoctorId == doctorId)
                .Include(s => s.Slot.Doctor)
                .ToListAsync();

            return _mapper.Map<List<AppointmentSlotResponse>>(slots);
        }

        public async Task<List<AppointmentSlotResponse>> GetAppointmentSlotsForPatientAsync(Guid profileId)
        {
            var slots = await _unitOfWork.AppointmentSlots
                .GetAll(s => s.Profile, s => s.Slot.Doctor, s => s.AppointmentSlotMedicines)
                .Include(s => s.Slot.Doctor)
                .Include(s => s.AppointmentSlotMedicines)
                .ToListAsync();

            return _mapper.Map<List<AppointmentSlotResponse>>(slots);
        }

        public async Task<AppointmentSlotResponse> GetAppointmentSlotByIdForDoctorAsync(Guid doctorId, Guid appointmentSlotId)
        {
            var slot = await _unitOfWork.AppointmentSlots
                .GetAll(a => a.Slot, a => a.Profile, a => a.AppointmentSlotMedicines)
                .Include(a => a.AppointmentSlotMedicines)
                    .ThenInclude(m => m.Medicine)
                .Where(a => a.Id == appointmentSlotId && a.Slot.DoctorId == doctorId)
                .FirstOrDefaultAsync();

            return _mapper.Map<AppointmentSlotResponse>(slot);
        }

        public async Task<AppointmentSlotResponse> GetAppointmentSlotByIdForPatientAsync(Guid profileId, Guid appointmentSlotId)
        {
            var slot = await _unitOfWork.AppointmentSlots
                .GetAll(a => a.Profile,
                        a => a.AppointmentSlotMedicines,
                        a => a.Slot)
                .Include(a => a.AppointmentSlotMedicines)
                    .ThenInclude(asm => asm.Medicine)
                .Where(a => a.ProfileId == profileId && a.Id == appointmentSlotId)
                .FirstOrDefaultAsync();

            return slot != null ? _mapper.Map<AppointmentSlotResponse>(slot) : null;
        }


        public async Task<AppointmentSlotResponse> UpdateAppointmentSlot(Guid id, AppointmentSlotRequest appointmentSlotRequest)
        {
            try
            {
                var existingSlot = await _unitOfWork.AppointmentSlots.GetAsync(s => s.Id == id);
                if (existingSlot == null)
                {
                    return null;
                }

                _mapper.Map(appointmentSlotRequest, existingSlot);
                _unitOfWork.AppointmentSlots.Update(existingSlot);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<AppointmentSlotResponse>(existingSlot);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
