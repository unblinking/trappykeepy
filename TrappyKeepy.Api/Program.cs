using Microsoft.EntityFrameworkCore;
using TrappyKeepy.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// OpenAPI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<KeepyContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("TrappyKeepy");
    opt.UseNpgsql(connectionString);
    opt.UseSnakeCaseNamingConvention();

    // TODO: Remove these two before production.
    opt.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
    opt.EnableSensitiveDataLogging();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrappyKeepy");
        c.RoutePrefix = "";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();