using MediPlat.Model.RequestObject;
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
    [Authorize(Roles = "Admin,Doctor")]
    public IActionResult Get()
    {
        return Ok(_medicineService.GetAllMedicines());
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _medicineService.GetMedicineByIdAsync(id);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] MedicineRequest request)
    {
        await _medicineService.AddMedicineAsync(request);
        return Created("odata/Medicines", request);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] MedicineRequest request)
    {
        await _medicineService.UpdateMedicineAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _medicineService.DeleteMedicineAsync(id);
        return NoContent();
    }
}