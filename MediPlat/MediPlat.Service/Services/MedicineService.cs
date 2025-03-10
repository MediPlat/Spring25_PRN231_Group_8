using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
using MediPlat.Service.Services;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class MedicineService : IMedicineService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<MedicineService> _logger;

    public MedicineService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MedicineService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }
    public IQueryable<MedicineResponse> GetAllMedicines()
    {
        return _unitOfWork.Medicines.GetAll().ToList()
            .Where(m => m.Status == "Active")
            .Select(m => _mapper.Map<MedicineResponse>(m)).AsQueryable();
    }

    public async Task<MedicineResponse> GetMedicineByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.Medicines.GetAsync(m => m.Id == id);
        return entity != null ? _mapper.Map<MedicineResponse>(entity) : null;
    }

    public async Task<MedicineResponse> AddMedicineAsync(MedicineRequest request)
    {
        var existingMedicine = await _unitOfWork.Medicines.GetAsync(m =>
            m.Name.ToLower() == request.Name.ToLower() &&
            (m.DosageForm == null || m.DosageForm.ToLower() == request.DosageForm.ToLower()) &&
            m.Strength.ToLower() == request.Strength.ToLower());

        if (existingMedicine != null)
        {
            throw new InvalidOperationException("Thuốc này đã tồn tại với cùng tên, dạng bào chế và hàm lượng.");
        }

        var entity = _mapper.Map<Medicine>(request);
        entity.Id = Guid.NewGuid();
        _unitOfWork.Medicines.Add(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MedicineResponse>(entity);
    }

    public async Task<MedicineResponse> UpdateMedicineAsync(Guid id, MedicineRequest request)
    {
        var entity = await _unitOfWork.Medicines.GetAsync(m => m.Id == id);
        if (entity == null)
        {
            throw new KeyNotFoundException("Không tìm thấy thuốc.");
        }

        var existingMedicine = await _unitOfWork.Medicines.GetAsync(m =>
            m.Id != id &&
            m.Name.ToLower() == request.Name.ToLower() &&
            (m.DosageForm == null || m.DosageForm.ToLower() == request.DosageForm.ToLower()) &&
            m.Strength.ToLower() == request.Strength.ToLower());

        if (existingMedicine != null)
        {
            throw new InvalidOperationException("Tên thuốc, dạng bào chế và hàm lượng này đã tồn tại.");
        }

        entity.Name = request.Name;
        entity.DosageForm = request.DosageForm;
        entity.Strength = request.Strength;
        entity.SideEffects = request.SideEffects;

        if (!string.IsNullOrEmpty(request.Status))
        {
            entity.Status = request.Status;
        }

        await _unitOfWork.Medicines.UpdatePartialAsync(entity,
            e => e.Name, e => e.DosageForm, e => e.Strength, e => e.SideEffects, e => e.Status);

        return _mapper.Map<MedicineResponse>(entity);
    }
}
