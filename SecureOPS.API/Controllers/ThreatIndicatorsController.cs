using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SecureOps.Application.Interfaces;
using SecureOps.Application.ServiceInterfaces;
using SecureOps.Applications.DTOs;

namespace SecureOps.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ThreatIndicatorsController : ControllerBase
    {
        private readonly IThreatIndicatorService _service;

        public ThreatIndicatorsController(IThreatIndicatorService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "L1 Analyst, L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
        public async Task<IActionResult> GetAll()
        {
            try { return Ok(await _service.GetAllIndicatorsAsync()); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "L1 Analyst, L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetIndicatorByIdAsync(id);
                if (result == null) return NotFound(new { message = $"We couldn't find an indicator with ID {id}." });
                return Ok(result);
            }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        [HttpPost]
        [Authorize(Roles = "L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
        public async Task<IActionResult> Create([FromBody] ThreatIndicatorCreateDto dto)
        {
            try
            {
                var result = await _service.CreateIndicatorAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.IndicatorId },
                    new { message = "New indicator successfully added.", data = result });
            }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ThreatIndicatorCreateDto dto)
        {
            try
            {
                var success = await _service.UpdateIndicatorAsync(id, dto);
                if (!success) return NotFound(new { message = "Update failed because the indicator was not found." });
                return Ok(new { message = "Indicator information has been updated successfully." });
            }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Security Admin, Threat Hunter")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeleteIndicatorAsync(id);
                if (!success) return NotFound(new { message = "The indicator you are trying to delete does not exist." });
                return Ok(new { message = "The indicator has been successfully removed (soft-deleted)." });
            }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }
    }
}