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

    [HttpPost]
    //[Authorize(Policy = "DoctorOrAdminPolicy")]
    public async Task<IActionResult> Insert([FromBody] SpecialtyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _specialtyService.AddSpecialtyAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    //[Authorize(Policy = "DoctorOrAdminPolicy")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SpecialtyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _specialtyService.UpdateSpecialtyAsync(id, request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    //[Authorize(Policy = "DoctorOrAdminPolicy")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _specialtyService.DeleteSpecialtyAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}