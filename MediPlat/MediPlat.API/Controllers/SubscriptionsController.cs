﻿using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace MediPlat.API.Controllers
{
    [Route("odata/Subscriptions")]
    [ApiController]
    public class SubscriptionsController : ODataController
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<SubscriptionsController> _logger;

        public SubscriptionsController(ISubscriptionService subscriptionService, ILogger<SubscriptionsController> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        [HttpGet]
        [EnableQuery]
        [Authorize(Policy = "DoctorOrAdminPolicy")]
        public IQueryable<SubscriptionResponse> GetSubscriptions()
        {
            return _subscriptionService.GetAllSubscriptions().OrderBy(s => s.Price);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "DoctorOrAdminPolicy")]
        public async Task<IActionResult> GetSubscription(Guid id)
        {
            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
            return Ok(subscription);
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateSubscription([FromBody] SubscriptionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _subscriptionService.AddSubscriptionAsync(request);
            return CreatedAtAction(nameof(GetSubscription), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateSubscription(Guid id, [FromBody] SubscriptionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _subscriptionService.UpdateSubscriptionAsync(id, request);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteSubscription(Guid id)
        {
            await _subscriptionService.DeleteSubscriptionAsync(id);
            return NoContent();
        }
    }
}
