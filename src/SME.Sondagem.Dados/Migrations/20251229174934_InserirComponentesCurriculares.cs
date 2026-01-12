using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class InserirComponentesCurriculares : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var dataAtual = new DateTime(2025, 12, 29, 0, 0, 0, DateTimeKind.Utc);

            // Verifica se os componentes curriculares já existem antes de inserir
            migrationBuilder.Sql($@"
                DO $$ 
                BEGIN 
                    -- Inserir Português se não existir
                    IF NOT EXISTS (SELECT 1 FROM componente_curricular WHERE id = 1) THEN
                        INSERT INTO componente_curricular (id, nome, ano, modalidade, codigo_eol, criado_em, criado_por, criado_rf, excluido)
                        VALUES (1, 'Português', NULL, NULL, 1, '{dataAtual:yyyy-MM-dd HH:mm:ss}', 'Sistema', 'ADMIN', false);
                    END IF;

                    -- Inserir Matemática se não existir
                    IF NOT EXISTS (SELECT 1 FROM componente_curricular WHERE id = 2) THEN
                        INSERT INTO componente_curricular (id, nome, ano, modalidade, codigo_eol, criado_em, criado_por, criado_rf, excluido)
                        VALUES (2, 'Matemática', NULL, NULL, 2, '{dataAtual:yyyy-MM-dd HH:mm:ss}', 'Sistema', 'ADMIN', false);
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "componente_curricular",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "componente_curricular",
                keyColumn: "id",
                keyValue: 2);
        }
    }
}
