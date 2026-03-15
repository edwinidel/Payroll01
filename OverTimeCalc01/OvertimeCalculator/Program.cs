using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OvertimeCalculator.Data;
using OvertimeCalculator.Services;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
var logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "app-.txt");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day, 
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Add logging service
builder.Services.AddLogging();

// Configure API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<OvertimeCalculatorService>();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Ingrese su token JWT en el campo de texto a continuación.\n\nExample: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
    
    // Include XML documentation
    var xmlFile = Path.Combine(AppContext.BaseDirectory, "OvertimeCalculator.xml");
    if (File.Exists(xmlFile))
    {
        c.IncludeXmlComments(xmlFile);
    }
});
builder.Services.AddDbContext<OvertimeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.CommandTimeout(60)));

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing");
        var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing");
        var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Response.HasStarted)
                {
                    // If the response already started, skip writing the custom body to avoid InvalidOperationException
                    return Task.CompletedTask;
                }

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                
                string errorMessage = context.Exception?.GetType().Name switch
                {
                    nameof(SecurityTokenExpiredException) => "Token expirado",
                    nameof(SecurityTokenInvalidSignatureException) => "Firma de token inválida",
                    nameof(SecurityTokenInvalidIssuerException) => "Emisor de token inválido",
                    nameof(SecurityTokenInvalidAudienceException) => "Público de token inválido",
                    _ => context.Exception?.Message ?? "Token inválido"
                };

                return context.Response.WriteAsync($"{{\"error\": \"Token inválido\", \"message\": \"{errorMessage}\"}}");
            },
            OnChallenge = context =>
            {
                if (context.Response.HasStarted)
                {
                    // Response already started; avoid modifying it to prevent InvalidOperationException
                    return Task.CompletedTask;
                }

                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                
                string message = "Falta el token o no es válido. Asegúrese de enviar el token en el encabezado Autorización como: Autorización: Portador <token>";
                return context.Response.WriteAsync($"{{\"error\": \"Unauthorized\", \"message\": \"{message}\"}}");
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Initialize database with seed data
await SeedDb.InitializeAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
