using Microsoft.EntityFrameworkCore;
using Orbit.Api.Models;

namespace Orbit.Api.Data;

public class OrbitDbContext : DbContext
{
    public OrbitDbContext(DbContextOptions<OrbitDbContext> options) : base(options)
    {
    }

    public DbSet<Astronauta> Astronautas => Set<Astronauta>();
    public DbSet<Nave> Naves => Set<Nave>();
    public DbSet<BaseEspacial> BasesEspaciais => Set<BaseEspacial>();
    public DbSet<Missao> Missoes => Set<Missao>();
    public DbSet<MissaoAstronauta> MissoesAstronautas => Set<MissaoAstronauta>();
    public DbSet<CheckupMissao> CheckupsMissoes => Set<CheckupMissao>();
    public DbSet<Alerta> Alertas => Set<Alerta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Astronauta>(entity =>
        {
            entity.ToTable("ASTRONAUTA");
            entity.Property(item => item.Id).HasColumnName("ID");
            entity.Property(item => item.Nome).HasColumnName("NOME").HasMaxLength(120).IsRequired();
            entity.Property(item => item.Funcao).HasColumnName("FUNCAO").HasMaxLength(80).IsRequired();
            entity.Property(item => item.Fadiga).HasColumnName("FADIGA");
            entity.Property(item => item.Hidratacao).HasColumnName("HIDRATACAO");
            entity.Property(item => item.Oxigenacao).HasColumnName("OXIGENACAO");
            entity.Property(item => item.TemperaturaCorporal).HasColumnName("TEMPERATURA_CORPORAL").HasPrecision(4, 1);
        });

        modelBuilder.Entity<Nave>(entity =>
        {
            entity.ToTable("NAVE");
            entity.Property(item => item.Id).HasColumnName("ID");
            entity.Property(item => item.Nome).HasColumnName("NOME").HasMaxLength(120).IsRequired();
            entity.Property(item => item.Tipo).HasColumnName("TIPO").HasConversion<string>().HasMaxLength(30);
            entity.Property(item => item.CombustivelBateria).HasColumnName("COMBUSTIVEL_BATERIA");
            entity.Property(item => item.TemperaturaSistema).HasColumnName("TEMPERATURA_SISTEMA").HasPrecision(5, 1);
            entity.Property(item => item.ComunicacaoOk)
                .HasColumnName("COMUNICACAO_OK")
                .HasConversion(item => item ? "S" : "N", item => item == "S")
                .HasMaxLength(1);
            entity.Property(item => item.StatusOperacional).HasColumnName("STATUS_OPERACIONAL").HasConversion<string>().HasMaxLength(30);
        });

        modelBuilder.Entity<BaseEspacial>(entity =>
        {
            entity.ToTable("BASE_ESPACIAL");
            entity.Property(item => item.Id).HasColumnName("ID");
            entity.Property(item => item.Nome).HasColumnName("NOME").HasMaxLength(120).IsRequired();
            entity.Property(item => item.Tipo).HasColumnName("TIPO").HasConversion<string>().HasMaxLength(30);
            entity.Property(item => item.Localizacao).HasColumnName("LOCALIZACAO").HasMaxLength(120).IsRequired();
            entity.Property(item => item.Energia).HasColumnName("ENERGIA");
            entity.Property(item => item.Agua).HasColumnName("AGUA");
            entity.Property(item => item.Oxigenio).HasColumnName("OXIGENIO");
            entity.Property(item => item.Medicamentos).HasColumnName("MEDICAMENTOS");
            entity.Property(item => item.PecasManutencao).HasColumnName("PECAS_MANUTENCAO");
            entity.Property(item => item.StatusOperacional).HasColumnName("STATUS_OPERACIONAL").HasConversion<string>().HasMaxLength(30);
        });

        modelBuilder.Entity<Missao>(entity =>
        {
            entity.ToTable("MISSAO");
            entity.Property(item => item.Id).HasColumnName("ID");
            entity.Property(item => item.Nome).HasColumnName("NOME").HasMaxLength(140).IsRequired();
            entity.Property(item => item.Objetivo).HasColumnName("OBJETIVO").HasMaxLength(240).IsRequired();
            entity.Property(item => item.Destino).HasColumnName("DESTINO").HasMaxLength(120).IsRequired();
            entity.Property(item => item.Status).HasColumnName("STATUS").HasConversion<string>().HasMaxLength(30);
            entity.Property(item => item.NaveId).HasColumnName("NAVE_ID");
            entity.Property(item => item.BaseSuporteId).HasColumnName("BASE_SUPORTE_ID");

            entity.HasOne(item => item.Nave)
                .WithMany(item => item.Missoes)
                .HasForeignKey(item => item.NaveId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(item => item.BaseSuporte)
                .WithMany(item => item.MissoesComoSuporte)
                .HasForeignKey(item => item.BaseSuporteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MissaoAstronauta>(entity =>
        {
            entity.ToTable("MISSAO_ASTRONAUTA");
            entity.HasKey(item => new { item.MissaoId, item.AstronautaId });
            entity.Property(item => item.MissaoId).HasColumnName("MISSAO_ID");
            entity.Property(item => item.AstronautaId).HasColumnName("ASTRONAUTA_ID");
            entity.Property(item => item.PapelNaMissao).HasColumnName("PAPEL_NA_MISSAO").HasMaxLength(80);

            entity.HasOne(item => item.Missao)
                .WithMany(item => item.Astronautas)
                .HasForeignKey(item => item.MissaoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(item => item.Astronauta)
                .WithMany(item => item.Missoes)
                .HasForeignKey(item => item.AstronautaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CheckupMissao>(entity =>
        {
            entity.ToTable("CHECKUP_MISSAO");
            entity.Property(item => item.Id).HasColumnName("ID");
            entity.Property(item => item.MissaoId).HasColumnName("MISSAO_ID");
            entity.Property(item => item.CriadoEm).HasColumnName("CRIADO_EM");
            entity.Property(item => item.Resultado).HasColumnName("RESULTADO").HasConversion<string>().HasMaxLength(30);
            entity.Property(item => item.PontuacaoRisco).HasColumnName("PONTUACAO_RISCO");
            entity.Property(item => item.Recomendacao).HasColumnName("RECOMENDACAO").HasMaxLength(500).IsRequired();

            entity.HasOne(item => item.Missao)
                .WithMany(item => item.Checkups)
                .HasForeignKey(item => item.MissaoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Alerta>(entity =>
        {
            entity.ToTable("ALERTA");
            entity.Property(item => item.Id).HasColumnName("ID");
            entity.Property(item => item.CheckupMissaoId).HasColumnName("CHECKUP_MISSAO_ID");
            entity.Property(item => item.Tipo).HasColumnName("TIPO").HasConversion<string>().HasMaxLength(30);
            entity.Property(item => item.Nivel).HasColumnName("NIVEL").HasConversion<string>().HasMaxLength(30);
            entity.Property(item => item.Mensagem).HasColumnName("MENSAGEM").HasMaxLength(300).IsRequired();

            entity.HasOne(item => item.CheckupMissao)
                .WithMany(item => item.Alertas)
                .HasForeignKey(item => item.CheckupMissaoId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
