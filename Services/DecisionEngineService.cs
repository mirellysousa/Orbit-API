using Microsoft.EntityFrameworkCore;
using Orbit.Api.Data;
using Orbit.Api.Models;

namespace Orbit.Api.Services;

public class DecisionEngineService : IDecisionEngineService
{
    private readonly OrbitDbContext _context;

    public DecisionEngineService(OrbitDbContext context)
    {
        _context = context;
    }

    public async Task<CheckupMissao?> AvaliarMissaoAsync(int missaoId, CancellationToken cancellationToken)
    {
        var missao = await _context.Missoes
            .Include(item => item.Nave)
            .Include(item => item.BaseSuporte)
            .Include(item => item.Astronautas)
                .ThenInclude(item => item.Astronauta)
            .FirstOrDefaultAsync(item => item.Id == missaoId, cancellationToken);

        if (missao is null || missao.Nave is null)
        {
            return null;
        }

        var alertas = new List<Alerta>();
        var risco = 0;

        risco += AvaliarAstronautas(missao, alertas);
        risco += AvaliarNave(missao.Nave, alertas);
        risco += AvaliarBase(missao.BaseSuporte, alertas);

        risco = Math.Clamp(risco, 0, 100);
        var resultado = DefinirResultado(alertas, risco);

        var checkup = new CheckupMissao
        {
            MissaoId = missao.Id,
            Resultado = resultado,
            PontuacaoRisco = risco,
            Recomendacao = MontarRecomendacao(resultado, alertas),
            Alertas = alertas
        };

        missao.Status = resultado;
        _context.CheckupsMissoes.Add(checkup);
        await _context.SaveChangesAsync(cancellationToken);

        return await _context.CheckupsMissoes
            .AsNoTracking()
            .Include(item => item.Missao)
            .Include(item => item.Alertas)
            .FirstAsync(item => item.Id == checkup.Id, cancellationToken);
    }

    private static int AvaliarAstronautas(Missao missao, ICollection<Alerta> alertas)
    {
        var risco = 0;

        foreach (var vinculo in missao.Astronautas)
        {
            var astronauta = vinculo.Astronauta;
            if (astronauta is null)
            {
                continue;
            }

            if (astronauta.Fadiga >= 85)
            {
                risco += Adicionar(alertas, TipoAlerta.Saude, NivelAlerta.Critico, $"{astronauta.Nome} esta com fadiga critica.");
            }
            else if (astronauta.Fadiga >= 70)
            {
                risco += Adicionar(alertas, TipoAlerta.Saude, NivelAlerta.Moderado, $"{astronauta.Nome} esta com fadiga elevada.");
            }

            if (astronauta.Hidratacao < 50)
            {
                risco += Adicionar(alertas, TipoAlerta.Saude, NivelAlerta.Moderado, $"{astronauta.Nome} precisa de reposicao de hidratacao.");
            }

            if (astronauta.Oxigenacao < 92)
            {
                risco += Adicionar(alertas, TipoAlerta.Saude, NivelAlerta.Critico, $"{astronauta.Nome} esta com oxigenacao abaixo do limite seguro.");
            }
            else if (astronauta.Oxigenacao < 95)
            {
                risco += Adicionar(alertas, TipoAlerta.Saude, NivelAlerta.Moderado, $"{astronauta.Nome} esta com oxigenacao em observacao.");
            }

            if (astronauta.TemperaturaCorporal >= 38)
            {
                risco += Adicionar(alertas, TipoAlerta.Saude, NivelAlerta.Critico, $"{astronauta.Nome} esta com temperatura corporal critica.");
            }
        }

        return risco;
    }

    private static int AvaliarNave(Nave nave, ICollection<Alerta> alertas)
    {
        var risco = 0;

        if (!nave.ComunicacaoOk)
        {
            risco += Adicionar(alertas, TipoAlerta.Nave, NivelAlerta.Critico, "Comunicacao da nave indisponivel.");
        }

        if (nave.CombustivelBateria < 10)
        {
            risco += Adicionar(alertas, TipoAlerta.Nave, NivelAlerta.Critico, "Combustivel ou bateria abaixo do limite minimo.");
        }
        else if (nave.CombustivelBateria < 30)
        {
            risco += Adicionar(alertas, TipoAlerta.Nave, NivelAlerta.Moderado, "Combustivel ou bateria em nivel de atencao.");
        }

        if (nave.TemperaturaSistema >= 90)
        {
            risco += Adicionar(alertas, TipoAlerta.Nave, NivelAlerta.Critico, "Temperatura da nave em nivel critico.");
        }
        else if (nave.TemperaturaSistema >= 75)
        {
            risco += Adicionar(alertas, TipoAlerta.Nave, NivelAlerta.Moderado, "Temperatura da nave acima do ideal.");
        }

        if (nave.StatusOperacional == StatusOperacional.Inoperante)
        {
            risco += Adicionar(alertas, TipoAlerta.Nave, NivelAlerta.Critico, "Nave marcada como inoperante.");
        }

        return risco;
    }

    private static int AvaliarBase(BaseEspacial? baseSuporte, ICollection<Alerta> alertas)
    {
        if (baseSuporte is null)
        {
            return Adicionar(alertas, TipoAlerta.Base, NivelAlerta.Moderado, "Missao sem base de suporte definida.");
        }

        var risco = 0;

        if (baseSuporte.StatusOperacional == StatusOperacional.Inoperante)
        {
            risco += Adicionar(alertas, TipoAlerta.Base, NivelAlerta.Critico, "Base de suporte inoperante.");
        }

        if (baseSuporte.Oxigenio < 40 || baseSuporte.Agua < 40 || baseSuporte.Energia < 40)
        {
            risco += Adicionar(alertas, TipoAlerta.Base, NivelAlerta.Critico, "Base com recurso essencial em nivel critico.");
        }

        if (baseSuporte.Medicamentos < 50 || baseSuporte.PecasManutencao < 50)
        {
            risco += Adicionar(alertas, TipoAlerta.Base, NivelAlerta.Moderado, "Base com medicamentos ou pecas em nivel de atencao.");
        }

        return risco;
    }

    private static StatusMissao DefinirResultado(IReadOnlyCollection<Alerta> alertas, int risco)
    {
        if (alertas.Any(item => item.Nivel == NivelAlerta.Critico) || risco >= 70)
        {
            return StatusMissao.Bloqueada;
        }

        if (alertas.Any(item => item.Nivel == NivelAlerta.Moderado) || risco >= 30)
        {
            return StatusMissao.EmAtencao;
        }

        return StatusMissao.Apta;
    }

    private static string MontarRecomendacao(StatusMissao resultado, IReadOnlyCollection<Alerta> alertas)
    {
        if (resultado == StatusMissao.Apta)
        {
            return "Missao apta para continuidade com monitoramento de rotina.";
        }

        var temSaude = alertas.Any(item => item.Tipo == TipoAlerta.Saude);
        var temNave = alertas.Any(item => item.Tipo == TipoAlerta.Nave);

        if (resultado == StatusMissao.Bloqueada)
        {
            if (temSaude && temNave)
            {
                return "Bloquear missao e priorizar suporte medico e manutencao tecnica.";
            }

            return temSaude
                ? "Bloquear missao e priorizar suporte medico."
                : "Bloquear missao e priorizar manutencao ou reposicao de recursos.";
        }

        return "Missao em atencao. Continuar apenas com monitoramento reforcado e novo check-up.";
    }

    private static int Adicionar(ICollection<Alerta> alertas, TipoAlerta tipo, NivelAlerta nivel, string mensagem)
    {
        alertas.Add(new Alerta { Tipo = tipo, Nivel = nivel, Mensagem = mensagem });
        return nivel == NivelAlerta.Critico ? 25 : 10;
    }
}
