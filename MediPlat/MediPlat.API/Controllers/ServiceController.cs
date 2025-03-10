using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MediPlat.API.Controllers
{
    [ApiController]
    [Route("odata/Services")]
    public class ServiceController : ODataController
    {
        private readonly IMediPlatService _mediPlatService;
        private readonly ILogger<ServiceController> _logger;

        public ServiceController(IMediPlatService mediPlatService, ILogger<ServiceController> logger)
        {
            _mediPlatService = mediPlatService;
            _logger = logger;
        }

        [HttpGet]
        [EnableQuery]
        [Authorize(Policy = "DoctorOrAdminPolicy")]
        public IQueryable<ServiceResponse> GetServices()
        {
            return _mediPlatService.GetAllServices();
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "DoctorOrAdminPolicy")]
        public async Task<IActionResult> GetServiceById(Guid id)
        {
            return Ok(await _mediPlatService.GetServiceByIdAsync(id));
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateService([FromBody] ServiceRequest request)
        {
            return Ok(await _mediPlatService.AddServiceAsync(request));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateService(Guid id, [FromBody] ServiceRequest request)
        {
            var result = await _mediPlatService.UpdateServiceAsync(id, request);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteService(Guid id)
        {
            try
            {
                bool isDeleted = await _mediPlatService.DeleteServiceAsync(id);

                if (!isDeleted)
                {
                    return NotFound(new { message = "Service not found." });
                }

                return Ok(new { message = "Service deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting service.", error = ex.Message });
            }
        }
    }
}
