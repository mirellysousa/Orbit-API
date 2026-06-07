using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orbit.Api.Data;
using Orbit.Api.Dtos;
using Orbit.Api.Mappings;
using Orbit.Api.Models;
using Orbit.Api.Services;

namespace Orbit.Api.Controllers;

[ApiController]
[Route("api/checkups")]
public class CheckupsController : ControllerBase
{
    private readonly OrbitDbContext _context;
    private readonly IDecisionEngineService _decisionEngineService;

    public CheckupsController(OrbitDbContext context, IDecisionEngineService decisionEngineService)
    {
        _context = context;
        _decisionEngineService = decisionEngineService;
    }

    [HttpPost("avaliar")]
    public async Task<ActionResult<CheckupResponse>> Avaliar(CheckupRequest request, CancellationToken cancellationToken)
    {
        var checkup = await _decisionEngineService.AvaliarMissaoAsync(request.MissaoId, cancellationToken);
        return checkup is null ? NotFound("Missao nao encontrada.") : Ok(checkup.ToResponse());
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CheckupResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var checkups = await QueryCompleta().AsNoTracking().OrderByDescending(item => item.CriadoEm).ToListAsync(cancellationToken);
        return Ok(checkups.Select(item => item.ToResponse()));
    }

    [HttpGet("missao/{missaoId:int}")]
    public async Task<ActionResult<IEnumerable<CheckupResponse>>> GetByMissao(int missaoId, CancellationToken cancellationToken)
    {
        var checkups = await QueryCompleta()
            .AsNoTracking()
            .Where(item => item.MissaoId == missaoId)
            .OrderByDescending(item => item.CriadoEm)
            .ToListAsync(cancellationToken);

        return Ok(checkups.Select(item => item.ToResponse()));
    }

    private IQueryable<CheckupMissao> QueryCompleta()
    {
        return _context.CheckupsMissoes
            .Include(item => item.Missao)
            .Include(item => item.Alertas);
    }
}
