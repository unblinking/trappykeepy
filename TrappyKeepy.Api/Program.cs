using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TrappyKeepy.Data;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Service;

var builder = WebApplication.CreateBuilder(args);

// Add the AutoMapper - https://automapper.org/
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container to be used for dependency injection.
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IKeeperService, KeeperService>();
builder.Services.AddTransient<IGroupService, GroupService>();
builder.Services.AddTransient<IMembershipService, MembershipService>();
builder.Services.AddTransient<IPermitService, PermitService>();

// Add controllers to the container.
// When controllers respond with JSON, leave out any keys that have null values.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// Add JSON Web Token authorization and authentication.
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var key = $"{Environment.GetEnvironmentVariable("TK_CRYPTO_KEY")}";
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "TrappyKeepy",
        ValidAudience = "TrappyKeepy",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

// Setup the OpenAPI (aka Swagger) landing page.
builder.Services.AddEndpointsApiExplorer(); // OpenAPI.
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TrappyKeepy",
        Version = "v0.1.0",
        Description = "A simple document storage web API."
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer <token>'.",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // When starting the API in development mode, load the OpenAPI/Swagger page.
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrappyKeepy");
        c.RoutePrefix = "";
    });
}

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.EnvironmentName != "Testing")
{
    // If this is set during testing, the following warning will show up when using WebApplicationFactory
    // warn: Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware[3] Failed to determine the https port for redirect.
    // So, we only set this when we aren't in the "Testing" environment.
    app.UseHttpsRedirection();
}

app.MapControllers();
app.Run();

// This is here for the end-to-end tests.
// So that I can use IClassFixture<WebApplicationFactory<Program>>
#pragma warning disable CA1050 // Declare types in namespaces
public partial class Program
{
}
#pragma warning restore CA1050 // Declare types in namespaces
