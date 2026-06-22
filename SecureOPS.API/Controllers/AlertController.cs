using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureOPS.Applications.DTOs;
using SecureOPS.Applications.ServiceInterfaces;

namespace SecureOPS.WebAPI.Controllers
{
    [Authorize] // Requires a valid JWT token for all endpoints
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertsController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        [HttpGet]
        [Authorize(Roles = "L1 Analyst, L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
        public async Task<ActionResult<IEnumerable<AlertDto>>> GetActiveAlerts()
        {
            try
            {
                return Ok(await _alertService.GetAllAlertsAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "L2 Analyst, Incident Responder, Security Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] AlertDto dto)
        {
            try
            {
                await _alertService.UpdateAlertAsync(id, dto);
                return Ok(new { message = "Alert status updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Used by Member 3 (Incident Responder) to link alerts to an Incident
        [HttpPatch("{id}/link/{incidentId}")]
        [Authorize(Roles = "L2 Analyst, Incident Responder, Security Admin")]
        public async Task<IActionResult> LinkToIncident(int id, int incidentId)
        {
            try
            {
                await _alertService.LinkAlertToIncidentAsync(id, incidentId);
                return Ok(new { message = "Alert successfully linked to incident." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}