using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AjusteRelacionamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Esta migration foi criada após ajustes no modelo (remoção da navegação Questionarios em Bimestre)
            // Porém, as alterações de banco já foram aplicadas pela migration RemoverBimestreIdDeQuestionario
            // Mantemos esta migration vazia apenas para manter o histórico correto do EF Core

            // Nenhuma ação necessária - as alterações já foram aplicadas
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Não faz nada no Down também, pois o Down está na migration anterior
            // Caso seja necessário reverter, use: dotnet ef database update RemoverBimestreIdDeQuestionario
        }
    }
}
