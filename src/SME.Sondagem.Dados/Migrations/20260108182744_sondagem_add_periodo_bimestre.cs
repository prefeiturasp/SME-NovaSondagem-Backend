using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class sondagem_add_periodo_bimestre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_questionario_ciclo",
                table: "questionario");

            migrationBuilder.DropTable(
                name: "ciclo");

            migrationBuilder.DropIndex(
                name: "uk_resposta_sondagem_aluno_questao",
                table: "resposta_aluno");

            migrationBuilder.DropIndex(
                name: "IX_questionario_ciclo_id",
                table: "questionario");

            migrationBuilder.DropColumn(
                name: "ciclo_id",
                table: "questionario");

            migrationBuilder.DropColumn(
                name: "dre_id",
                table: "questionario");

            migrationBuilder.DropColumn(
                name: "dre_nome",
                table: "questionario");

            migrationBuilder.DropColumn(
                name: "modalidade_desc",
                table: "questionario");

            migrationBuilder.DropColumn(
                name: "serie_ano_nome",
                table: "questionario");

            migrationBuilder.DropColumn(
                name: "turma_id",
                table: "questionario");

            migrationBuilder.DropColumn(
                name: "turma_nome",
                table: "questionario");

            migrationBuilder.DropColumn(
                name: "ue_nome",
                table: "questionario");

            migrationBuilder.RenameColumn(
                name: "ue_id",
                table: "questionario",
                newName: "BimestreId");

            migrationBuilder.AddColumn<int>(
                name: "bimestre_id",
                table: "resposta_aluno",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "bimestre",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cod_bimestre_ensino_eol = table.Column<int>(type: "integer", nullable: false),
                    descricao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("pk_bimestre", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sondagem_periodo_bimestre",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sondagem_id = table.Column<int>(type: "integer", nullable: false),
                    bimestre_id = table.Column<int>(type: "integer", nullable: false),
                    data_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_fim = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("pk_sondagem_periodo_bimestre", x => x.id);
                    table.ForeignKey(
                        name: "fk_bimestre_sondagem_periodo_bimestre",
                        column: x => x.bimestre_id,
                        principalTable: "bimestre",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sondagem_sondagem_periodo_bimestre",
                        column: x => x.sondagem_id,
                        principalTable: "sondagem",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_resposta_aluno_bimestre_id",
                table: "resposta_aluno",
                column: "bimestre_id");

            migrationBuilder.CreateIndex(
                name: "uk_resposta_sondagem_aluno_questao",
                table: "resposta_aluno",
                columns: new[] { "sondagem_id", "aluno_id", "questao_id", "bimestre_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_questionario_BimestreId",
                table: "questionario",
                column: "BimestreId");

            migrationBuilder.CreateIndex(
                name: "uk_bimestre_cod_eol",
                table: "bimestre",
                column: "cod_bimestre_ensino_eol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_bimestre_desc",
                table: "bimestre",
                column: "descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sondagem_periodo_bimestre_bimestre_id",
                table: "sondagem_periodo_bimestre",
                column: "bimestre_id");

            migrationBuilder.CreateIndex(
                name: "IX_sondagem_periodo_bimestre_sondagem_id",
                table: "sondagem_periodo_bimestre",
                column: "sondagem_id");

            migrationBuilder.AddForeignKey(
                name: "FK_questionario_bimestre_BimestreId",
                table: "questionario",
                column: "BimestreId",
                principalTable: "bimestre",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_bimestre_resposta_aluno",
                table: "resposta_aluno",
                column: "bimestre_id",
                principalTable: "bimestre",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_questionario_bimestre_BimestreId",
                table: "questionario");

            migrationBuilder.DropForeignKey(
                name: "fk_bimestre_resposta_aluno",
                table: "resposta_aluno");

            migrationBuilder.DropTable(
                name: "sondagem_periodo_bimestre");

            migrationBuilder.DropTable(
                name: "bimestre");

            migrationBuilder.DropIndex(
                name: "IX_resposta_aluno_bimestre_id",
                table: "resposta_aluno");

            migrationBuilder.DropIndex(
                name: "uk_resposta_sondagem_aluno_questao",
                table: "resposta_aluno");

            migrationBuilder.DropIndex(
                name: "IX_questionario_BimestreId",
                table: "questionario");

            migrationBuilder.DropColumn(
                name: "bimestre_id",
                table: "resposta_aluno");

            migrationBuilder.RenameColumn(
                name: "BimestreId",
                table: "questionario",
                newName: "ue_id");

            migrationBuilder.AddColumn<int>(
                name: "ciclo_id",
                table: "questionario",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "dre_id",
                table: "questionario",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dre_nome",
                table: "questionario",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modalidade_desc",
                table: "questionario",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "serie_ano_nome",
                table: "questionario",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "turma_id",
                table: "questionario",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "turma_nome",
                table: "questionario",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ue_nome",
                table: "questionario",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ciclo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cod_ciclo_ensino_eol = table.Column<int>(type: "integer", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    desc_ciclo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ciclo", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "uk_resposta_sondagem_aluno_questao",
                table: "resposta_aluno",
                columns: new[] { "sondagem_id", "aluno_id", "questao_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_questionario_ciclo_id",
                table: "questionario",
                column: "ciclo_id");

            migrationBuilder.CreateIndex(
                name: "uk_ciclo_cod_eol",
                table: "ciclo",
                column: "cod_ciclo_ensino_eol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_ciclo_desc",
                table: "ciclo",
                column: "desc_ciclo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_questionario_ciclo",
                table: "questionario",
                column: "ciclo_id",
                principalTable: "ciclo",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
