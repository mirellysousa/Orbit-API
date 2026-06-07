using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orbit.Api.Data;
using Orbit.Api.Dtos;
using Orbit.Api.Mappings;
using Orbit.Api.Models;

namespace Orbit.Api.Controllers;

[ApiController]
[Route("api/bases-espaciais")]
public class BasesEspaciaisController : ControllerBase
{
    private readonly OrbitDbContext _context;

    public BasesEspaciaisController(OrbitDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BaseEspacialResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var bases = await _context.BasesEspaciais.AsNoTracking().OrderBy(item => item.Nome).ToListAsync(cancellationToken);
        return Ok(bases.Select(item => item.ToResponse()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BaseEspacialResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var baseEspacial = await _context.BasesEspaciais.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return baseEspacial is null ? NotFound() : Ok(baseEspacial.ToResponse());
    }

    [HttpPost]
    public async Task<ActionResult<BaseEspacialResponse>> Create(BaseEspacialRequest request, CancellationToken cancellationToken)
    {
        var baseEspacial = new BaseEspacial();
        ApplyRequest(baseEspacial, request);

        _context.BasesEspaciais.Add(baseEspacial);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = baseEspacial.Id }, baseEspacial.ToResponse());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BaseEspacialResponse>> Update(int id, BaseEspacialRequest request, CancellationToken cancellationToken)
    {
        var baseEspacial = await _context.BasesEspaciais.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (baseEspacial is null)
        {
            return NotFound();
        }

        ApplyRequest(baseEspacial, request);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(baseEspacial.ToResponse());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var baseEspacial = await _context.BasesEspaciais.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (baseEspacial is null)
        {
            return NotFound();
        }

        var vinculada = await _context.Missoes.AnyAsync(item => item.BaseSuporteId == id, cancellationToken);
        if (vinculada)
        {
            return Conflict("Nao e possivel excluir base vinculada a uma missao.");
        }

        _context.BasesEspaciais.Remove(baseEspacial);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static void ApplyRequest(BaseEspacial baseEspacial, BaseEspacialRequest request)
    {
        baseEspacial.Nome = request.Nome.Trim();
        baseEspacial.Tipo = request.Tipo;
        baseEspacial.Localizacao = request.Localizacao.Trim();
        baseEspacial.Energia = request.Energia;
        baseEspacial.Agua = request.Agua;
        baseEspacial.Oxigenio = request.Oxigenio;
        baseEspacial.Medicamentos = request.Medicamentos;
        baseEspacial.PecasManutencao = request.PecasManutencao;
        baseEspacial.StatusOperacional = request.StatusOperacional;
    }
}
