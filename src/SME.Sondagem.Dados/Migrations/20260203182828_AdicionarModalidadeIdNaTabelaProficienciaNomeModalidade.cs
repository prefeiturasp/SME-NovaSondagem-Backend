using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarModalidadeIdNaTabelaProficienciaNomeModalidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uk_proficiencia_nome_componente",
                table: "proficiencia");

            migrationBuilder.CreateIndex(
                name: "uk_proficiencia_nome_componente",
                table: "proficiencia",
                columns: new[] { "nome", "componente_curricular_id", "modalidade_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uk_proficiencia_nome_componente",
                table: "proficiencia");

            migrationBuilder.CreateIndex(
                name: "uk_proficiencia_nome_componente",
                table: "proficiencia",
                columns: new[] { "nome", "componente_curricular_id" },
                unique: true);
        }
    }
}
