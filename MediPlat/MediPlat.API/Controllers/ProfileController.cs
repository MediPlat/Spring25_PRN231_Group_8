using MediPlat.Model.RequestObject;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace MediPlat.API.Controllers
{
    [ApiController]
    [Route("odata/Profile")]
    public class ProfileController : ODataController
    {
        private readonly IProfileService _ProfileService;
        static Guid temp;
        public ProfileController(IProfileService ProfileService)
        {
            _ProfileService = ProfileService;
        }

        [Authorize(Roles = "Admin, Patient")]
        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _ProfileService.GetAll(HttpContext.User));
        }

        [Authorize(Roles = "Admin, Patient")]
        [EnableQuery]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _ProfileService.GetById(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Patient")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProfileRequest ProfileModel)
        {
            return Ok(await _ProfileService.Create(ProfileModel, HttpContext.User));
        }

        [Authorize(Roles = "Admin, Patient")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] ProfileRequest ProfileModel)
        {
            var result = await _ProfileService.Update(ProfileModel, HttpContext.User);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Patient")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete()
        {
            var result = await _ProfileService.DeleteById(HttpContext.User);
            return Ok(result);
        }
    }
}
