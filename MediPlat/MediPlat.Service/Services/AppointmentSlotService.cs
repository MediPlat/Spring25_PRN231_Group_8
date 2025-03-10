using AutoMapper;
using Azure.Core;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediPlat.Service.Services
{
    public class AppointmentSlotService : IAppointmentSlotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentSlotService> _logger;
        public AppointmentSlotService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AppointmentSlotService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AppointmentSlotResponse?> CreateAppointmentSlot(AppointmentSlotRequest appointmentSlotRequest)
        {
            try
            {
                var appointmentSlot = _mapper.Map<AppointmentSlot>(appointmentSlotRequest);

                appointmentSlot.Id = Guid.NewGuid();

                _unitOfWork.AppointmentSlots.Add(appointmentSlot);
                await _unitOfWork.SaveChangesAsync();

                // Tạo danh sách thuốc nếu có
                foreach (var medicine in appointmentSlotRequest.Medicines)
                {
                    var appointmentSlotMedicine = new AppointmentSlotMedicine
                    {
                        AppointmentSlotMedicineId = Guid.NewGuid(),
                        AppointmentSlotId = appointmentSlot.Id, // ID đã được tạo mới
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
                throw new Exception($"❌ Lỗi khi tạo AppointmentSlot: {ex.Message}");
            }
        }

        public async Task<bool> DeleteAppointmentSlot(Guid appointmentSlotId)
        {
            if (appointmentSlotId == Guid.Empty)
            {
                _logger.LogError("❌ Không thể xóa vì ID rỗng.");
                return false;
            }

            var entity = await _unitOfWork.AppointmentSlots
                .GetAll(a => a.AppointmentSlotMedicines)
                .FirstOrDefaultAsync(a => a.Id == appointmentSlotId);

            if (entity == null)
            {
                _logger.LogWarning($"❌ Không tìm thấy đơn thuốc ID: {appointmentSlotId}");
                return false;
            }

            var medicines = await _unitOfWork.AppointmentSlotMedicines.GetListAsync(m => m.AppointmentSlotId == appointmentSlotId);
            if (medicines.Any())
            {
                _unitOfWork.AppointmentSlotMedicines.RemoveRange(medicines);
            }

            _unitOfWork.AppointmentSlots.Remove(entity);

            await _unitOfWork.SaveChangesAsync();
            return true;
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
                .GetAll(s => s.Profile, s => s.AppointmentSlotMedicines, s => s.AppointmentSlotMedicines)
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
                .GetAll(a => a.AppointmentSlotMedicines, a => a.Slot, a => a.Profile, a => a.Slot.Doctor)
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
                _logger.LogInformation($"🔎 Checking AppointmentSlot: Id={id}, ProfileId={appointmentSlotRequest.ProfileId}, SlotId={appointmentSlotRequest.SlotId}");

                // 🔥 Kiểm tra và gán giá trị mặc định trước khi tìm `existingSlot`
                if (appointmentSlotRequest.ProfileId == null || appointmentSlotRequest.SlotId == null)
                {
                    var existingData = await _unitOfWork.AppointmentSlots
                        .GetAll(e => e.Slot, e => e.Profile)
                        .FirstOrDefaultAsync(e => e.Id == id);

                    if (existingData == null)
                    {
                        throw new KeyNotFoundException("❌ Không tìm thấy AppointmentSlot.");
                    }

                    appointmentSlotRequest.ProfileId ??= existingData.ProfileId;
                    appointmentSlotRequest.SlotId ??= existingData.SlotId;
                }

                // 🔥 Tìm slot sau khi đã đảm bảo ProfileId và SlotId không bị null
                var existingSlot = await _unitOfWork.AppointmentSlots
                    .GetAll(e => e.Slot, e => e.Profile, e => e.AppointmentSlotMedicines)
                    .FirstOrDefaultAsync(e => e.Id == id && e.ProfileId == appointmentSlotRequest.ProfileId && e.SlotId == appointmentSlotRequest.SlotId);

                if (existingSlot == null)
                {
                    throw new KeyNotFoundException("❌ AppointmentSlot không tồn tại hoặc không thuộc về bác sĩ này.");
                }

                _mapper.Map(appointmentSlotRequest, existingSlot);

                // 🔄 Cập nhật danh sách thuốc
                foreach (var medicine in existingSlot.AppointmentSlotMedicines)
                {
                    var updatedMedicine = appointmentSlotRequest.Medicines.FirstOrDefault(m => m.MedicineId == medicine.MedicineId);
                    if (updatedMedicine != null)
                    {
                        medicine.Dosage = updatedMedicine.Dosage;
                        medicine.Instructions = updatedMedicine.Instructions;
                        medicine.Quantity = updatedMedicine.Quantity;
                    }
                }

                // 🔄 Cập nhật các trường cần thiết
                await _unitOfWork.AppointmentSlots.UpdatePartialAsync(existingSlot, aps => aps.Notes, aps => aps.Status);

                var response = _mapper.Map<AppointmentSlotResponse>(existingSlot);
                await _unitOfWork.SaveChangesAsync();
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateAppointmentSlotStatus(Guid appointmentSlotId, string status)
        {
            var existingSlot = await _unitOfWork.AppointmentSlots
                .GetAll(e => e.Slot, e => e.Profile)
                .FirstOrDefaultAsync(e => e.Id == appointmentSlotId);

            if (existingSlot == null)
            {
                _logger.LogWarning($"❌ Không tìm thấy AppointmentSlot với ID: {appointmentSlotId}");
                return false;
            }

            if (existingSlot.Status == "Confirmed")
            {
                _logger.LogWarning($"⛔ Không thể cập nhật trạng thái vì đơn thuốc đã được xác nhận.");
                return false;
            }

            existingSlot.Status = status;

            await _unitOfWork.AppointmentSlots.UpdatePartialAsync(existingSlot, aps => aps.Status);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

    }
}