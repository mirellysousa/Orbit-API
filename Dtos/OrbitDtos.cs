using System.ComponentModel.DataAnnotations;
using Orbit.Api.Models;

namespace Orbit.Api.Dtos;

public class AstronautaRequest
{
    [Required, MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string Funcao { get; set; } = string.Empty;

    [Range(0, 100)]
    public int Fadiga { get; set; }

    [Range(0, 100)]
    public int Hidratacao { get; set; }

    [Range(0, 100)]
    public int Oxigenacao { get; set; }

    [Range(30, 45)]
    public decimal TemperaturaCorporal { get; set; }
}

public class NaveRequest
{
    [Required, MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    public TipoNave Tipo { get; set; }

    [Range(0, 100)]
    public int CombustivelBateria { get; set; }

    public decimal TemperaturaSistema { get; set; }

    public bool ComunicacaoOk { get; set; } = true;

    public StatusOperacional StatusOperacional { get; set; } = StatusOperacional.Operacional;
}

public class BaseEspacialRequest
{
    [Required, MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    public TipoBase Tipo { get; set; }

    [Required, MaxLength(120)]
    public string Localizacao { get; set; } = string.Empty;

    [Range(0, 100)]
    public int Energia { get; set; }

    [Range(0, 100)]
    public int Agua { get; set; }

    [Range(0, 100)]
    public int Oxigenio { get; set; }

    [Range(0, 100)]
    public int Medicamentos { get; set; }

    [Range(0, 100)]
    public int PecasManutencao { get; set; }

    public StatusOperacional StatusOperacional { get; set; } = StatusOperacional.Operacional;
}

public class MissaoRequest
{
    [Required, MaxLength(140)]
    public string Nome { get; set; } = string.Empty;

    [Required, MaxLength(240)]
    public string Objetivo { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string Destino { get; set; } = string.Empty;

    public int NaveId { get; set; }
    public int? BaseSuporteId { get; set; }
    public List<int> AstronautaIds { get; set; } = new();
}

public class CheckupRequest
{
    public int MissaoId { get; set; }
}

public record AstronautaResponse(int Id, string Nome, string Funcao, int Fadiga, int Hidratacao, int Oxigenacao, decimal TemperaturaCorporal);
public record NaveResponse(int Id, string Nome, TipoNave Tipo, int CombustivelBateria, decimal TemperaturaSistema, bool ComunicacaoOk, StatusOperacional StatusOperacional);
public record BaseEspacialResponse(int Id, string Nome, TipoBase Tipo, string Localizacao, int Energia, int Agua, int Oxigenio, int Medicamentos, int PecasManutencao, StatusOperacional StatusOperacional);
public record AstronautaResumo(int Id, string Nome, string Funcao);
public record NaveResumo(int Id, string Nome, TipoNave Tipo);
public record BaseResumo(int Id, string Nome, TipoBase Tipo, string Localizacao);
public record MissaoResponse(int Id, string Nome, string Objetivo, string Destino, StatusMissao Status, NaveResumo? Nave, BaseResumo? BaseSuporte, IReadOnlyCollection<AstronautaResumo> Astronautas);
public record AlertaResponse(int Id, TipoAlerta Tipo, NivelAlerta Nivel, string Mensagem);
public record CheckupResponse(int Id, int MissaoId, string NomeMissao, StatusMissao Resultado, int PontuacaoRisco, string Recomendacao, IReadOnlyCollection<AlertaResponse> Alertas);
