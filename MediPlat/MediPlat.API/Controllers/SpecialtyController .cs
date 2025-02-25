using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using System;
using System.Threading.Tasks;
using MediPlat.Model.Model;
using MediPlat.Service.Services;

[ApiController]
[Route("odata/Specialties")]
[Authorize(Policy = "DoctorOrAdminPolicy")]
public class SpecialtyController : ODataController
{
    private readonly ISpecialtyService _specialtyService;

    public SpecialtyController(ISpecialtyService specialtyService)
    {
        _specialtyService = specialtyService;
    }

    [HttpGet]
    [EnableQuery]
    [Authorize(Policy = "DoctorOrAdminPolicy")]
    public IActionResult GetAllSpecialties()
    {
         var result = _specialtyService.GetAllSpecialties();
         return Ok(result);
    }


    [HttpGet("{id}")]
    [Authorize(Policy = "DoctorOrAdminPolicy")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _specialtyService.GetSpecialtyByIdAsync(id);
        return result != null ? Ok(result) : NotFound();
    }
}
