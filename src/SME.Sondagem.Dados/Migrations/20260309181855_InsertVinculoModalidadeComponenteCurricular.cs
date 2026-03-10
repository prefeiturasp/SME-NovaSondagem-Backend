using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class InsertVinculoModalidadeComponenteCurricular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var criadoEm = new DateTime(2026, 3, 9, 0, 0, 0, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                table: "modalidade_componente_curricular",
                columns: ["modalidade_id", "componente_curricular_id", "excluido", "criado_em", "criado_por", "criado_rf"],
                values: new object[,]
                {
                 { 5, 1, false, criadoEm, "Sistema", "0" }, // EF PORTUGUÊS 
                 { 5, 2, false, criadoEm, "Sistema", "0" }, // EF MATEMATICA
                 { 3, 1, false, criadoEm, "Sistema", "0" }  // EJA PORTUGUÊS
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "modalidade_componente_curricular",
                keyColumns: ["modalidade_id", "componente_curricular_id"],
                keyValues: new object[] { 5, 1 });

            migrationBuilder.DeleteData(
                table: "modalidade_componente_curricular",
                keyColumns: ["modalidade_id", "componente_curricular_id"],
                keyValues: new object[] { 5, 2 });

            migrationBuilder.DeleteData(
                table: "modalidade_componente_curricular",
                keyColumns: ["modalidade_id", "componente_curricular_id"],
                keyValues: new object[] { 3, 1 });
        }
    }
}
