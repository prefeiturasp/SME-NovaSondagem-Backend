using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class remover_tabela_nao_utilizada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "perfil_info");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "perfil_info",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    acesso_irrestrito = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoRF = table.Column<string>(type: "text", nullable: true),
                    codigo = table.Column<Guid>(type: "uuid", nullable: false),
                    consultar_abrangencia = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: false),
                    CriadoRF = table.Column<string>(type: "text", nullable: false),
                    Excluido = table.Column<bool>(type: "boolean", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    permite_alterar = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    permite_consultar = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    permite_excluir = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    permite_inserir = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    tipo_validacao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfil_info", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_perfil_info_codigo",
                table: "perfil_info",
                column: "codigo",
                unique: true);
        }
    }
}
