using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;


namespace MediPlat.Service.Services
{
    public class SlotService : ISlotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SlotService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task CreateSlot(SlotRequest slotRequest)
        {
            try
            {
                var slot = _mapper.Map<Slot>(slotRequest);
                slot.Id = Guid.NewGuid();
                _unitOfWork.Slots.Add(slot);
                return _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new NotImplementedException();
        }

        public async Task DeleteSlot(Guid slotId)
        {
            var entity = await _unitOfWork.Slots.GetAsync(s => s.Id == slotId);
            if (entity == null)
            {
                throw new KeyNotFoundException("Không tìm thấy Slot.");
            }

            var isBeingUsed = await _unitOfWork.AppointmentsSlots.GetAsync(am => am.SlotId == slotId);
            if (isBeingUsed != null)
            {
                throw new InvalidOperationException("Slot đã được kích hoạt, không thể xóa.");
            }

            _unitOfWork.Slots.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public IQueryable<SlotResponse> GetAllSlot()
        {
            try
            {
                var slots = _unitOfWork.Slots.GetAll(s => s.Doctor, s => s.Service.Specialty).AsQueryable()
                    .Select(s => _mapper.Map<SlotResponse>(s));
                return slots;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SlotResponse?>> GetSlotByDoctorID(Guid doctorId)
        {
            try
            {
                var slot = await _unitOfWork.Slots.GetListAsync(s => s.DoctorId == doctorId);
                if (slot == null)
                {
                    return null;
                }
                var slotResponse = _mapper.Map<IEnumerable<SlotResponse?>>(slot);
                return slotResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SlotResponse?> GetSlotByID(Guid slotId)
        {
            try
            {
                var slot = await _unitOfWork.Slots.GetAsync(s => s.Id == slotId);
                if (slot == null)
                {
                    return null;
                }
                var slotResponse = _mapper.Map<SlotResponse>(slot);
                return slotResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SlotResponse?>> GetSlotByServiceID(Guid serviceId)
        {
            try
            {
                var slot = await _unitOfWork.Slots.GetListAsync(s => s.ServiceId == serviceId);
                if (slot == null)
                {
                    return null;
                }
                var slotResponse = _mapper.Map<IEnumerable<SlotResponse?>>(slot);
                return slotResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task UpdateSlot(SlotRequest slotRequest)
        {
            try
            {
                var slot = _mapper.Map<Slot>(slotRequest);
                _unitOfWork.Slots.Update(slot);
                return _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new NotImplementedException();
        }
    }
}
