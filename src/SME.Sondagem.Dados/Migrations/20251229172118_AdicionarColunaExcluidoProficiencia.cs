using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarColunaExcluidoProficiencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Verifica se a coluna já existe antes de tentar criá-la
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'proficiencia' 
                        AND column_name = 'excluido'
                    ) THEN
                        ALTER TABLE proficiencia 
                        ADD COLUMN excluido boolean NOT NULL DEFAULT false;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                 name: "excluido",
                 table: "proficiencia");
        }
    }
}
