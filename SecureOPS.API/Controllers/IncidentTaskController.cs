using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureOPS.Applications.DTOs;
using SecureOPS.Applications.ServiceInterfaces;

namespace SecureOPS.WebAPI.Controllers
{
    [Authorize] // Requires authentication for all task operations
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentTasksController : ControllerBase
    {
        private readonly IIncidentTaskService _taskService;
        public IncidentTasksController(IIncidentTaskService taskService) => _taskService = taskService;

        // POST: api/IncidentTasks/assign
        // L2 Analysts and Managers handle the initial delegation of work
        [HttpPost("assign")]
        [Authorize(Roles = "L2 Analyst, Manager, Security Admin")]
        public async Task<IActionResult> Assign([FromBody] IncidentTaskDto dto)
        {
            try
            {
                var result = await _taskService.CreateAndAssignTaskAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // HttpPatch: api/IncidentTasks/{id}/status
        // Responders update their own progress; L2 Analysts can oversee
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "L2 Analyst,Incident Responder, Security Admin")]
        public async Task<IActionResult> SetStatus(int id, [FromBody] string status)
        {
            try
            {
                await _taskService.UpdateTaskStatusAsync(id, status);
                return Ok(new { message = "Task status updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/IncidentTasks/{id}/notes
        // Documentation is primarily done by the assigned Responder
        [HttpPost("{id}/notes")]
        [Authorize(Roles = "L2 Analyst,Incident Responder, Security Admin")]
        public async Task<IActionResult> AddNotes(int id, [FromBody] string notes)
        {
            try
            {
                await _taskService.AddDocumentationAsync(id, notes);
                return Ok(new { message = "Work documentation added." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/IncidentTasks/{id}/reassign/{userId}
        // Escalation and reassignment are administrative/coordination tasks
        [HttpPost("{id}/reassign/{userId}")]
        [Authorize(Roles = "L2 Analyst, Manager, Security Admin")]
        public async Task<IActionResult> Reassign(int id, int userId, [FromQuery] bool urgent = false)
        {
            try
            {
                await _taskService.ReassignTaskAsync(id, userId, urgent);
                return Ok(new { message = "Task successfully reassigned." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}