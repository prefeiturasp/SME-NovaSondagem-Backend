using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class ajuste_uk_resposta_sondagem_aluno_questao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uk_resposta_sondagem_aluno_questao",
                table: "resposta_aluno");

            migrationBuilder.CreateIndex(
                name: "uk_resposta_sondagem_aluno_questao",
                table: "resposta_aluno",
                columns: new[] { "sondagem_id", "aluno_id", "questao_id", "bimestre_id" },
                unique: true,
                filter: "excluido = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uk_resposta_sondagem_aluno_questao",
                table: "resposta_aluno");

            migrationBuilder.CreateIndex(
                name: "uk_resposta_sondagem_aluno_questao",
                table: "resposta_aluno",
                columns: new[] { "sondagem_id", "aluno_id", "questao_id", "bimestre_id" },
                unique: true);
        }
    }
}
