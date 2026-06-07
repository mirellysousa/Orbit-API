using Orbit.Api.Dtos;
using Orbit.Api.Models;

namespace Orbit.Api.Mappings;

public static class DtoMappings
{
    public static AstronautaResponse ToResponse(this Astronauta astronauta)
    {
        return new AstronautaResponse(astronauta.Id, astronauta.Nome, astronauta.Funcao, astronauta.Fadiga, astronauta.Hidratacao, astronauta.Oxigenacao, astronauta.TemperaturaCorporal);
    }

    public static NaveResponse ToResponse(this Nave nave)
    {
        return new NaveResponse(nave.Id, nave.Nome, nave.Tipo, nave.CombustivelBateria, nave.TemperaturaSistema, nave.ComunicacaoOk, nave.StatusOperacional);
    }

    public static BaseEspacialResponse ToResponse(this BaseEspacial baseEspacial)
    {
        return new BaseEspacialResponse(baseEspacial.Id, baseEspacial.Nome, baseEspacial.Tipo, baseEspacial.Localizacao, baseEspacial.Energia, baseEspacial.Agua, baseEspacial.Oxigenio, baseEspacial.Medicamentos, baseEspacial.PecasManutencao, baseEspacial.StatusOperacional);
    }

    public static MissaoResponse ToResponse(this Missao missao)
    {
        var astronautas = missao.Astronautas
            .Where(vinculo => vinculo.Astronauta is not null)
            .Select(vinculo => new AstronautaResumo(vinculo.Astronauta!.Id, vinculo.Astronauta.Nome, vinculo.Astronauta.Funcao))
            .ToList();

        var nave = missao.Nave is null ? null : new NaveResumo(missao.Nave.Id, missao.Nave.Nome, missao.Nave.Tipo);
        var baseSuporte = missao.BaseSuporte is null ? null : new BaseResumo(missao.BaseSuporte.Id, missao.BaseSuporte.Nome, missao.BaseSuporte.Tipo, missao.BaseSuporte.Localizacao);

        return new MissaoResponse(missao.Id, missao.Nome, missao.Objetivo, missao.Destino, missao.Status, nave, baseSuporte, astronautas);
    }

    public static CheckupResponse ToResponse(this CheckupMissao checkup)
    {
        var alertas = checkup.Alertas
            .Select(alerta => new AlertaResponse(alerta.Id, alerta.Tipo, alerta.Nivel, alerta.Mensagem))
            .ToList();

        return new CheckupResponse(
            checkup.Id,
            checkup.MissaoId,
            checkup.Missao?.Nome ?? string.Empty,
            checkup.Resultado,
            checkup.PontuacaoRisco,
            checkup.Recomendacao,
            alertas);
    }
}
