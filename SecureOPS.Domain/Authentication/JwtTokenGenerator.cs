using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SecureOPS.Domain.Entities;
using System.Security.Cryptography;

namespace SecureOPS.Infrastructure.Authentication;

public class JwtTokenGenerator
{
	private readonly IConfiguration _config;

	// Use a consistent field name
	public JwtTokenGenerator(IConfiguration config) => _config = config;

	private string GetSecretKey() => _config["JwtSettings:Secret"] ?? "Your_Super_Secret_Key_At_Least_32_Chars_Long";

	public string GenerateToken(User user, bool rememberMe = false)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSecretKey()));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		// Standardizing expiry logic
		var expiry = rememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(1);

		var claims = new List<Claim>
        {
			// Use ?? "" to provide an empty string if the property is null
			new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
	        new(ClaimTypes.Name, user.Email ?? ""),
			new("name", user.Name ?? "User"),
			new("id", user.UserId.ToString()),
			new("isVerified", user.IsEmailVerified.ToString().ToLower())
		};

		if (!string.IsNullOrEmpty(user.Role))
		{
			foreach (var r in user.Role.Split(','))
				claims.Add(new Claim(ClaimTypes.Role, r.Trim()));
		}

		var token = new JwtSecurityToken(
			_config["JwtSettings:Issuer"],
			_config["JwtSettings:Audience"],
			claims,
			expires: expiry,
			signingCredentials: creds);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
	{
		var tokenValidationParameters = new TokenValidationParameters
		{
			ValidateAudience = false, // Set to true in production if you have multiple audiences
			ValidateIssuer = false,   // Set to true in production
			ValidateIssuerSigningKey = true,
			// Fixed: Use GetSecretKey() to match GenerateToken logic
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSecretKey())),
			ValidateLifetime = false // Crucial: Allows us to read the email even if the token is expired
		};

		var tokenHandler = new JwtSecurityTokenHandler();

		// Fix: Changed _configuration to _config to match the constructor
		var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

		if (securityToken is not JwtSecurityToken jwtSecurityToken ||
			!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
		{
			throw new SecurityTokenException("Invalid token algorithm");
		}

		return principal;
	}

	public string GenerateRefreshToken()
	{
		var randomNumber = new byte[32];
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}
}