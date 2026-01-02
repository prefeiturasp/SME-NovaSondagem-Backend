using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AddCorFundoECorTextoInOpcaoResposta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cor_fundo",
                table: "opcao_resposta",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cor_texto",
                table: "opcao_resposta",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cor_fundo",
                table: "opcao_resposta");

            migrationBuilder.DropColumn(
                name: "cor_texto",
                table: "opcao_resposta");
        }
    }
}
