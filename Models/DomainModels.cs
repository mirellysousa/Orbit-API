using System.ComponentModel.DataAnnotations;

namespace Orbit.Api.Models;

public enum StatusMissao
{
    Apta = 1,
    EmAtencao = 2,
    Bloqueada = 3
}

public enum StatusOperacional
{
    Operacional = 1,
    Atencao = 2,
    Inoperante = 3
}

public enum TipoNave
{
    Nave = 1,
    Rover = 2
}

public enum TipoBase
{
    BaseLunar = 1,
    EstacaoOrbital = 2,
    HubManutencao = 3,
    BaseMarciana = 4
}

public enum TipoAlerta
{
    Saude = 1,
    Nave = 2,
    Base = 3
}

public enum NivelAlerta
{
    Moderado = 1,
    Critico = 2
}

public class Astronauta
{
    public int Id { get; set; }

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

    public ICollection<MissaoAstronauta> Missoes { get; set; } = new List<MissaoAstronauta>();
}

public class Nave
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    public TipoNave Tipo { get; set; }

    [Range(0, 100)]
    public int CombustivelBateria { get; set; }

    public decimal TemperaturaSistema { get; set; }

    public bool ComunicacaoOk { get; set; } = true;

    public StatusOperacional StatusOperacional { get; set; } = StatusOperacional.Operacional;

    public ICollection<Missao> Missoes { get; set; } = new List<Missao>();
}

public class BaseEspacial
{
    public int Id { get; set; }

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

    public ICollection<Missao> MissoesComoSuporte { get; set; } = new List<Missao>();
}

public class Missao
{
    public int Id { get; set; }

    [Required, MaxLength(140)]
    public string Nome { get; set; } = string.Empty;

    [Required, MaxLength(240)]
    public string Objetivo { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string Destino { get; set; } = string.Empty;

    public StatusMissao Status { get; set; } = StatusMissao.Apta;

    public int NaveId { get; set; }
    public Nave? Nave { get; set; }

    public int? BaseSuporteId { get; set; }
    public BaseEspacial? BaseSuporte { get; set; }

    public ICollection<MissaoAstronauta> Astronautas { get; set; } = new List<MissaoAstronauta>();
    public ICollection<CheckupMissao> Checkups { get; set; } = new List<CheckupMissao>();
}

public class MissaoAstronauta
{
    public int MissaoId { get; set; }
    public Missao? Missao { get; set; }

    public int AstronautaId { get; set; }
    public Astronauta? Astronauta { get; set; }

    [MaxLength(80)]
    public string PapelNaMissao { get; set; } = "Tripulante";
}

public class CheckupMissao
{
    public int Id { get; set; }

    public int MissaoId { get; set; }
    public Missao? Missao { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public StatusMissao Resultado { get; set; }

    [Range(0, 100)]
    public int PontuacaoRisco { get; set; }

    [Required, MaxLength(500)]
    public string Recomendacao { get; set; } = string.Empty;

    public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
}

public class Alerta
{
    public int Id { get; set; }

    public int CheckupMissaoId { get; set; }
    public CheckupMissao? CheckupMissao { get; set; }

    public TipoAlerta Tipo { get; set; }
    public NivelAlerta Nivel { get; set; }

    [Required, MaxLength(300)]
    public string Mensagem { get; set; } = string.Empty;
}
