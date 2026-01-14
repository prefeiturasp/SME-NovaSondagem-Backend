using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class RemoverVinculoTabelaAluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_resposta_opcao",
                table: "resposta_aluno");

            migrationBuilder.AlterColumn<int>(
                name: "opcao_resposta_id",
                table: "resposta_aluno",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "fk_resposta_opcao",
                table: "resposta_aluno",
                column: "opcao_resposta_id",
                principalTable: "opcao_resposta",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_resposta_opcao",
                table: "resposta_aluno");

            migrationBuilder.AlterColumn<int>(
                name: "opcao_resposta_id",
                table: "resposta_aluno",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_resposta_opcao",
                table: "resposta_aluno",
                column: "opcao_resposta_id",
                principalTable: "opcao_resposta",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
