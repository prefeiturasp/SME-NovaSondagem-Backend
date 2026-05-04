using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class RemoverCamposRacaCorRepostaAluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "genero",
                table: "resposta_aluno");

            migrationBuilder.DropColumn(
                name: "raca",
                table: "resposta_aluno");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "genero",
                table: "resposta_aluno",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "raca",
                table: "resposta_aluno",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
