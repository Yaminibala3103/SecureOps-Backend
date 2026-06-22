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
    public class ThreatCampaignsController : ControllerBase
    {
        private readonly IThreatCampaignService _service;

        public ThreatCampaignsController(IThreatCampaignService service)
        {
            _service = service;
        }

        // 1. GET ALL
        [HttpGet]
        [Authorize(Roles = "L1 Analyst, L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var results = await _service.GetAllCampaignsAsync();
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "We couldn't retrieve the campaign list right now. Please try again in a moment." });
            }
        }

        // 2. GET BY ID
        [HttpGet("{id}")]
        [Authorize(Roles = "L1 Analyst, L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetCampaignByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = $"We couldn't find a threat campaign with ID {id}." });
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the campaign details." });
            }
        }

        // 3. CREATE
        [HttpPost]
        [Authorize(Roles = "Threat Hunter, Security Admin, L2 Analyst")]
        public async Task<IActionResult> Create([FromBody] ThreatCampaignCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _service.CreateCampaignAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.CampaignId }, new
                {
                    message = "Threat campaign has been successfully created.",
                    data = result
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "We ran into an issue while saving the new campaign. Please check your input." });
            }
        }

        // 4. UPDATE (FULL)
        [HttpPut("{id}")]
        [Authorize(Roles = "Threat Hunter, Security Admin, L2 Analyst")]
        public async Task<IActionResult> Update(int id, [FromBody] ThreatCampaignCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var success = await _service.UpdateCampaignAsync(id, dto);
                if (!success)
                {
                    return NotFound(new { message = $"Update failed because campaign ID {id} does not exist." });
                }
                return Ok(new { message = "The campaign details have been successfully updated." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Something went wrong while updating the campaign. Please try again later." });
            }
        }
        [HttpPatch("{id}/assign/{userId}")]
        [Authorize(Roles = "Security Admin, Threat Hunter")] // Only high-level users can delegate campaigns
        public async Task<IActionResult> Assign(int id, int userId)
        {
            try
            {
                var success = await _service.AssignToUserAsync(id, userId);
                if (!success)
                {
                    return NotFound(new { message = $"Assignment failed. Either Campaign {id} or User {userId} was not found." });
                }
                return Ok(new { message = $"Campaign {id} has been successfully assigned to User {userId}." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 5. PATCH (STATUS ONLY)
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Incident Responder, Threat Hunter, Security Admin, L2 Analyst")]
        public async Task<IActionResult> PatchStatus(int id, [FromBody] CampaignStatus status)
        {
            try
            {
                var success = await _service.PatchStatusAsync(id, status);
                if (!success)
                {
                    return NotFound(new { message = $"We couldn't find campaign {id} to update its status." });
                }
                return Ok(new { message = $"The campaign status has been successfully changed to {status}." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "The system was unable to update the campaign status at this time." });
            }
        }

        // 6. DELETE
        [HttpDelete("{id}")]
        [Authorize(Roles = "Security Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeleteCampaignAsync(id);
                if (!success)
                {
                    return NotFound(new { message = $"Deletion failed. A campaign with ID {id} was not found." });
                }
                return Ok(new { message = "The campaign and its related threat indicators have been successfully removed." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "A problem occurred while trying to delete the campaign." });
            }
        }
    }
}