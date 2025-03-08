﻿using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;

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

        public Task CreateAppointmentSlot(AppointmentSlotRequest appointmentSlotRequest)
        {
            try
            {
                var appointmentSlot = _mapper.Map<AppointmentSlot>(appointmentSlotRequest);
                _unitOfWork.AppointmentsSlots.Add(appointmentSlot);
                return _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new NotImplementedException();
        }

        public async Task DeleteAppointmentSlot(Guid appointmentSlotId)
        {
            var entity = await _unitOfWork.AppointmentsSlots.GetAsync(am => am.Id == appointmentSlotId);
            if (entity == null)
            {
                throw new InvalidOperationException("Slot đã được kích hoạt, không thể xóa.");
            }

            _unitOfWork.AppointmentsSlots.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public IQueryable<AppointmentSlotResponse> GetAppointmentSlot()
        {
            try
            {
                var appointmentSlots = _unitOfWork.AppointmentsSlots.GetAll().AsQueryable()
                    .Select(s => _mapper.Map<AppointmentSlotResponse>(s));
                return appointmentSlots;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new NotImplementedException();
        }

        public async Task<AppointmentSlotResponse?> GetAppointmentSlotByID(Guid appointmentSlotId)
        {
            try
            {
                var appointmentSlot = await _unitOfWork.AppointmentsSlots.GetAsync(s => s.Id == appointmentSlotId);
                if (appointmentSlot == null)
                {
                    return null;
                }
                var slotResponse = _mapper.Map<AppointmentSlotResponse>(appointmentSlot);
                return slotResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            throw new NotImplementedException();
        }
        public Task UpdateAppointmentSlot(AppointmentSlotRequest appointmentSlotRequest)
        {
            try
            {
                var slot = _mapper.Map<AppointmentSlot>(appointmentSlotRequest);
                _unitOfWork.AppointmentsSlots.Update(slot);
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
