using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orbit.Api.Data;
using Orbit.Api.Dtos;
using Orbit.Api.Mappings;
using Orbit.Api.Models;

namespace Orbit.Api.Controllers;

[ApiController]
[Route("api/naves")]
public class NavesController : ControllerBase
{
    private readonly OrbitDbContext _context;

    public NavesController(OrbitDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NaveResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var naves = await _context.Naves.AsNoTracking().OrderBy(item => item.Nome).ToListAsync(cancellationToken);
        return Ok(naves.Select(item => item.ToResponse()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NaveResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var nave = await _context.Naves.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return nave is null ? NotFound() : Ok(nave.ToResponse());
    }

    [HttpPost]
    public async Task<ActionResult<NaveResponse>> Create(NaveRequest request, CancellationToken cancellationToken)
    {
        var nave = new Nave();
        ApplyRequest(nave, request);

        _context.Naves.Add(nave);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = nave.Id }, nave.ToResponse());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<NaveResponse>> Update(int id, NaveRequest request, CancellationToken cancellationToken)
    {
        var nave = await _context.Naves.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (nave is null)
        {
            return NotFound();
        }

        ApplyRequest(nave, request);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(nave.ToResponse());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var nave = await _context.Naves.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (nave is null)
        {
            return NotFound();
        }

        var vinculada = await _context.Missoes.AnyAsync(item => item.NaveId == id, cancellationToken);
        if (vinculada)
        {
            return Conflict("Nao e possivel excluir nave vinculada a uma missao.");
        }

        _context.Naves.Remove(nave);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static void ApplyRequest(Nave nave, NaveRequest request)
    {
        nave.Nome = request.Nome.Trim();
        nave.Tipo = request.Tipo;
        nave.CombustivelBateria = request.CombustivelBateria;
        nave.TemperaturaSistema = request.TemperaturaSistema;
        nave.ComunicacaoOk = request.ComunicacaoOk;
        nave.StatusOperacional = request.StatusOperacional;
    }
}
