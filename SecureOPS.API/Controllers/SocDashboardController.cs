////using Microsoft.AspNetCore.Authorization;
////using Microsoft.AspNetCore.Mvc;
////using SecureOPS.Domain.Data;

//namespace SecureOPS.API.Controllers
//{
//	[ApiController]
//	[Route("api/soc")]
//	[Authorize] // Requires a valid JWT to access anything here
//	public class SocDashboardController : ControllerBase
//	{
//		private readonly SecureOpsDbContext _context;
//		public SocDashboardController(SecureOpsDbContext context) => _context = context;

////		// ACCESSIBLE BY ALL ANALYSTS
////		[HttpGet("active-alerts")]
////		[Authorize(Roles = "L1 Analyst,L2 Analyst,Security Admin")]
////		public IActionResult GetAlerts() => Ok("Current Threats: 0 Active.");

////		// ACCESSIBLE BY THREAT HUNTERS
////		[HttpGet("threat-intel")]
////		[Authorize(Roles = "Threat Hunter,Security Admin")]
////		public IActionResult GetIntel() => Ok("Deep Web Scan: No data leaks found for your domain.");

////		// ACCESSIBLE BY RESPONDERS (WRITE ACTIONS)
////		[HttpPost("isolate-host")]
////		[Authorize(Roles = "Incident Responder,Security Admin")]
////		public IActionResult IsolateHost(string ip)
////		{
////			// Logic for isolation would go here (e.g., calling a firewall API)
////			return Ok($"Security Action Success: {ip} is now in quarantine.");
////		}

////		// ACCESSIBLE BY MANAGERS
////		[HttpGet("efficiency-stats")]
////		[Authorize(Roles = "Security Manager,Security Admin")]
////		public IActionResult GetStats() => Ok("MTTR: 5 minutes. SOC Efficiency: 98%.");
////	}
////}