using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "auditoria",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    acao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    chave = table.Column<long>(type: "bigint", nullable: false),
                    data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    entidade = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    rf = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    usuario = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    perfil = table.Column<Guid>(type: "uuid", nullable: true),
                    administrador = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auditoria", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "auditoria_detalhe",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    auditoria_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome_propriedade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    valor_antigo = table.Column<string>(type: "text", nullable: true),
                    valor_novo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auditoria_detalhe", x => x.id);
                    table.ForeignKey(
                        name: "fk_auditoria_detalhe_auditoria",
                        column: x => x.auditoria_id,
                        principalTable: "auditoria",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_auditoria_chave",
                table: "auditoria",
                column: "chave");

            migrationBuilder.CreateIndex(
                name: "ix_auditoria_data",
                table: "auditoria",
                column: "data");

            migrationBuilder.CreateIndex(
                name: "ix_auditoria_entidade",
                table: "auditoria",
                column: "entidade");

            migrationBuilder.CreateIndex(
                name: "ix_auditoria_entidade_chave",
                table: "auditoria",
                columns: new[] { "entidade", "chave" });

            migrationBuilder.CreateIndex(
                name: "ix_auditoria_detalhe_auditoria_id",
                table: "auditoria_detalhe",
                column: "auditoria_id");

            migrationBuilder.CreateIndex(
                name: "ix_auditoria_detalhe_nome_propriedade",
                table: "auditoria_detalhe",
                column: "nome_propriedade");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auditoria_detalhe");

            migrationBuilder.DropTable(
                name: "auditoria");
        }
    }
}
