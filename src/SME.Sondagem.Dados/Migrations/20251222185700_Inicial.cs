using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "aluno",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ra_aluno = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    nome_aluno = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    is_pap = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_aee = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_pcd = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deficiencia_id = table.Column<int>(type: "integer", nullable: true),
                    deficiencia_nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    raca_id = table.Column<int>(type: "integer", nullable: true),
                    raca_nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    cor_id = table.Column<int>(type: "integer", nullable: true),
                    cor_nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aluno", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ciclo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cod_ciclo_ensino_eol = table.Column<int>(type: "integer", nullable: false),
                    desc_ciclo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ciclo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "componente_curricular",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ano = table.Column<int>(type: "integer", nullable: true),
                    modalidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_componente_curricular", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "grupo_questoes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    subtitulo = table.Column<string>(type: "text", nullable: true),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_grupo_questoes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "opcao_resposta",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao_opcao_resposta = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    legenda = table.Column<string>(type: "text", nullable: true),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_opcao_resposta", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "proficiencia",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    componente_curricular_id = table.Column<int>(type: "integer", nullable: false),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_proficiencia", x => x.id);
                    table.ForeignKey(
                        name: "fk_proficiencia_componente",
                        column: x => x.componente_curricular_id,
                        principalTable: "componente_curricular",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "questionario",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ano_letivo = table.Column<int>(type: "integer", nullable: false),
                    modalidade_id = table.Column<int>(type: "integer", nullable: true),
                    modalidade_desc = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    dre_id = table.Column<int>(type: "integer", nullable: true),
                    dre_nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ue_id = table.Column<int>(type: "integer", nullable: true),
                    ue_nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    serie_ano = table.Column<int>(type: "integer", nullable: true),
                    serie_ano_nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    turma_id = table.Column<int>(type: "integer", nullable: true),
                    turma_nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    componente_curricular_id = table.Column<int>(type: "integer", nullable: false),
                    proficiencia_id = table.Column<int>(type: "integer", nullable: false),
                    ciclo_id = table.Column<int>(type: "integer", nullable: false),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questionario", x => x.id);
                    table.ForeignKey(
                        name: "fk_questionario_ciclo",
                        column: x => x.ciclo_id,
                        principalTable: "ciclo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_questionario_componente",
                        column: x => x.componente_curricular_id,
                        principalTable: "componente_curricular",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_questionario_proficiencia",
                        column: x => x.proficiencia_id,
                        principalTable: "proficiencia",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "questao",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    questionario_id = table.Column<int>(type: "integer", nullable: false),
                    grupo_questoes_id = table.Column<int>(type: "integer", nullable: true),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    nome = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    observacao = table.Column<string>(type: "text", nullable: false),
                    obrigatorio = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    opcionais = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    somente_leitura = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    dimensao = table.Column<int>(type: "integer", nullable: false),
                    tamanho = table.Column<int>(type: "integer", nullable: true),
                    mascara = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    place_holder = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    nome_componente = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questao", x => x.id);
                    table.ForeignKey(
                        name: "fk_questao_grupo",
                        column: x => x.grupo_questoes_id,
                        principalTable: "grupo_questoes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_questao_questionario",
                        column: x => x.questionario_id,
                        principalTable: "questionario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sondagem",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    questionario_id = table.Column<int>(type: "integer", nullable: false),
                    data_aplicacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sondagem", x => x.id);
                    table.ForeignKey(
                        name: "fk_sondagem_questionario",
                        column: x => x.questionario_id,
                        principalTable: "questionario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "questao_opcao_resposta",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    questao_id = table.Column<int>(type: "integer", nullable: false),
                    opcao_resposta_id = table.Column<int>(type: "integer", nullable: false),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questao_opcao_resposta", x => x.id);
                    table.ForeignKey(
                        name: "fk_questao_opcao_opcao",
                        column: x => x.opcao_resposta_id,
                        principalTable: "opcao_resposta",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_questao_opcao_questao",
                        column: x => x.questao_id,
                        principalTable: "questao",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resposta_aluno",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sondagem_id = table.Column<int>(type: "integer", nullable: false),
                    aluno_id = table.Column<int>(type: "integer", nullable: false),
                    questao_id = table.Column<int>(type: "integer", nullable: false),
                    opcao_resposta_id = table.Column<int>(type: "integer", nullable: false),
                    data_resposta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    alterado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    alterado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    alterado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    criado_rf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_resposta_aluno", x => x.id);
                    table.ForeignKey(
                        name: "fk_resposta_aluno",
                        column: x => x.aluno_id,
                        principalTable: "aluno",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_resposta_opcao",
                        column: x => x.opcao_resposta_id,
                        principalTable: "opcao_resposta",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_resposta_questao",
                        column: x => x.questao_id,
                        principalTable: "questao",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_resposta_sondagem",
                        column: x => x.sondagem_id,
                        principalTable: "sondagem",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "uk_aluno_ra",
                table: "aluno",
                column: "ra_aluno",
                unique: true,
                filter: "ra_aluno IS NOT NULL");

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

            migrationBuilder.CreateIndex(
                name: "uk_componente_nome_ano_modalidade",
                table: "componente_curricular",
                columns: new[] { "nome", "ano", "modalidade" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_opcao_resposta_desc",
                table: "opcao_resposta",
                column: "descricao_opcao_resposta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_proficiencia_componente_curricular_id",
                table: "proficiencia",
                column: "componente_curricular_id");

            migrationBuilder.CreateIndex(
                name: "uk_proficiencia_nome_componente",
                table: "proficiencia",
                columns: new[] { "nome", "componente_curricular_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_questao_grupo_questoes_id",
                table: "questao",
                column: "grupo_questoes_id");

            migrationBuilder.CreateIndex(
                name: "IX_questao_questionario_id",
                table: "questao",
                column: "questionario_id");

            migrationBuilder.CreateIndex(
                name: "IX_questao_opcao_resposta_opcao_resposta_id",
                table: "questao_opcao_resposta",
                column: "opcao_resposta_id");

            migrationBuilder.CreateIndex(
                name: "uk_questao_opcao",
                table: "questao_opcao_resposta",
                columns: new[] { "questao_id", "opcao_resposta_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_questionario_ciclo_id",
                table: "questionario",
                column: "ciclo_id");

            migrationBuilder.CreateIndex(
                name: "IX_questionario_componente_curricular_id",
                table: "questionario",
                column: "componente_curricular_id");

            migrationBuilder.CreateIndex(
                name: "IX_questionario_proficiencia_id",
                table: "questionario",
                column: "proficiencia_id");

            migrationBuilder.CreateIndex(
                name: "IX_resposta_aluno_aluno_id",
                table: "resposta_aluno",
                column: "aluno_id");

            migrationBuilder.CreateIndex(
                name: "IX_resposta_aluno_opcao_resposta_id",
                table: "resposta_aluno",
                column: "opcao_resposta_id");

            migrationBuilder.CreateIndex(
                name: "IX_resposta_aluno_questao_id",
                table: "resposta_aluno",
                column: "questao_id");

            migrationBuilder.CreateIndex(
                name: "uk_resposta_sondagem_aluno_questao",
                table: "resposta_aluno",
                columns: new[] { "sondagem_id", "aluno_id", "questao_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sondagem_questionario_id",
                table: "sondagem",
                column: "questionario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "questao_opcao_resposta");

            migrationBuilder.DropTable(
                name: "resposta_aluno");

            migrationBuilder.DropTable(
                name: "aluno");

            migrationBuilder.DropTable(
                name: "opcao_resposta");

            migrationBuilder.DropTable(
                name: "questao");

            migrationBuilder.DropTable(
                name: "sondagem");

            migrationBuilder.DropTable(
                name: "grupo_questoes");

            migrationBuilder.DropTable(
                name: "questionario");

            migrationBuilder.DropTable(
                name: "ciclo");

            migrationBuilder.DropTable(
                name: "proficiencia");

            migrationBuilder.DropTable(
                name: "componente_curricular");
        }
    }
}
