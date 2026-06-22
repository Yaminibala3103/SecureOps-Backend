using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SecureOps.Application.ServiceInterfaces;
using SecureOps.Applications.DTOs;

using SecureOPS.Domain.Entities;
using SecureOPS.Domain.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecureOps.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VulnerabilitiesController : ControllerBase
    {
        private readonly IVulnerabilityService _service;

        public VulnerabilitiesController(IVulnerabilityService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "L1 Analyst, L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
        public async Task<ActionResult<IEnumerable<Vulnerability>>> GetAll()
        {
            var results = await _service.GetAllAsync();
            return Ok(results);
        }


        [HttpGet("{id:int}")]
        [Authorize(Roles = "L1 Analyst, L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
        public async Task<ActionResult<Vulnerability>> GetById(int id)
        {
            var vulnerability = await _service.GetByIdAsync(id);
            if (vulnerability == null)
            {
                return NotFound(new { message = $"Vulnerability with ID {id} not found." });
            }
            return Ok(vulnerability);
        }


        [HttpGet("severity/{severity}")]
        [Authorize(Roles = "L1 Analyst, L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
        public async Task<ActionResult<IEnumerable<Vulnerability>>> GetBySeverity(SeverityLevel severity)
        {
            var results = await _service.GetBySeverityAsync(severity);
            return Ok(results);
        }


        [HttpPost]
        [Authorize(Roles = "L2 Analyst, Threat Hunter, Security Admin")]
        public async Task<ActionResult<Vulnerability>> Create([FromBody] VulnerabilityCreateDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                // Returns 201 Created with the location of the new resource
                return CreatedAtAction(nameof(GetById), new { id = created.VulnerabilityId }, created);
            }
            catch (System.InvalidOperationException ex)
            {
                // Returns 400 if CVE already exists
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPut("{id:int}")]
        [Authorize(Roles = "L2 Analyst, Threat Hunter, Security Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] VulnerabilityCreateDto dto)
        {
            try
            {
                var success = await _service.UpdateAsync(id, dto);
                if (!success)
                {
                    return NotFound(new { message = $"Vulnerability with ID {id} not found." });
                }
                return Ok(new { message = $"Updated successfully Id {id}" }); // 204 Success, no content to return
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Security Admin, Threat Hunter")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Vulnerability with ID {id} not found." });
            }
            return Ok(new { message = $"Vulnerability Id {id} is deleted" });
        }
    }
}