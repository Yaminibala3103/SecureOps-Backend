using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureOPS.Applications.DTOs;
using SecureOPS.Applications.ServiceInterfaces;

namespace SecureOPS.WebAPI.Controllers
{
    [Authorize] // Requires authentication for all endpoints
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentsController : ControllerBase
    {
        private readonly IIncidentService _incidentService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="incidentService"></param>
        public IncidentsController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        // POST: api/Incidents/promote/10
        // Restricted to L2 Analysts and Admins
        [HttpPost("promote/{alertId}")]
        [Authorize(Roles = "L2 Analyst, Security Admin")]
        public async Task<IActionResult> PromoteAlert(int alertId, [FromBody] string title)
        {
            if (string.IsNullOrEmpty(title)) return BadRequest("Incident title is required.");

            try
            {
                var result = await _incidentService.PromoteAlertToIncidentAsync(alertId, title);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET: api/Incidents/5
        // Accessible to the whole SOC team for situational awareness
        [HttpGet("{id}")]
        [Authorize(Roles = "L1 Analyst, L2 Analyst,Incident Responder, Hunter, Manager, Admin, Security Admin")]
        public async Task<ActionResult<IncidentDto>> GetIncident(int id)
        {
            try
            {
                var incident = await _incidentService.GetIncidentDetailsAsync(id);
                if (incident == null) return NotFound(new { message = "Incident not found." });
                return Ok(incident);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PATCH: api/Incidents/5/status
        // Restricted to those managing the case lifecycle
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "L2 Analyst,Incident Responder, Manager, Security Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
        {
            if (string.IsNullOrEmpty(newStatus)) return BadRequest("Status cannot be empty.");

            try
            {
                await _incidentService.UpdateIncidentStatusAsync(id, newStatus);
                return Ok(new { message = $"Incident {id} marked as {newStatus}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/Incidents/tasks
        // Typically handled by L2 Analysts or Managers to delegate work
        [HttpPost("tasks")]
        [Authorize(Roles = "L2 Analyst, Manager, Security Admin")]
        public async Task<IActionResult> CreateTask([FromBody] IncidentTaskDto taskDto)
        {
            if (taskDto == null) return BadRequest();

            try
            {
                var result = await _incidentService.AssignTaskAsync(taskDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}