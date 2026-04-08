using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AlterarTipoDeDadoDosCamposAnoTurmaEModalidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE resposta_aluno ALTER COLUMN modalidade_id TYPE integer USING modalidade_id::integer;");

            migrationBuilder.Sql("ALTER TABLE resposta_aluno ALTER COLUMN ano_turma TYPE integer USING ano_turma::integer;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "modalidade_id",
                table: "resposta_aluno",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ano_turma",
                table: "resposta_aluno",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 10,
                oldNullable: true);
        }
    }
}
