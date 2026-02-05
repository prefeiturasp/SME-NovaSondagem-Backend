using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableParametroSondagemQuestionario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "parametro_questionario");

            migrationBuilder.CreateTable(
                name: "parametro_sondagem_questionario",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_questionario = table.Column<int>(type: "integer", nullable: false),
                    id_parametro_sondagem = table.Column<int>(type: "integer", nullable: false),
                    valor = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_parametro_questionario", x => x.id);
                    table.ForeignKey(
                        name: "fk_parametro_questionario_parametro_sondagem_id",
                        column: x => x.id_parametro_sondagem,
                        principalTable: "parametro_sondagem",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_parametro_questionario_questionario_id",
                        column: x => x.id_questionario,
                        principalTable: "questionario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_parametro_questionario_parametro_sondagem_id",
                table: "parametro_sondagem_questionario",
                column: "id_parametro_sondagem");

            migrationBuilder.CreateIndex(
                name: "ix_parametro_questionario_questionario_id",
                table: "parametro_sondagem_questionario",
                column: "id_questionario");

            migrationBuilder.CreateIndex(
                name: "uk_parametro_questionario_questionario_parametro_sondagem",
                table: "parametro_sondagem_questionario",
                columns: new[] { "id_questionario", "id_parametro_sondagem" },
                unique: true,
                filter: "excluido = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "parametro_sondagem_questionario");

            migrationBuilder.CreateTable(
                name: "parametro_questionario",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_parametro_sondagem = table.Column<int>(type: "integer", nullable: false),
                    id_questionario = table.Column<int>(type: "integer", nullable: false),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    valor = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_parametro_questionario", x => x.id);
                    table.ForeignKey(
                        name: "fk_parametro_questionario_parametro_sondagem_id",
                        column: x => x.id_parametro_sondagem,
                        principalTable: "parametro_sondagem",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_parametro_questionario_questionario_id",
                        column: x => x.id_questionario,
                        principalTable: "questionario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_parametro_questionario_parametro_sondagem_id",
                table: "parametro_questionario",
                column: "id_parametro_sondagem");

            migrationBuilder.CreateIndex(
                name: "ix_parametro_questionario_questionario_id",
                table: "parametro_questionario",
                column: "id_questionario");

            migrationBuilder.CreateIndex(
                name: "uk_parametro_questionario_questionario_parametro_sondagem",
                table: "parametro_questionario",
                columns: new[] { "id_questionario", "id_parametro_sondagem" },
                unique: true,
                filter: "excluido = false");
        }
    }
}
