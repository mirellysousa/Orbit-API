using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbit.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ASTRONAUTA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NOME = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    FUNCAO = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    FADIGA = table.Column<int>(type: "INTEGER", nullable: false),
                    HIDRATACAO = table.Column<int>(type: "INTEGER", nullable: false),
                    OXIGENACAO = table.Column<int>(type: "INTEGER", nullable: false),
                    TEMPERATURA_CORPORAL = table.Column<decimal>(type: "TEXT", precision: 4, scale: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASTRONAUTA", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BASE_ESPACIAL",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NOME = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    TIPO = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    LOCALIZACAO = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    ENERGIA = table.Column<int>(type: "INTEGER", nullable: false),
                    AGUA = table.Column<int>(type: "INTEGER", nullable: false),
                    OXIGENIO = table.Column<int>(type: "INTEGER", nullable: false),
                    MEDICAMENTOS = table.Column<int>(type: "INTEGER", nullable: false),
                    PECAS_MANUTENCAO = table.Column<int>(type: "INTEGER", nullable: false),
                    STATUS_OPERACIONAL = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BASE_ESPACIAL", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NAVE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NOME = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    TIPO = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    COMBUSTIVEL_BATERIA = table.Column<int>(type: "INTEGER", nullable: false),
                    TEMPERATURA_SISTEMA = table.Column<decimal>(type: "TEXT", precision: 5, scale: 1, nullable: false),
                    COMUNICACAO_OK = table.Column<string>(type: "TEXT", maxLength: 1, nullable: false),
                    STATUS_OPERACIONAL = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MISSAO",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NOME = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    OBJETIVO = table.Column<string>(type: "TEXT", maxLength: 240, nullable: false),
                    DESTINO = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    STATUS = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    NAVE_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    BASE_SUPORTE_ID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MISSAO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MISSAO_BASE_ESPACIAL_BASE_SUPORTE_ID",
                        column: x => x.BASE_SUPORTE_ID,
                        principalTable: "BASE_ESPACIAL",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MISSAO_NAVE_NAVE_ID",
                        column: x => x.NAVE_ID,
                        principalTable: "NAVE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CHECKUP_MISSAO",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MISSAO_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    CRIADO_EM = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RESULTADO = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    PONTUACAO_RISCO = table.Column<int>(type: "INTEGER", nullable: false),
                    RECOMENDACAO = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHECKUP_MISSAO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CHECKUP_MISSAO_MISSAO_MISSAO_ID",
                        column: x => x.MISSAO_ID,
                        principalTable: "MISSAO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MISSAO_ASTRONAUTA",
                columns: table => new
                {
                    MISSAO_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    ASTRONAUTA_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    PAPEL_NA_MISSAO = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MISSAO_ASTRONAUTA", x => new { x.MISSAO_ID, x.ASTRONAUTA_ID });
                    table.ForeignKey(
                        name: "FK_MISSAO_ASTRONAUTA_ASTRONAUTA_ASTRONAUTA_ID",
                        column: x => x.ASTRONAUTA_ID,
                        principalTable: "ASTRONAUTA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MISSAO_ASTRONAUTA_MISSAO_MISSAO_ID",
                        column: x => x.MISSAO_ID,
                        principalTable: "MISSAO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ALERTA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CHECKUP_MISSAO_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    TIPO = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    NIVEL = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    MENSAGEM = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALERTA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ALERTA_CHECKUP_MISSAO_CHECKUP_MISSAO_ID",
                        column: x => x.CHECKUP_MISSAO_ID,
                        principalTable: "CHECKUP_MISSAO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ALERTA_CHECKUP_MISSAO_ID",
                table: "ALERTA",
                column: "CHECKUP_MISSAO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CHECKUP_MISSAO_MISSAO_ID",
                table: "CHECKUP_MISSAO",
                column: "MISSAO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MISSAO_BASE_SUPORTE_ID",
                table: "MISSAO",
                column: "BASE_SUPORTE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MISSAO_NAVE_ID",
                table: "MISSAO",
                column: "NAVE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MISSAO_ASTRONAUTA_ASTRONAUTA_ID",
                table: "MISSAO_ASTRONAUTA",
                column: "ASTRONAUTA_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALERTA");

            migrationBuilder.DropTable(
                name: "MISSAO_ASTRONAUTA");

            migrationBuilder.DropTable(
                name: "CHECKUP_MISSAO");

            migrationBuilder.DropTable(
                name: "ASTRONAUTA");

            migrationBuilder.DropTable(
                name: "MISSAO");

            migrationBuilder.DropTable(
                name: "BASE_ESPACIAL");

            migrationBuilder.DropTable(
                name: "NAVE");
        }
    }
}
