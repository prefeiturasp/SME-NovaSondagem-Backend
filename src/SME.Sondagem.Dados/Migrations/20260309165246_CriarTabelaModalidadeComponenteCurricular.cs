using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelaModalidadeComponenteCurricular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "modalidade_componente_curricular",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    modalidade_id = table.Column<int>(type: "integer", nullable: false),
                    componente_curricular_id = table.Column<int>(type: "integer", nullable: false),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_modalidade_componente_curricular", x => x.id);
                    table.ForeignKey(
                        name: "fk_modalidade_componente_curricular_componente",
                        column: x => x.componente_curricular_id,
                        principalTable: "componente_curricular",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_modalidade_componente_curricular_componente_curricular_id",
                table: "modalidade_componente_curricular",
                column: "componente_curricular_id");

            migrationBuilder.CreateIndex(
                name: "uk_modalidade_componente",
                table: "modalidade_componente_curricular",
                columns: new[] { "modalidade_id", "componente_curricular_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "modalidade_componente_curricular");
        }
    }
}
