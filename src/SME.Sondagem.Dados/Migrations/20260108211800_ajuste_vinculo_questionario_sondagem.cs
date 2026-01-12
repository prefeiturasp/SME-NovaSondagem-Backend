using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class ajuste_vinculo_questionario_sondagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sondagem_questionario",
                table: "sondagem");

            migrationBuilder.DropIndex(
                name: "IX_sondagem_questionario_id",
                table: "sondagem");

            migrationBuilder.DropColumn(
                name: "questionario_id",
                table: "sondagem");

            migrationBuilder.AddColumn<string>(
                name: "descricao",
                table: "sondagem",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "sondagem_id",
                table: "questionario",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_questionario_sondagem_id",
                table: "questionario",
                column: "sondagem_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sondagem_questionario",
                table: "questionario",
                column: "sondagem_id",
                principalTable: "sondagem",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sondagem_questionario",
                table: "questionario");

            migrationBuilder.DropIndex(
                name: "IX_questionario_sondagem_id",
                table: "questionario");

            migrationBuilder.DropColumn(
                name: "descricao",
                table: "sondagem");

            migrationBuilder.DropColumn(
                name: "sondagem_id",
                table: "questionario");

            migrationBuilder.AddColumn<int>(
                name: "questionario_id",
                table: "sondagem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_sondagem_questionario_id",
                table: "sondagem",
                column: "questionario_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sondagem_questionario",
                table: "sondagem",
                column: "questionario_id",
                principalTable: "questionario",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
