using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

[ApiController]
[Route("odata/Medicines")]
public class MedicineController : ODataController
{
    private readonly IMedicineService _medicineService;

    public MedicineController(IMedicineService medicineService)
    {
        _medicineService = medicineService;
    }

    [HttpGet]
    [EnableQuery]
    [Authorize(Policy = "DoctorOrAdminPolicy")]
    public IQueryable<MedicineResponse> GetAllMedicines()
    {
        return _medicineService.GetAllMedicines();
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "DoctorOrAdminPolicy")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _medicineService.GetMedicineByIdAsync(id);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Create([FromBody] MedicineRequest request)
    {
        Console.WriteLine($"API nhận được request: {request.Name}");
        var result = await _medicineService.AddMedicineAsync(request);
        return Created($"odata/Medicines/{result.Id}", new { result.Id });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Update(Guid id, [FromBody] MedicineRequest request)
    {
        var result = await _medicineService.UpdateMedicineAsync(id, request);
        return result != null ? Ok(result) : NotFound();
    }
}