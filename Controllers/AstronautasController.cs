using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orbit.Api.Data;
using Orbit.Api.Dtos;
using Orbit.Api.Mappings;
using Orbit.Api.Models;

namespace Orbit.Api.Controllers;

[ApiController]
[Route("api/astronautas")]
public class AstronautasController : ControllerBase
{
    private readonly OrbitDbContext _context;

    public AstronautasController(OrbitDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AstronautaResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var astronautas = await _context.Astronautas.AsNoTracking().OrderBy(item => item.Nome).ToListAsync(cancellationToken);
        return Ok(astronautas.Select(item => item.ToResponse()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AstronautaResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var astronauta = await _context.Astronautas.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return astronauta is null ? NotFound() : Ok(astronauta.ToResponse());
    }

    [HttpPost]
    public async Task<ActionResult<AstronautaResponse>> Create(AstronautaRequest request, CancellationToken cancellationToken)
    {
        var astronauta = new Astronauta();
        ApplyRequest(astronauta, request);

        _context.Astronautas.Add(astronauta);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = astronauta.Id }, astronauta.ToResponse());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AstronautaResponse>> Update(int id, AstronautaRequest request, CancellationToken cancellationToken)
    {
        var astronauta = await _context.Astronautas.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (astronauta is null)
        {
            return NotFound();
        }

        ApplyRequest(astronauta, request);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(astronauta.ToResponse());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var astronauta = await _context.Astronautas.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (astronauta is null)
        {
            return NotFound();
        }

        var vinculado = await _context.MissoesAstronautas.AnyAsync(item => item.AstronautaId == id, cancellationToken);
        if (vinculado)
        {
            return Conflict("Nao e possivel excluir astronauta vinculado a uma missao.");
        }

        _context.Astronautas.Remove(astronauta);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static void ApplyRequest(Astronauta astronauta, AstronautaRequest request)
    {
        astronauta.Nome = request.Nome.Trim();
        astronauta.Funcao = request.Funcao.Trim();
        astronauta.Fadiga = request.Fadiga;
        astronauta.Hidratacao = request.Hidratacao;
        astronauta.Oxigenacao = request.Oxigenacao;
        astronauta.TemperaturaCorporal = request.TemperaturaCorporal;
    }
}
