using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class CRIARTABELAPERMISSAO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "controle_acesso_options",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GrupoSituacao = table.Column<int>(type: "integer", nullable: false),
                    SistemaId = table.Column<int>(type: "integer", nullable: false),
                    ModuloId = table.Column<int>(type: "integer", nullable: false),
                    CacheDuracaoMinutos = table.Column<int>(type: "integer", nullable: false),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoRF = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: false),
                    CriadoRF = table.Column<string>(type: "text", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_controle_acesso_options", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "perfil_info",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PermiteConsultar = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PermiteInserir = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PermiteAlterar = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PermiteExcluir = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TipoValidacao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ConsultarAbrangencia = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AcessoIrrestrito = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoRF = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: false),
                    CriadoRF = table.Column<string>(type: "text", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfil_info", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "perfil_configuracao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TipoValidacao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, comment: "Valores esperados: Regencia, UE, AcessoTotal"),
                    ConsultarAbrangencia = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AcessoIrrestrito = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ControleAcessoOptionsId = table.Column<int>(type: "integer", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoRF = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: false),
                    CriadoRF = table.Column<string>(type: "text", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfil_configuracao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_perfil_configuracao_controle_acesso_options_ControleAcessoO~",
                        column: x => x.ControleAcessoOptionsId,
                        principalTable: "controle_acesso_options",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_perfil_configuracao_ControleAcessoOptionsId",
                table: "perfil_configuracao",
                column: "ControleAcessoOptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_perfil_info_Codigo",
                table: "perfil_info",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "perfil_configuracao");

            migrationBuilder.DropTable(
                name: "perfil_info");

            migrationBuilder.DropTable(
                name: "controle_acesso_options");
        }
    }
}
