using SafeVisionPlatform.IAM.Application.Internal.CommandServices;
using SafeVisionPlatform.IAM.Application.Internal.OutboundServices;
using SafeVisionPlatform.IAM.Application.Internal.QueryServices;
using SafeVisionPlatform.IAM.Domain.Model.Commands;
using SafeVisionPlatform.IAM.Domain.Respositories;
using SafeVisionPlatform.IAM.Domain.Services;
using SafeVisionPlatform.IAM.Infrastructure.Hashing.BCrypt.Services;
using SafeVisionPlatform.IAM.Infrastructure.Persistence.EFC.Respositories;
using SafeVisionPlatform.IAM.Infrastructure.Pipeline.Middleware.Extensions;
using SafeVisionPlatform.IAM.Infrastructure.Tokens.JWT.Configuration;
using SafeVisionPlatform.IAM.Infrastructure.Tokens.JWT.Services;
using SafeVisionPlatform.IAM.Interfaces.ACL;
using SafeVisionPlatform.IAM.Interfaces.ACL.Service;
using SafeVisionPlatform.Driver.Infrastructure.Configuration;
using SafeVisionPlatform.FatigueMonitoring.Infrastructure.Configuration;
using SafeVisionPlatform.Management.Infrastructure.Configuration;
using SafeVisionPlatform.Shared.Domain.Repositories;
using SafeVisionPlatform.Shared.Infrastructure.Interfaces.ASP.Configuration;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;
using SafeVisionPlatform.Trip.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers( options => options.Conventions.Add(new KebabCaseRouteNamingConvention()));

// Add Database Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var developmentString = builder.Configuration.GetConnectionString("DevelopmentConnection");

// Configure Database Context and Logging Levels

builder.Services.AddDbContext<AppDbContext>(
    options =>
    {
        if (builder.Environment.IsDevelopment())
        {
            options.UseMySql(developmentString, ServerVersion.AutoDetect(developmentString))
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }
        else if (builder.Environment.IsProduction())
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .LogTo(Console.WriteLine, LogLevel.Error)
                .EnableDetailedErrors();
        }
    });
// Configure Lowercase URLs
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1",
            new OpenApiInfo
            {
                Title = "SafeVision.API",
                Version = "v1",
                Description = "Safe Vision API",
                TermsOfService = new Uri("https://safe-vision.com/tos"),
                Contact = new OpenApiContact
                {
                    Name = "Safe VIsion",
                    Email = "contact@safevision.com"
                },
                License = new OpenApiLicense
                {
                    Name = "Apache 2.0",
                    Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
                }
            });
        c.EnableAnnotations();
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                Array.Empty<string>()
            }
        });
    });

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy",
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// IAM Bounded Context Injection Configuration
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IHashingService, HashingService>();
// Registrar el servicio externo de Profiles que usa IAM al comunicarse con el bounded context Profiles
builder.Services.AddScoped<IProfilesUserExternalService, ProfilesUserExternalService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();
builder.Services.AddScoped<IMfaCommandService, MfaCommandService>();
builder.Services.AddScoped<ISeedUserRoleCommandService, SeedUserRoleCommandService>();
builder.Services.AddScoped<ISeedAdminCommandService, SeedAdminCommandService>();

// Trip Bounded Context Injection Configuration
builder.Services.AddTripBoundedContext();

// Driver Bounded Context Injection Configuration
builder.Services.AddDriverBoundedContext();

// Fatigue Monitoring Bounded Context Injection Configuration
builder.Services.AddFatigueMonitoringBoundedContext();

// Management Bounded Context Injection Configuration
builder.Services.AddManagementBoundedContext();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    
    var userRoleCommandService = services.GetRequiredService<ISeedUserRoleCommandService>();
    await userRoleCommandService.Handle(new SeedUserRolesCommand());
    
    var adminCommandService = services.GetRequiredService<ISeedAdminCommandService>();
    await adminCommandService.Handle(new SeedAdminCommand());
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllPolicy");

app.UseRequestAuthorization();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
