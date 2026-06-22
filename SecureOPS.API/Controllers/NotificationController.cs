using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureOPS.Applications.DTOs;
using SecureOPS.Applications.ServiceInterfaces;

namespace SecureOPS.WebAPI.Controllers
{
    [Authorize] // Requires a valid JWT for all endpoints
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/Notifications/user/2472047
        // Analysts can view their own history; Admins can view any history
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "L1 Analyst, L2 Analyst, Responder, Hunter, Manager, Security Admin")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetForUser(int userId)
        {
            try
            {
                // In a production app, you would verify that the userId matches the token's ID
                // unless the user is an Admin.
                var result = await _notificationService.GetNotificationsByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PATCH: api/Notifications/5/read
        [HttpPatch("{id}/read")]
        [Authorize(Roles = "L1 Analyst, L2 Analyst, Responder, Hunter, Manager, Security Admin")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/Notifications/send
        // Targeted notifications typically sent by L2s or Admins
        [HttpPost("send")]
        [Authorize(Roles = "L2 Analyst, Manager, Security Admin")]
        public async Task<IActionResult> CreateNotification([FromBody] NotificationDto dto)
        {
            if (dto == null) return BadRequest("Invalid notification data.");

            try
            {
                var result = await _notificationService.CreateNotificationAsync(dto);
                return CreatedAtAction(nameof(GetForUser), new { userId = result.UserID }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/Notifications/notify-role/L2 Analyst
        // ESCALATION: Crucial for the L2 Analyst (You) to alert teams
        [HttpPost("notify-role/{role}")]
        [Authorize(Roles = "L2 Analyst, Manager, Security Admin")]
        public async Task<IActionResult> NotifyByRole(string role, [FromBody] string message)
        {
            if (string.IsNullOrEmpty(message))
                return BadRequest("Message content is required.");

            try
            {
                await _notificationService.NotifyRoleAsync(message, "Security-Escalation", role);
                return Ok(new { status = $"Alert broadcasted to all users with role: {role}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/Notifications/broadcast
        // High-level system alerts reserved for Admins and Managers
        [HttpPost("broadcast")]
        [Authorize(Roles = "Manager, Security Admin")]
        public async Task<IActionResult> SendSystemAlert([FromBody] string message)
        {
            if (string.IsNullOrEmpty(message))
                return BadRequest("Message content is required.");

            try
            {
                await _notificationService.CreateSystemNotificationAsync(message, "System");
                return Ok(new { status = "System broadcast sent." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}