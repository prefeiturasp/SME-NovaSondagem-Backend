using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class adicionar_questao_vinculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "questao_vinculo_id",
                table: "questao",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_questao_questao_vinculo_id",
                table: "questao",
                column: "questao_vinculo_id");

            migrationBuilder.AddForeignKey(
                name: "fk_questao_vinculo",
                table: "questao",
                column: "questao_vinculo_id",
                principalTable: "questao",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_questao_vinculo",
                table: "questao");

            migrationBuilder.DropIndex(
                name: "IX_questao_questao_vinculo_id",
                table: "questao");

            migrationBuilder.DropColumn(
                name: "questao_vinculo_id",
                table: "questao");
        }
    }
}
