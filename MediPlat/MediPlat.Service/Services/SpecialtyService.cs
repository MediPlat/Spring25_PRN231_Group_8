using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
using System;
using System.Linq;
using System.Threading.Tasks;

public class SpecialtyService : ISpecialtyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SpecialtyService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public IQueryable<SpecialtyResponse> GetAllSpecialties()
    {
        var specialties = _unitOfWork.Specialties.GetAll().ToList();
        return specialties.Select(s => _mapper.Map<SpecialtyResponse>(s)).AsQueryable();
    }

    public async Task<SpecialtyResponse?> GetSpecialtyByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.Specialties.GetAsync(s => s.Id == id);
        return entity != null ? _mapper.Map<SpecialtyResponse>(entity) : null;
    }

    public async Task<SpecialtyResponse> UpdateSpecialtyAsync(Guid id, SpecialtyRequest request)
    {
        var existingSpecialty = await _unitOfWork.Specialties.GetAsync(s => s.Id == id) ?? throw new Exception("Specialty not found");

        // Map request vào entity hiện có
        _mapper.Map(request, existingSpecialty);

        // Cập nhật trong database
        _unitOfWork.Specialties.Update(existingSpecialty);
        await _unitOfWork.SaveChangesAsync();

        // Trả về response
        return _mapper.Map<SpecialtyResponse>(existingSpecialty);
    }

    public async Task<SpecialtyResponse> AddSpecialtyAsync(SpecialtyRequest request)
    {
        // Map từ request sang entity
        var specialty = _mapper.Map<Specialty>(request);
        specialty.Id = Guid.NewGuid(); // Tạo mới Id

        // Thêm vào database
        _unitOfWork.Specialties.Add(specialty);
        await _unitOfWork.SaveChangesAsync();

        // Trả về response
        return _mapper.Map<SpecialtyResponse>(specialty);
    }

    public async Task<bool> DeleteSpecialtyAsync(Guid id)
    {
        var specialty = await _unitOfWork.Specialties.GetAsync(s => s.Id == id);
        if (specialty == null)
        {
            return false;
        }

        // Xóa khỏi database
        _unitOfWork.Specialties.Remove(specialty);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
