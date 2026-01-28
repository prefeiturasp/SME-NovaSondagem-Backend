using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class RemoverBimestreIdDeQuestionario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Remove a foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_questionario_bimestre_BimestreId",
                table: "questionario");

            // 2. Remove o índice
            migrationBuilder.DropIndex(
                name: "IX_questionario_BimestreId",
                table: "questionario");

            // 3. Remove a coluna
            migrationBuilder.DropColumn(
                name: "BimestreId",
                table: "questionario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recria a coluna caso seja necessário fazer rollback
            migrationBuilder.AddColumn<int>(
                name: "BimestreId",
                table: "questionario",
                type: "integer",
                nullable: true);

            // Recria o índice
            migrationBuilder.CreateIndex(
                name: "IX_questionario_BimestreId",
                table: "questionario",
                column: "BimestreId");

            // Recria a foreign key
            migrationBuilder.AddForeignKey(
                name: "FK_questionario_bimestre_BimestreId",
                table: "questionario",
                column: "BimestreId",
                principalTable: "bimestre",
                principalColumn: "id");
        }
    }
}