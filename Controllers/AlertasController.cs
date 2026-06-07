using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orbit.Api.Data;
using Orbit.Api.Dtos;

namespace Orbit.Api.Controllers;

[ApiController]
[Route("api/alertas")]
public class AlertasController : ControllerBase
{
    private readonly OrbitDbContext _context;

    public AlertasController(OrbitDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlertaResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var alertas = await _context.Alertas
            .AsNoTracking()
            .OrderByDescending(item => item.Id)
            .Select(item => new AlertaResponse(item.Id, item.Tipo, item.Nivel, item.Mensagem))
            .ToListAsync(cancellationToken);

        return Ok(alertas);
    }
}
