using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureOPS.Applications.DTOs;
using SecureOPS.Applications.ServiceInterfaces;
using SecureOPS.Domain.Entities;

namespace SecureOps.WebAPI.Controllers
{

	[ApiController]
	[Route("api/[controller]")]
	public class AuditController : ControllerBase
	{
		private readonly IAuditLogService _auditService;

		public AuditController(IAuditLogService auditService)
		{
			_auditService = auditService;
		}

		[HttpGet]
		[Authorize(Roles = "L1 Analyst, L2 Analyst, Security Admin, Manager")]
		public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAllLogs()
		{
			var logs = await _auditService.GetAllLogsAsync();
			return Ok(logs);
		}


		[HttpGet("user/{userId}")]
		[Authorize(Roles = "L1 Analyst, L2 Analyst, Security Admin, Manager")]
		public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetLogsByUser(string userId)
		{
			var logs = await _auditService.GetLogsByUserAsync(userId);
			//if (logs == null) return NotFound($"No activity history found for user: {userId}");
			return Ok(logs);
		}
	}
}