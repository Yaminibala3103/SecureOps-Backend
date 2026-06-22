using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureOPS.Applications.DTOs;
using SecureOPS.Applications.ServiceInterfaces;

namespace SecureOPS.WebAPI.Controllers
{
    [Authorize] // Requires a valid JWT token for all endpoints
    [ApiController]
    [Route("api/[controller]")]
    public class SecurityEventsController : ControllerBase
    {
        private readonly ISecurityEventService _eventService;

        public SecurityEventsController(ISecurityEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        // Restricted to analysts and admins for oversight
        [Authorize(Roles = "L1 Analyst, L2 Analyst, Threat Hunter, Security Admin")]
        public async Task<ActionResult<IEnumerable<SecurityEventDto>>> GetAll()
        {
            try
            {
                return Ok(await _eventService.GetAllEventsAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving events.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "L1 Analyst, L2 Analyst, Threat Hunter, Security Admin")]
        public async Task<ActionResult<SecurityEventDto>> GetById(int id)
        {
            try
            {
                var result = await _eventService.GetEventByIdAsync(id);
                return result != null ? Ok(result) : NotFound(new { message = "Event not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        // 'Employee' is included so they can raise security events
        [Authorize(Roles = "Employee,Security Admin")]
        public async Task<ActionResult<SecurityEventDto>> Ingest([FromBody] SecurityEventDto dto)
        {
            if (dto == null) return BadRequest("Event data is required.");

            try
            {
                // This triggers the 'Watcher' logic to auto-create alerts and notify roles
                var created = await _eventService.CreateEventAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.EventID }, created);
            }
            catch (Exception ex)
            {
                // Catching potential SqlNullValueException from the notification service
                return StatusCode(500, new { message = "Event saved, but notification loop failed.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        // Strictly for Admins to protect the audit trail
        [Authorize(Roles = "Security Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _eventService.DeleteEventAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}