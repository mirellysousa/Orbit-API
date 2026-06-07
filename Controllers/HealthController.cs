using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orbit.Api.Data;

namespace Orbit.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly OrbitDbContext _context;
    private readonly IConfiguration _configuration;

    public HealthController(OrbitDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet("database")]
    public async Task<ActionResult<object>> Database(CancellationToken cancellationToken)
    {
        var canConnect = await _context.Database.CanConnectAsync(cancellationToken);

        return Ok(new
        {
            provider = _configuration.GetValue("Database:Provider", "Sqlite"),
            efProvider = _context.Database.ProviderName,
            canConnect
        });
    }
}
