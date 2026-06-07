using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Orbit.Api.Data;
using Orbit.Api.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();

builder.Services.AddDbContext<OrbitDbContext>(options =>
{
    var sqliteConnection = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=orbit.db";

    options.UseSqlite(sqliteConnection);
});

builder.Services.AddScoped<IDecisionEngineService, DecisionEngineService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Mobile", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseCors("Mobile");

app.UseAuthorization();

app.MapControllers();

await DatabaseSeeder.SeedAsync(app.Services, app.Configuration);

app.Run();
