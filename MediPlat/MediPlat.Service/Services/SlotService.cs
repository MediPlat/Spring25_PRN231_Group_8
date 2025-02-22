using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
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
        public SlotService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<string> CreateSlot(SlotRequest slotRequest)
        {
            try 
            {
                var slot = _mapper.Map<Slot>(slotRequest);
                slot.Id = Guid.NewGuid();
                _unitOfWork.Slots.Add(slot);
                await _unitOfWork.SaveChangesAsync();
                return "Slot created successfully";
            } catch (Exception ex) { }
            throw new NotImplementedException();
        }

        public async Task<string> UpdateSlot(SlotRequest slotRequest)
        {
            try
            {
                var slot = _mapper.Map<Slot>(slotRequest);
                _unitOfWork.Slots.Update(slot);
                await _unitOfWork.SaveChangesAsync();
                return "Slot updated successfully";
            }
            catch (Exception ex) { }
            throw new NotImplementedException();
        } throw new NotImplementedException();
        }

        public async Task<string> GetSlotByDocorID(Guid doctorId)
        {
            try
            {
                var slots = await _unitOfWork.Slots.GetAllAsync(s =>
                {
                    return s.Doctor == doctorId;
                });
                if (slots == null)
                {
                    return "Slot not found";
                }
                return "Slot found";
            }
            catch (Exception ex) { }
            throw new NotImplementedException();
        }
    }
}
