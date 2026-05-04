using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class RemoverTabelaProgramaAtendimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_programa_atendimento_resposta_aluno",
                table: "resposta_aluno");

            migrationBuilder.DropTable(
                name: "programa_atendimento");

            migrationBuilder.DropIndex(
                name: "IX_resposta_aluno_programa_atendimento_id",
                table: "resposta_aluno");

            migrationBuilder.DropColumn(
                name: "programa_atendimento_id",
                table: "resposta_aluno");

            migrationBuilder.AddColumn<bool>(
                name: "aee",
                table: "resposta_aluno",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "deficiente",
                table: "resposta_aluno",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "pap",
                table: "resposta_aluno",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "aee",
                table: "resposta_aluno");

            migrationBuilder.DropColumn(
                name: "deficiente",
                table: "resposta_aluno");

            migrationBuilder.DropColumn(
                name: "pap",
                table: "resposta_aluno");

            migrationBuilder.AddColumn<int>(
                name: "programa_atendimento_id",
                table: "resposta_aluno",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "programa_atendimento",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    excluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_programa_atendimento", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_resposta_aluno_programa_atendimento_id",
                table: "resposta_aluno",
                column: "programa_atendimento_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramaAtendimento_Descricao",
                table: "programa_atendimento",
                column: "descricao",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_programa_atendimento_resposta_aluno",
                table: "resposta_aluno",
                column: "programa_atendimento_id",
                principalTable: "programa_atendimento",
                principalColumn: "id");
        }
    }
}
