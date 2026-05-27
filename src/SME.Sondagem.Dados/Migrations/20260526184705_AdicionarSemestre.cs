using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarSemestre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "semestre_id",
                table: "resposta_aluno",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "semestre",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cod_semestre_eol = table.Column<int>(type: "integer", nullable: false),
                    descricao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("pk_semestre", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_resposta_aluno_semestre_id",
                table: "resposta_aluno",
                column: "semestre_id");

            migrationBuilder.AddForeignKey(
                name: "fk_semestre_resposta_aluno",
                table: "resposta_aluno",
                column: "semestre_id",
                principalTable: "semestre",
                principalColumn: "id");

            migrationBuilder.Sql(@"
                INSERT INTO semestre (id, cod_semestre_eol, descricao, criado_em, criado_por, criado_rf, excluido)
                VALUES
                    (1, 1, '1° semestre', '2025-01-09 11:00:00+00', 'SISTEMA', 'SISTEMA', FALSE),
                    (2, 2, '2° semestre', '2025-01-09 11:00:00+00', 'SISTEMA', 'SISTEMA', FALSE);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_semestre_resposta_aluno",
                table: "resposta_aluno");

            migrationBuilder.DropTable(
                name: "semestre");

            migrationBuilder.DropIndex(
                name: "IX_resposta_aluno_semestre_id",
                table: "resposta_aluno");

            migrationBuilder.DropColumn(
                name: "semestre_id",
                table: "resposta_aluno");
        }
    }
}
