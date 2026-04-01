using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCamposTurmaUeDreModalidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ano_letivo",
                table: "resposta_aluno",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "dre_id",
                table: "resposta_aluno",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "modalidade_id",
                table: "resposta_aluno",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "turma_id",
                table: "resposta_aluno",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ue_id",
                table: "resposta_aluno",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ano_letivo",
                table: "resposta_aluno");

            migrationBuilder.DropColumn(
                name: "dre_id",
                table: "resposta_aluno");

            migrationBuilder.DropColumn(
                name: "modalidade_id",
                table: "resposta_aluno");

            migrationBuilder.DropColumn(
                name: "turma_id",
                table: "resposta_aluno");

            migrationBuilder.DropColumn(
                name: "ue_id",
                table: "resposta_aluno");
        }
    }
}
