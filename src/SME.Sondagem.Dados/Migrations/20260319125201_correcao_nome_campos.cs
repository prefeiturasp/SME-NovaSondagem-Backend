using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SME.Sondagem.Dados.Migrations
{
    /// <inheritdoc />
    public partial class correcao_nome_campos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "perfil_info",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Codigo",
                table: "perfil_info",
                newName: "codigo");

            migrationBuilder.RenameColumn(
                name: "TipoValidacao",
                table: "perfil_info",
                newName: "tipo_validacao");

            migrationBuilder.RenameColumn(
                name: "PermiteInserir",
                table: "perfil_info",
                newName: "permite_inserir");

            migrationBuilder.RenameColumn(
                name: "PermiteExcluir",
                table: "perfil_info",
                newName: "permite_excluir");

            migrationBuilder.RenameColumn(
                name: "PermiteConsultar",
                table: "perfil_info",
                newName: "permite_consultar");

            migrationBuilder.RenameColumn(
                name: "PermiteAlterar",
                table: "perfil_info",
                newName: "permite_alterar");

            migrationBuilder.RenameColumn(
                name: "ConsultarAbrangencia",
                table: "perfil_info",
                newName: "consultar_abrangencia");

            migrationBuilder.RenameColumn(
                name: "AcessoIrrestrito",
                table: "perfil_info",
                newName: "acesso_irrestrito");

            migrationBuilder.RenameIndex(
                name: "IX_perfil_info_Codigo",
                table: "perfil_info",
                newName: "IX_perfil_info_codigo");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "perfil_configuracao",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Codigo",
                table: "perfil_configuracao",
                newName: "codigo");

            migrationBuilder.RenameColumn(
                name: "TipoValidacao",
                table: "perfil_configuracao",
                newName: "tipo_validacao");

            migrationBuilder.RenameColumn(
                name: "ConsultarAbrangencia",
                table: "perfil_configuracao",
                newName: "consultar_abrangencia");

            migrationBuilder.RenameColumn(
                name: "AcessoIrrestrito",
                table: "perfil_configuracao",
                newName: "acesso_irrestrito");

            migrationBuilder.RenameColumn(
                name: "SistemaId",
                table: "controle_acesso_options",
                newName: "sistema_id");

            migrationBuilder.RenameColumn(
                name: "ModuloId",
                table: "controle_acesso_options",
                newName: "modulo_id");

            migrationBuilder.RenameColumn(
                name: "GrupoSituacao",
                table: "controle_acesso_options",
                newName: "grupo_situacao");

            migrationBuilder.RenameColumn(
                name: "CacheDuracaoMinutos",
                table: "controle_acesso_options",
                newName: "cache_duracao_minutos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "nome",
                table: "perfil_info",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "codigo",
                table: "perfil_info",
                newName: "Codigo");

            migrationBuilder.RenameColumn(
                name: "tipo_validacao",
                table: "perfil_info",
                newName: "TipoValidacao");

            migrationBuilder.RenameColumn(
                name: "permite_inserir",
                table: "perfil_info",
                newName: "PermiteInserir");

            migrationBuilder.RenameColumn(
                name: "permite_excluir",
                table: "perfil_info",
                newName: "PermiteExcluir");

            migrationBuilder.RenameColumn(
                name: "permite_consultar",
                table: "perfil_info",
                newName: "PermiteConsultar");

            migrationBuilder.RenameColumn(
                name: "permite_alterar",
                table: "perfil_info",
                newName: "PermiteAlterar");

            migrationBuilder.RenameColumn(
                name: "consultar_abrangencia",
                table: "perfil_info",
                newName: "ConsultarAbrangencia");

            migrationBuilder.RenameColumn(
                name: "acesso_irrestrito",
                table: "perfil_info",
                newName: "AcessoIrrestrito");

            migrationBuilder.RenameIndex(
                name: "IX_perfil_info_codigo",
                table: "perfil_info",
                newName: "IX_perfil_info_Codigo");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "perfil_configuracao",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "codigo",
                table: "perfil_configuracao",
                newName: "Codigo");

            migrationBuilder.RenameColumn(
                name: "tipo_validacao",
                table: "perfil_configuracao",
                newName: "TipoValidacao");

            migrationBuilder.RenameColumn(
                name: "consultar_abrangencia",
                table: "perfil_configuracao",
                newName: "ConsultarAbrangencia");

            migrationBuilder.RenameColumn(
                name: "acesso_irrestrito",
                table: "perfil_configuracao",
                newName: "AcessoIrrestrito");

            migrationBuilder.RenameColumn(
                name: "sistema_id",
                table: "controle_acesso_options",
                newName: "SistemaId");

            migrationBuilder.RenameColumn(
                name: "modulo_id",
                table: "controle_acesso_options",
                newName: "ModuloId");

            migrationBuilder.RenameColumn(
                name: "grupo_situacao",
                table: "controle_acesso_options",
                newName: "GrupoSituacao");

            migrationBuilder.RenameColumn(
                name: "cache_duracao_minutos",
                table: "controle_acesso_options",
                newName: "CacheDuracaoMinutos");
        }
    }
}
