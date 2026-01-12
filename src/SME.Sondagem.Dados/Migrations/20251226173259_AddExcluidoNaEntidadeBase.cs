using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AddExcluidoNaEntidadeBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "excluido",
                table: "sondagem",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "excluido",
                table: "resposta_aluno",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "excluido",
                table: "questao_opcao_resposta",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "excluido",
                table: "questao",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "excluido",
                table: "proficiencia",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "excluido",
                table: "opcao_resposta",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "excluido",
                table: "grupo_questoes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "excluido",
                table: "componente_curricular",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "excluido",
                table: "ciclo",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "excluido",
                table: "aluno",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "excluido",
                table: "sondagem");

            migrationBuilder.DropColumn(
                name: "excluido",
                table: "resposta_aluno");

            migrationBuilder.DropColumn(
                name: "excluido",
                table: "questao_opcao_resposta");

            migrationBuilder.DropColumn(
                name: "excluido",
                table: "questao");

            migrationBuilder.DropColumn(
                name: "excluido",
                table: "proficiencia");

            migrationBuilder.DropColumn(
                name: "excluido",
                table: "opcao_resposta");

            migrationBuilder.DropColumn(
                name: "excluido",
                table: "grupo_questoes");

            migrationBuilder.DropColumn(
                name: "excluido",
                table: "componente_curricular");

            migrationBuilder.DropColumn(
                name: "excluido",
                table: "ciclo");

            migrationBuilder.DropColumn(
                name: "excluido",
                table: "aluno");
        }
    }
}
