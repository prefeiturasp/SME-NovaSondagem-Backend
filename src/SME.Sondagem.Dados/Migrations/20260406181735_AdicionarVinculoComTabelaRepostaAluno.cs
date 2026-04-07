using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarVinculoComTabelaRepostaAluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "genero_sexo_id",
                table: "resposta_aluno",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "programa_atendimento_id",
                table: "resposta_aluno",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "raca_cor_id",
                table: "resposta_aluno",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_resposta_aluno_genero_sexo_id",
                table: "resposta_aluno",
                column: "genero_sexo_id");

            migrationBuilder.CreateIndex(
                name: "IX_resposta_aluno_programa_atendimento_id",
                table: "resposta_aluno",
                column: "programa_atendimento_id");

            migrationBuilder.CreateIndex(
                name: "IX_resposta_aluno_raca_cor_id",
                table: "resposta_aluno",
                column: "raca_cor_id");

            migrationBuilder.AddForeignKey(
                name: "fk_genero_sexo_resposta_aluno",
                table: "resposta_aluno",
                column: "genero_sexo_id",
                principalTable: "genero_sexo",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_programa_atendimento_resposta_aluno",
                table: "resposta_aluno",
                column: "programa_atendimento_id",
                principalTable: "programa_atendimento",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_raca_cor_resposta_aluno",
                table: "resposta_aluno",
                column: "raca_cor_id",
                principalTable: "raca_cor",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_genero_sexo_resposta_aluno",
                table: "resposta_aluno");

            migrationBuilder.DropForeignKey(
                name: "fk_programa_atendimento_resposta_aluno",
                table: "resposta_aluno");

            migrationBuilder.DropForeignKey(
                name: "fk_raca_cor_resposta_aluno",
                table: "resposta_aluno");

            migrationBuilder.DropIndex(
                name: "IX_resposta_aluno_genero_sexo_id",
                table: "resposta_aluno");

            migrationBuilder.DropIndex(
                name: "IX_resposta_aluno_programa_atendimento_id",
                table: "resposta_aluno");

            migrationBuilder.DropIndex(
                name: "IX_resposta_aluno_raca_cor_id",
                table: "resposta_aluno");

            migrationBuilder.DropColumn(
                name: "genero_sexo_id",
                table: "resposta_aluno");

            migrationBuilder.DropColumn(
                name: "programa_atendimento_id",
                table: "resposta_aluno");

            migrationBuilder.DropColumn(
                name: "raca_cor_id",
                table: "resposta_aluno");
        }
    }
}
