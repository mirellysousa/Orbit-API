using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orbit.Api.Data;
using Orbit.Api.Models;

namespace Orbit.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly OrbitDbContext _context;

    public DashboardController(OrbitDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<object>> Get(CancellationToken cancellationToken)
    {
        var total = await _context.Missoes.CountAsync(cancellationToken);
        var aptas = await _context.Missoes.CountAsync(item => item.Status == StatusMissao.Apta, cancellationToken);
        var emAtencao = await _context.Missoes.CountAsync(item => item.Status == StatusMissao.EmAtencao, cancellationToken);
        var bloqueadas = await _context.Missoes.CountAsync(item => item.Status == StatusMissao.Bloqueada, cancellationToken);
        var alertasCriticos = await _context.Alertas.CountAsync(item => item.Nivel == NivelAlerta.Critico, cancellationToken);

        return Ok(new
        {
            missoes = new { total, aptas, emAtencao, bloqueadas },
            alertas = new { criticos = alertasCriticos }
        });
    }
}
