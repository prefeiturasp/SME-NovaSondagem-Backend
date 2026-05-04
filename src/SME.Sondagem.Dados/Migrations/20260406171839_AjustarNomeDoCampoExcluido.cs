using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AjustarNomeDoCampoExcluido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Excluido",
                table: "raca_cor",
                newName: "excluido");

            migrationBuilder.RenameColumn(
                name: "Excluido",
                table: "programa_atendimento",
                newName: "excluido");

            migrationBuilder.RenameColumn(
                name: "Excluido",
                table: "genero_sexo",
                newName: "excluido");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "excluido",
                table: "raca_cor",
                newName: "Excluido");

            migrationBuilder.RenameColumn(
                name: "excluido",
                table: "programa_atendimento",
                newName: "Excluido");

            migrationBuilder.RenameColumn(
                name: "excluido",
                table: "genero_sexo",
                newName: "Excluido");
        }
    }
}
