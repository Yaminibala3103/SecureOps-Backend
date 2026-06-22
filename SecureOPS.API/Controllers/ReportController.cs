using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureOPS.Applications.DTOs;

using SecureOPS.Applications.ServiceInterfaces;

namespace SecureOPS.WebAPI.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class ReportsController : ControllerBase
	{
		private readonly IReportService _service;

		public ReportsController(IReportService service)
		{
			_service = service;
		}

		[HttpGet]
		[Authorize(Roles = "L1 Analyst, L2 Analyst, Security Admin, Manager")]
		public async Task<IActionResult> GetAll()
		{
			var reports = await _service.GetAllReports();
			return Ok(reports);
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "L1 Analyst, L2 Analyst, Security Admin, Manager")]
		public async Task<IActionResult> GetById(int id)
		{
			var report = await _service.GetReportById(id);
			return Ok(report);
		}

		

		[HttpPost]
		[Authorize(Roles = "L2 Analyst, Security Admin")]
		public async Task<IActionResult> CreateReport(CreateReportDTO report, int currUserId)
		{
			var result = await _service.CreateReport(report, currUserId);
			return CreatedAtAction(nameof(GetById), new { id = result.ReportId }, result);
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "L2 Analyst, Security Admin")]
		public async Task<IActionResult> UpdateReport(int id, UpdateReportDTO report, int currUserId)
		{
			await _service.UpdateReport(report, currUserId);
			return NoContent();
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "L2 Analyst, Security Admin")]
		public async Task<IActionResult> DeleteReport(int id, int currUserId)
		{
			await _service.DeleteReport(id, currUserId);
			return NoContent();
		}

		[HttpGet("status/{status}")]
		[Authorize(Roles = "L2 Analyst, Security Admin")]
		public async Task<IActionResult> GetByStatus(string status)
		{
			var reports = await _service.GetReportsByStatus(status);
			return Ok(reports);
		}

		[HttpGet("severity/{severity}")]
		[Authorize(Roles = "L2 Analyst, Incident Responder, Security Admin")]
		public async Task<IActionResult> GetBySeverity(string severity)
		{
			var reports = await _service.GetReportsBySeverity(severity);
			return Ok(reports);
		}

		[HttpPost("generate-analytics")]
		[Authorize(Roles = "L2 Analyst, Incident Responder, Security Admin")]
		public async Task<ActionResult<ReportAnalyticsDTO>> GenerateAnalytics()
		{
			var analytics = await _service.GenerateStrategicAnalyticsAsync();
			return Ok(analytics);
		}
	}
}