using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace MediPlat.API.Controllers
{
    [ApiController]
    [Route("odata/Profiles")]
    public class ProfileController : ODataController
    {
        private readonly IProfileService _ProfileService;
        static Guid temp;
        public ProfileController(IProfileService ProfileService)
        {
            _ProfileService = ProfileService;
        }

        
        [EnableQuery]
        [Authorize(Policy = "DoctorOrAdminPolicy")]
        [HttpGet]
        public async Task<IQueryable<ProfileResponse>> GetAll()
        {
            return (await _ProfileService.GetAll(HttpContext.User)).AsQueryable();
        }

        [Authorize(Policy = "DoctorOrAdminPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _ProfileService.GetById(id);
            return Ok(result);
        }

        [Authorize(Policy = "PatientPolicy")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProfileRequest ProfileModel)
        {
            return Ok(await _ProfileService.Create(ProfileModel, HttpContext.User));
        }

        [Authorize(Policy = "PatientPolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ProfileRequest ProfileModel)
        {
            var result = await _ProfileService.Update(id, ProfileModel, HttpContext.User);
            return Ok(result);
        }

        [Authorize(Policy = "AdminOrPatientPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _ProfileService.DeleteById(id, HttpContext.User);
            return Ok(result);
        }
    }
}
