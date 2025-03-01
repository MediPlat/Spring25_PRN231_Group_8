using AutoMapper;
using MediPlat.Model.Model;
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
}
