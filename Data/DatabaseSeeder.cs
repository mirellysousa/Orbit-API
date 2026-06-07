using Microsoft.EntityFrameworkCore;
using Orbit.Api.Models;

namespace Orbit.Api.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrbitDbContext>();

        var applyMigrations = configuration.GetValue("Database:ApplyMigrationsOnStartup", true);
        if (applyMigrations)
        {
            await context.Database.MigrateAsync();
        }

        var seedOnStartup = configuration.GetValue("Database:SeedOnStartup", true);
        if (!seedOnStartup)
        {
            return;
        }

        if (await context.Missoes.AnyAsync())
        {
            return;
        }

        var comandante = new Astronauta
        {
            Nome = "Helena Duarte",
            Funcao = "Comandante",
            Fadiga = 62,
            Hidratacao = 74,
            Oxigenacao = 97,
            TemperaturaCorporal = 36.7m
        };

        var engenheira = new Astronauta
        {
            Nome = "Lia Campos",
            Funcao = "Engenheira de sistemas",
            Fadiga = 78,
            Hidratacao = 55,
            Oxigenacao = 94,
            TemperaturaCorporal = 37.6m
        };

        var nave = new Nave
        {
            Nome = "Orbit-01",
            Tipo = TipoNave.Nave,
            CombustivelBateria = 28,
            TemperaturaSistema = 76.5m,
            ComunicacaoOk = true,
            StatusOperacional = StatusOperacional.Atencao
        };

        var baseLunar = new BaseEspacial
        {
            Nome = "Base Lunar Alpha",
            Tipo = TipoBase.BaseLunar,
            Localizacao = "Polo Sul Lunar",
            Energia = 82,
            Agua = 67,
            Oxigenio = 76,
            Medicamentos = 71,
            PecasManutencao = 80,
            StatusOperacional = StatusOperacional.Operacional
        };

        var missao = new Missao
        {
            Nome = "Supply-04",
            Objetivo = "Transportar suprimentos para uma base lunar e validar condicoes de retorno.",
            Destino = "Lua",
            Nave = nave,
            BaseSuporte = baseLunar,
            Astronautas =
            {
                new MissaoAstronauta { Astronauta = comandante, PapelNaMissao = "Comandante" },
                new MissaoAstronauta { Astronauta = engenheira, PapelNaMissao = "Especialista tecnica" }
            }
        };

        context.AddRange(comandante, engenheira, nave, baseLunar, missao);
        await context.SaveChangesAsync();
    }
}
