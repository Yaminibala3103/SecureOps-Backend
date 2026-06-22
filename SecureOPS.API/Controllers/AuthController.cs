using SecureOPS.Infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SecureOPS.Domain.Data;
using SecureOPS.Infrastructure.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using AuditLogEntity = SecureOPS.Domain.Entities.AuditLog;
using SecureOPS.Domain.Entities;


namespace SecureOPS.WebApi.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly SecureOpsDbContext _context;
		private readonly JwtTokenGenerator _jwtGenerator;
		private readonly EmailService _emailService;

		public AuthController(SecureOpsDbContext context, JwtTokenGenerator jwtGenerator, EmailService emailService)
		{
			_context = context;
			_jwtGenerator = jwtGenerator;
			_emailService = emailService;
		}

		// --- STEP 1: SIGNUP REQUEST ---
		[HttpPost("signup-request")]
		[EnableRateLimiting("AuthPolicy")]
		public async Task<IActionResult> SignupRequest(string name, string email, string phone)
		{
			var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
			if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
			{
				return BadRequest("Invalid email format. Email must contain '@'.");
			}
			if (existingUser != null)
			{
				await LogAction(existingUser.UserId, "Signup Attempt Failed: Email already exists");
				return BadRequest("Email already registered.");
			}

			string otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
			_emailService.SendOtpEmail(email, otp);

			var user = new User
			{
				Name = name,
				Email = email,
				Phone = phone,
				VerificationOtp = otp,
				OtpExpiry = DateTime.UtcNow.AddMinutes(10),
				IsEmailVerified = false,
				Role = "User"
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			await LogAction(user.UserId, "Signup Initiated: OTP Sent");

			return Ok(new { Message = "OTP sent to Gmail. Valid for 10 minutes." });
		}

		// --- STEP 2: VERIFY OTP ---
		[HttpPost("verify-otp")]
		public async Task<IActionResult> VerifyOtp(string email, string otp)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

			if (user == null || user.VerificationOtp != otp || user.OtpExpiry < DateTime.UtcNow)
			{
				if (user != null) await LogAction(user.UserId, "OTP Verification Failed");
				return BadRequest("Invalid or expired OTP.");
			}

			user.IsEmailVerified = true;
			user.VerificationOtp = null;
			user.OtpExpiry = null;
			await _context.SaveChangesAsync();

			await LogAction(user.UserId, "Email Verified Successfully");

			return Ok("Email verified. Please use complete-registration to set your password and role.");
		}

		// --- STEP 3: COMPLETE REGISTRATION ---
		// --- STEP 3: COMPLETE REGISTRATION ---
		[HttpPost("complete-registration")]
		public async Task<IActionResult> CompleteRegistration(string email, string password, string confirmPassword, string role)
		{
			// 1. Check if passwords match
			if (password != confirmPassword)
				return BadRequest("Passwords do not match.");

			// 2. Check password strength
			if (!IsPasswordStrong(password))
				return BadRequest("Password must be at least 8 characters long and include: an uppercase letter, a lowercase letter, a number, and a special character.");

			var allowedRoles = new List<string>
	{
		"L1 Analyst", "L2 Analyst", "Incident Responder", "Threat Hunter", "Security Admin","Employee","Manager"
	};

			if (!allowedRoles.Contains(role))
				return BadRequest($"Invalid Role.");

			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
			if (user == null || !user.IsEmailVerified)
			{
				return BadRequest("Email verification required.");
			}

			// Hash password and set role
			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
			user.Role = role;

			// Generate tokens
			var accessToken = _jwtGenerator.GenerateToken(user, false);
			var refreshToken = _jwtGenerator.GenerateRefreshToken();

			// Update User Entity
			user.RefreshToken = refreshToken;
			user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

			await _context.SaveChangesAsync();
			await LogAction(user.UserId, $"Registration Completed as {role}");

			// --- COOKIE CONFIGURATION ---
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,        // Prevents JavaScript access (XSS protection)
				Secure = true,          // Ensures cookie is sent over HTTPS only
				SameSite = SameSiteMode.Strict, // Prevents CSRF attacks
				Expires = DateTime.UtcNow.AddMinutes(20)
			};

			// --- APPEND TOKENS TO COOKIES ---
			Response.Cookies.Append("accessToken", accessToken, cookieOptions);
			Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

			// Return success message without the token in the body
			return Ok(new { Message = "Registration completed successfully." });
		}
		// --- LOGIN ---
		[HttpPost("login")]
		[EnableRateLimiting("AuthPolicy")]
		public async Task<IActionResult> Login(string email, string password, bool rememberMe)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
			Console.WriteLine(user);
			if (user == null || !user.IsEmailVerified || string.IsNullOrEmpty(user.PasswordHash))
			{
				return Unauthorized("User not found or unverified.");
			}

			if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
			{
				await LogAction(user.UserId, "Login Failed: Invalid Password");
				return Unauthorized("Invalid password!");
			}

			var accessToken = _jwtGenerator.GenerateToken(user, rememberMe);
			var refreshToken = _jwtGenerator.GenerateRefreshToken();

			// Update Database
			user.RefreshToken = refreshToken;
			user.RefreshTokenExpiry = rememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(7);
			await _context.SaveChangesAsync();

			// Cookie Configuration
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,   // JavaScript cannot access this
				Secure = true,     // Only sent over HTTPS
				SameSite = SameSiteMode.Strict, // CSRF protection
				Expires = user.RefreshTokenExpiry
			};

			// 2. SET COOKIES
			Response.Cookies.Append("accessToken", accessToken, cookieOptions);
			Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

			await LogAction(user.UserId, "User Login Successful");

			// Return NO tokens in body, just user info if needed
			return Ok(new { Message = "Login successful", user.Role });
		}

		// --- FORGOT PASSWORD ---
		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword(string email)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

			// To prevent email enumeration, we return Ok even if the user doesn't exist
			if (user == null)
				return Ok(new { Message = "If that email is registered, a reset OTP has been sent." });

			string otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
			user.VerificationOtp = otp;
			user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);

			await _context.SaveChangesAsync();
			_emailService.SendOtpEmail(email, otp);

			await LogAction(user.UserId, "Password Reset Requested");

			return Ok(new { Message = "Reset OTP sent to your Gmail. Valid for 10 minutes." });
		}

		// --- RESET PASSWORD ---
		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword(string email, string otp, string newPassword, string confirmPassword)
		{
			if (newPassword != confirmPassword)
				return BadRequest("Passwords do not match.");

			if (!IsPasswordStrong(newPassword))
				return BadRequest("Password does not meet complexity requirements.");

			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

			if (user == null || user.VerificationOtp != otp || user.OtpExpiry < DateTime.UtcNow)
			{
				return BadRequest("Invalid or expired reset OTP.");
			}

			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
			user.VerificationOtp = null;
			user.OtpExpiry = null;

			await _context.SaveChangesAsync();
			await LogAction(user.UserId, "Password Reset Successful");

			return Ok("Password updated successfully.");
		}
		/// <returns></returns>
		// --- REFRESH TOKEN ---
		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken()
		{
			var refreshTokenFromCookie = Request.Cookies["refreshToken"];

			if (string.IsNullOrEmpty(refreshTokenFromCookie))
				return Unauthorized("No refresh token provided.");

			var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshTokenFromCookie);

			if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
				return Unauthorized("Invalid or expired refresh token.");

			var newAccessToken = _jwtGenerator.GenerateToken(user, false);
			var newRefreshToken = _jwtGenerator.GenerateRefreshToken();

			user.RefreshToken = newRefreshToken;
			user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
			await _context.SaveChangesAsync();

			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTime.UtcNow.AddDays(7)
			};

			// Update both cookies
			Response.Cookies.Append("accessToken", newAccessToken, cookieOptions);
			Response.Cookies.Append("refreshToken", newRefreshToken, cookieOptions);

			return Ok(new { Message = "Token refreshed successfully" });
		}

		// --- LOGOUT ---
		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			// 1. Identify the user via the Claims in the Access Token
			var email = User.FindFirstValue(ClaimTypes.Email);

			if (!string.IsNullOrEmpty(email))
			{
				var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
				if (user != null)
				{
					// Revoke the refresh token in the Database
					user.RefreshToken = null;
					user.RefreshTokenExpiry = null;
					await _context.SaveChangesAsync();
					await LogAction(user.UserId, "User Logged Out - Session Revoked");
				}
			}

			// 2. Clear the Cookies in the Browser
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTime.UtcNow.AddDays(-1) // Set to yesterday to force deletion
			};

			Response.Cookies.Delete("accessToken", cookieOptions);
			Response.Cookies.Delete("refreshToken", cookieOptions);

			return Ok(new { Message = "Logged out successfully and cookies cleared." });
		}

		// --- AUDIT LOG HELPER ---
		private async Task LogAction(int UserId, string message)
		{
			var auditLog = new AuditLogEntity
			{
				UserId = UserId,
				Action = message,
				Timestamp = DateTime.UtcNow
			};
			_context.AuditLogs.Add(auditLog);
			await _context.SaveChangesAsync();
		}
		private bool IsPasswordStrong(string password)
		{
			// Regex: At least 8 chars, 1 uppercase, 1 lowercase, 1 number, 1 special character
			var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$");
			return passwordRegex.IsMatch(password);
		}
	}
}