using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orbit.Api.Data;
using Orbit.Api.Dtos;
using Orbit.Api.Mappings;
using Orbit.Api.Models;

namespace Orbit.Api.Controllers;

[ApiController]
[Route("api/missoes")]
public class MissoesController : ControllerBase
{
    private readonly OrbitDbContext _context;

    public MissoesController(OrbitDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MissaoResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var missoes = await QueryCompleta().AsNoTracking().OrderBy(item => item.Nome).ToListAsync(cancellationToken);
        return Ok(missoes.Select(item => item.ToResponse()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MissaoResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var missao = await QueryCompleta().AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return missao is null ? NotFound() : Ok(missao.ToResponse());
    }

    [HttpPost]
    public async Task<ActionResult<MissaoResponse>> Create(MissaoRequest request, CancellationToken cancellationToken)
    {
        var erro = await ValidarRelacionamentosAsync(request, cancellationToken);
        if (erro is not null)
        {
            return erro;
        }

        var missao = new Missao();
        ApplyRequest(missao, request);

        _context.Missoes.Add(missao);
        await _context.SaveChangesAsync(cancellationToken);

        var criada = await QueryCompleta().AsNoTracking().FirstAsync(item => item.Id == missao.Id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = missao.Id }, criada.ToResponse());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MissaoResponse>> Update(int id, MissaoRequest request, CancellationToken cancellationToken)
    {
        var missao = await _context.Missoes
            .Include(item => item.Astronautas)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (missao is null)
        {
            return NotFound();
        }

        var erro = await ValidarRelacionamentosAsync(request, cancellationToken);
        if (erro is not null)
        {
            return erro;
        }

        ApplyRequest(missao, request);
        await _context.SaveChangesAsync(cancellationToken);

        var atualizada = await QueryCompleta().AsNoTracking().FirstAsync(item => item.Id == id, cancellationToken);
        return Ok(atualizada.ToResponse());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var missao = await _context.Missoes.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (missao is null)
        {
            return NotFound();
        }

        _context.Missoes.Remove(missao);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private IQueryable<Missao> QueryCompleta()
    {
        return _context.Missoes
            .Include(item => item.Nave)
            .Include(item => item.BaseSuporte)
            .Include(item => item.Astronautas)
                .ThenInclude(item => item.Astronauta);
    }

    private async Task<ActionResult?> ValidarRelacionamentosAsync(MissaoRequest request, CancellationToken cancellationToken)
    {
        if (!await _context.Naves.AnyAsync(item => item.Id == request.NaveId, cancellationToken))
        {
            return BadRequest("NaveId informado nao existe.");
        }

        if (request.BaseSuporteId.HasValue &&
            !await _context.BasesEspaciais.AnyAsync(item => item.Id == request.BaseSuporteId.Value, cancellationToken))
        {
            return BadRequest("BaseSuporteId informado nao existe.");
        }

        var astronautaIds = request.AstronautaIds.Distinct().ToList();
        if (astronautaIds.Count == 0)
        {
            return BadRequest("Informe pelo menos um astronauta para a missao.");
        }

        var existentes = await _context.Astronautas
            .Where(item => astronautaIds.Contains(item.Id))
            .Select(item => item.Id)
            .ToListAsync(cancellationToken);

        var invalidos = astronautaIds.Except(existentes).ToList();
        return invalidos.Count == 0 ? null : BadRequest($"AstronautaIds invalidos: {string.Join(", ", invalidos)}.");
    }

    private static void ApplyRequest(Missao missao, MissaoRequest request)
    {
        missao.Nome = request.Nome.Trim();
        missao.Objetivo = request.Objetivo.Trim();
        missao.Destino = request.Destino.Trim();
        missao.NaveId = request.NaveId;
        missao.BaseSuporteId = request.BaseSuporteId;
        missao.Astronautas.Clear();

        foreach (var astronautaId in request.AstronautaIds.Distinct())
        {
            missao.Astronautas.Add(new MissaoAstronauta { AstronautaId = astronautaId });
        }
    }
}
