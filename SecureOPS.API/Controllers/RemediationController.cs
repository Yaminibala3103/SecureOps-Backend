using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SecureOps.Application.Interfaces;
using SecureOps.Applications.DTOs;
using SecureOPS.Domain.Enum;


namespace SecureOps.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RemediationController : ControllerBase
    {
        private readonly IRemediationService _service;
        public RemediationController(IRemediationService service) => _service = service;

        [HttpGet]
        [Authorize(Roles = "L1 Analyst, L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
        [HttpGet("{id}")]
        [Authorize(Roles = "L1 Analyst, L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result != null ? Ok(result) : NotFound($"{id} not Found");
        }

        [HttpPost]
        [Authorize(Roles = "L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
        public async Task<IActionResult> Create([FromBody] CreateRemediationDto dto)
        {
            try
            {
                var result = await _service.CreateRemediationAsync(dto);
                return Ok(new { Message = "Remediation task identified successfully.", Data = result });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "L2 Analyst, Incident Responder, Security Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateRemediationStatusDto dto)
        {
            // Fix for CS1503: Pass dto.Status (int) to the service
            var success = await _service.UpdateStatusAsync(id, dto.Status);
            return success ? Ok("Status updated.") : NotFound("Task not found.");
        }
        [HttpPatch("{id}/AssignedToUserId")]
        [Authorize(Roles = "Security Admin, Incident Responder")]
        public async Task<IActionResult> UpdateOwnerAsyn(int id, [FromBody] UpdateRemediationOwnerDto dto)
        {
            var success = await _service.UpdateOwnerAsync(id, dto.AssignedToUserId);
            return success ? Ok("Owner updated.") : NotFound("Task not found.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Security Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? Ok("Record archived.") : NotFound();
        }
    }
}