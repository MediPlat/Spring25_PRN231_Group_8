using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.Services
{
    public class SlotService : ISlotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SlotService> _logger;
        public SlotService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SlotService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public Task CreateSlot(SlotRequest slotRequest)
        {
            try
            {
                var slot = _mapper.Map<Slot>(slotRequest);
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

            var isBeingUsed = await _unitOfWork.AppointmentSlots.GetAsync(am => am.SlotId == slotId);
            if (isBeingUsed != null)
            {
                throw new InvalidOperationException("Slot đã được kích hoạt, không thể xóa.");
            }

            _unitOfWork.Slots.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public IQueryable<SlotResponse> GetSlot()
        {
            return _unitOfWork.Slots.GetAll(s => s.Doctor, s => s.Service).ToList()
            .Select(s => _mapper.Map<SlotResponse>(s)).AsQueryable();
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
