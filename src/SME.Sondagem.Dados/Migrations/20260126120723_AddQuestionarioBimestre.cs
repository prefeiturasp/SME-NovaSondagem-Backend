using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionarioBimestre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "questionario_bimestre",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    questionario_id = table.Column<int>(type: "integer", nullable: false),
                    bimestre_id = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("pk_questionario_bimestre", x => x.id);
                    table.ForeignKey(
                        name: "fk_questionario_bimestre_bimestre",
                        column: x => x.bimestre_id,
                        principalTable: "bimestre",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_questionario_bimestre_questionario",
                        column: x => x.questionario_id,
                        principalTable: "questionario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_questionario_bimestre_bimestre_id",
                table: "questionario_bimestre",
                column: "bimestre_id");

            migrationBuilder.CreateIndex(
                name: "ix_questionario_bimestre_questionario_id",
                table: "questionario_bimestre",
                column: "questionario_id");

            migrationBuilder.CreateIndex(
                name: "uk_questionario_bimestre_questionario_bimestre",
                table: "questionario_bimestre",
                columns: new[] { "questionario_id", "bimestre_id" },
                unique: true,
                filter: "excluido = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "questionario_bimestre");
        }
    }
}
