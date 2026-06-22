using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SecureOps.Application.Interfaces;
using SecureOps.Applications.DTOs;
using SecureOPS.Domain;
using SecureOPS.Domain.Enum;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _service;
	public AssetsController(IAssetService service) => _service = service;

	[HttpGet]
    [Authorize(Roles = "L1 Analyst, L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
    public async Task<IActionResult> GetAll() { 
        return Ok(await _service.GetAllAssetsAsync());
}

    [HttpGet("{id}")]
    [Authorize(Roles = "L1 Analyst, L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var asset = await _service.GetAssetByIdAsync(id);
        return asset == null ? NotFound() : Ok(asset);
    }

    // New Endpoint: Get by Criticality
    [HttpGet("criticality/{criticality}")]
    [Authorize(Roles = "L1 Analyst, L2 Analyst, Incident Responder, Threat Hunter, Security Admin")]
    public async Task<IActionResult> GetByCriticality(AssetCriticality criticality)
    {
        var results = await _service.GetAssetsByCriticalityAsync(criticality);
        return Ok(results);
    }

    [HttpPost]
    [Authorize(Roles = "Security Admin, Threat Hunter, L2 Analyst")]
    public async Task<IActionResult> Create([FromBody] AssetCreateDto dto)
    {
        try
        {
            var result = await _service.CreateAssetAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.AssetId }, result);
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Security Admin, L2 Analyst")]
    public async Task<IActionResult> Update(int id, [FromBody] AssetCreateDto dto)
    {
        try
        {
            var success = await _service.UpdateAssetAsync(id, dto);
            if (!success) return NotFound();

            return Ok(new { message = "Asset updated successfully", id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // paging 
    [HttpGet("paged")]
    [Authorize(Roles = "Security Admin, Threat Hunter")]
    public async Task<ActionResult<PagedResult<AssetReadDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        // Basic validation to prevent negative pages
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await _service.GetAssetsPagedAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Security Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAssetAsync(id);
        return success ? Ok(new { message = "Asset deleted" }) : NotFound();
    }


}