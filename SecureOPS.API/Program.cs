using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SecureOps.Application.Interfaces;
using SecureOps.Application.ServiceInterfaces;
using SecureOps.Application.Services;
using SecureOps.Applications.Interfaces;
using SecureOps.Domain.Interfaces;
using SecureOps.Infrastructure.Repositories;
using SecureOPS.Application.Mapping;
using SecureOPS.Application.Services;
using SecureOPS.Applications.Calculators;
using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Applications.ServiceInterfaces;
using SecureOPS.Applications.Services;
using SecureOPS.Domain.Data;
using SecureOPS.Infrastructure.Authentication;
using SecureOPS.Infrastructure.Repositories;
using SecureOPS.Infrastructure.Services;
using SecureOPS.WebAPI.Middleware;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//==========================================
//1. AZURE KEY VAULT
//==========================================
var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
if (!string.IsNullOrEmpty(keyVaultUrl))
{
	try
	{
		builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());
		Console.WriteLine("Successfully connected to Azure Key Vault.");
	}
	catch (Exception ex)
	{
		Console.WriteLine($"WARNING: Key Vault inaccessible. Using local settings. Error: {ex.Message}");
	}
}

// -------------------------------
// 2. DATABASE
// -------------------------------
var connectionString = builder.Configuration["ConnectionString-Kamal"];
if (string.IsNullOrEmpty(connectionString))
{
	throw new InvalidOperationException("DefaultConnection string not found in configuration.");
}
builder.Services.AddDbContext<SecureOpsDbContext>(options =>
	options.UseSqlServer(connectionString));

// -------------------------------
// 3. AUTHENTICATION & JWT
// -------------------------------
var jwt = builder.Configuration.GetSection("JwtSettings");
var issuer = jwt["Issuer"] ?? "SecureOpsAPI";
var audience = jwt["Audience"] ?? "SecureOpsClient";
var secret = jwt["Secret"] ?? "YourSuperSecretKeyThatIsAtLeast32CharsLong";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = key,
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidIssuer = issuer,
		ValidAudience = audience,
		ClockSkew = TimeSpan.Zero
	};
	options.Events = new JwtBearerEvents
	{
		OnMessageReceived = context =>
		{
			if (string.IsNullOrEmpty(context.Token) &&
				context.Request.Cookies.TryGetValue("accessToken", out var token))
			{
				context.Token = token;
			}
			return Task.CompletedTask;
		}
	};
});

// -------------------------------
// 4. CORS & RATE LIMITING
// -------------------------------
builder.Services.AddCors(options =>
{
	options.AddPolicy("SecurePolicy", policy =>
		policy.WithOrigins("https://localhost:3000", "http://localhost:3000", "http://localhost:5157")
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials());

	options.AddPolicy("AllowAll", policy =>
		policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddRateLimiter(options =>
{
	options.AddFixedWindowLimiter(policyName: "AuthPolicy", opt =>
	{
		opt.Window = TimeSpan.FromMinutes(1);
		opt.PermitLimit = 5;
		opt.QueueLimit = 0;
	});
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// -------------------------------
// 5. APPLICATION SERVICES & REPOSITORIES
// -------------------------------
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddSignalR();
builder.Services.AddAutoMapper((typeof(MappingProfile).Assembly));

builder.Host.UseSerilog((context, configuration) =>
	configuration.ReadFrom.Configuration(context.Configuration));

// Repositories
builder.Services.AddScoped<IThreatIndicatorRepository, ThreatIndicatorRepository>();
builder.Services.AddScoped<IThreatCampaignRepository, ThreatCampaignRepository>();
builder.Services.AddScoped<IVulnerabilityRepository, VulnerabilityRepository>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IRemediationRepository, RemediationRepository>();
builder.Services.AddScoped<ISecurityEventRepository, SecurityEventRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<IIncidentTaskRepository, IncidentTaskRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

// Services
builder.Services.AddScoped<IThreatIndicatorService, ThreatIndicatorService>();
builder.Services.AddScoped<IThreatCampaignService, ThreatCampaignService>();
builder.Services.AddScoped<IVulnerabilityService, VulnerabilityService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IRemediationService, RemediationService>();
builder.Services.AddScoped<ISecurityEventService, SecurityEventService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<IAuditLogService, AuditService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReportMetricsCalculator, ReportMetricsCalculator>();





builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});


// -------------------------------
// 6. CONTROLLERS & SWAGGER
// -------------------------------
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
		options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
	});
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
	options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo { Title = "SecureOPS API", Version = "v1" });

	options.OrderActionsBy(apiDesc =>
	{
		var controllerName = apiDesc.ActionDescriptor.RouteValues["controller"];
		return controllerName == "Auth" ? "001" : $"002_{controllerName}";
	});

    options.AddSecurityRequirement(document =>
		new OpenApiSecurityRequirement
		{
			[new OpenApiSecuritySchemeReference("bearer", document)] = new List<string>()
		});
});

var app = builder.Build();

// -------------------------------
// 7. MIDDLEWARE PIPELINE
// -------------------------------
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("SecurePolicy");
app.UseRateLimiter();
app.UseAuthentication(); 
app.UseAuthorization();

app.MapHub<NotificationHub>("/notificationHub");
app.MapControllers();

// -------------------------------
// 8. AUTO-MIGRATION
// -------------------------------
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var context = services.GetRequiredService<SecureOpsDbContext>();
		context.Database.Migrate();
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while setting up the database.");
	}
}

app.Run();