using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AjusteNomeCamposAudi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CriadoRF",
                table: "raca_cor",
                newName: "criado_rf");

            migrationBuilder.RenameColumn(
                name: "CriadoPor",
                table: "raca_cor",
                newName: "criado_por");

            migrationBuilder.RenameColumn(
                name: "CriadoEm",
                table: "raca_cor",
                newName: "criado_em");

            migrationBuilder.RenameColumn(
                name: "AlteradoRF",
                table: "raca_cor",
                newName: "alterado_rf");

            migrationBuilder.RenameColumn(
                name: "AlteradoPor",
                table: "raca_cor",
                newName: "alterado_por");

            migrationBuilder.RenameColumn(
                name: "AlteradoEm",
                table: "raca_cor",
                newName: "alterado_em");

            migrationBuilder.RenameColumn(
                name: "CriadoRF",
                table: "programa_atendimento",
                newName: "criado_rf");

            migrationBuilder.RenameColumn(
                name: "CriadoPor",
                table: "programa_atendimento",
                newName: "criado_por");

            migrationBuilder.RenameColumn(
                name: "CriadoEm",
                table: "programa_atendimento",
                newName: "criado_em");

            migrationBuilder.RenameColumn(
                name: "AlteradoRF",
                table: "programa_atendimento",
                newName: "alterado_rf");

            migrationBuilder.RenameColumn(
                name: "AlteradoPor",
                table: "programa_atendimento",
                newName: "alterado_por");

            migrationBuilder.RenameColumn(
                name: "AlteradoEm",
                table: "programa_atendimento",
                newName: "alterado_em");

            migrationBuilder.RenameColumn(
                name: "CriadoRF",
                table: "genero_sexo",
                newName: "criado_rf");

            migrationBuilder.RenameColumn(
                name: "CriadoPor",
                table: "genero_sexo",
                newName: "criado_por");

            migrationBuilder.RenameColumn(
                name: "CriadoEm",
                table: "genero_sexo",
                newName: "criado_em");

            migrationBuilder.RenameColumn(
                name: "AlteradoRF",
                table: "genero_sexo",
                newName: "alterado_rf");

            migrationBuilder.RenameColumn(
                name: "AlteradoPor",
                table: "genero_sexo",
                newName: "alterado_por");

            migrationBuilder.RenameColumn(
                name: "AlteradoEm",
                table: "genero_sexo",
                newName: "alterado_em");

            migrationBuilder.AlterColumn<string>(
                name: "criado_rf",
                table: "raca_cor",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "criado_por",
                table: "raca_cor",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "alterado_rf",
                table: "raca_cor",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "alterado_por",
                table: "raca_cor",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "criado_rf",
                table: "programa_atendimento",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "criado_por",
                table: "programa_atendimento",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "alterado_rf",
                table: "programa_atendimento",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "alterado_por",
                table: "programa_atendimento",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "criado_rf",
                table: "genero_sexo",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "criado_por",
                table: "genero_sexo",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "alterado_rf",
                table: "genero_sexo",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "alterado_por",
                table: "genero_sexo",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "criado_rf",
                table: "raca_cor",
                newName: "CriadoRF");

            migrationBuilder.RenameColumn(
                name: "criado_por",
                table: "raca_cor",
                newName: "CriadoPor");

            migrationBuilder.RenameColumn(
                name: "criado_em",
                table: "raca_cor",
                newName: "CriadoEm");

            migrationBuilder.RenameColumn(
                name: "alterado_rf",
                table: "raca_cor",
                newName: "AlteradoRF");

            migrationBuilder.RenameColumn(
                name: "alterado_por",
                table: "raca_cor",
                newName: "AlteradoPor");

            migrationBuilder.RenameColumn(
                name: "alterado_em",
                table: "raca_cor",
                newName: "AlteradoEm");

            migrationBuilder.RenameColumn(
                name: "criado_rf",
                table: "programa_atendimento",
                newName: "CriadoRF");

            migrationBuilder.RenameColumn(
                name: "criado_por",
                table: "programa_atendimento",
                newName: "CriadoPor");

            migrationBuilder.RenameColumn(
                name: "criado_em",
                table: "programa_atendimento",
                newName: "CriadoEm");

            migrationBuilder.RenameColumn(
                name: "alterado_rf",
                table: "programa_atendimento",
                newName: "AlteradoRF");

            migrationBuilder.RenameColumn(
                name: "alterado_por",
                table: "programa_atendimento",
                newName: "AlteradoPor");

            migrationBuilder.RenameColumn(
                name: "alterado_em",
                table: "programa_atendimento",
                newName: "AlteradoEm");

            migrationBuilder.RenameColumn(
                name: "criado_rf",
                table: "genero_sexo",
                newName: "CriadoRF");

            migrationBuilder.RenameColumn(
                name: "criado_por",
                table: "genero_sexo",
                newName: "CriadoPor");

            migrationBuilder.RenameColumn(
                name: "criado_em",
                table: "genero_sexo",
                newName: "CriadoEm");

            migrationBuilder.RenameColumn(
                name: "alterado_rf",
                table: "genero_sexo",
                newName: "AlteradoRF");

            migrationBuilder.RenameColumn(
                name: "alterado_por",
                table: "genero_sexo",
                newName: "AlteradoPor");

            migrationBuilder.RenameColumn(
                name: "alterado_em",
                table: "genero_sexo",
                newName: "AlteradoEm");

            migrationBuilder.AlterColumn<string>(
                name: "CriadoRF",
                table: "raca_cor",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CriadoPor",
                table: "raca_cor",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "AlteradoRF",
                table: "raca_cor",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AlteradoPor",
                table: "raca_cor",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CriadoRF",
                table: "programa_atendimento",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CriadoPor",
                table: "programa_atendimento",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "AlteradoRF",
                table: "programa_atendimento",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AlteradoPor",
                table: "programa_atendimento",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CriadoRF",
                table: "genero_sexo",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CriadoPor",
                table: "genero_sexo",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "AlteradoRF",
                table: "genero_sexo",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AlteradoPor",
                table: "genero_sexo",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
