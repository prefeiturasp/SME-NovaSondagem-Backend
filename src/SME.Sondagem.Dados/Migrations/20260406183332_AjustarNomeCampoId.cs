using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AjustarNomeCampoId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "raca_cor",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "programa_atendimento",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "genero_sexo",
                newName: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "raca_cor",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "programa_atendimento",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "genero_sexo",
                newName: "Id");
        }
    }
}
