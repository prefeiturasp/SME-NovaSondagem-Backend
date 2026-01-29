using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dados.Teste.Services.Auditoria;
using SME.Sondagem.Dominio.Entidades.Questionario;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioOpcaoRespostaTeste : RepositorioBaseTeste
    {
        private  static RepositorioOpcaoResposta CriarRepositorio(string nomeBanco)
        {
            var servicoAuditoria = CriarServicoAuditoria();
            var contexto = CriarContexto(nomeBanco);
            var conextoBase = CriarConextoBase();
            return new RepositorioOpcaoResposta(contexto, servicoAuditoria, conextoBase);
        }

        #region ObterTodosAsync

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarSomenteNaoExcluidosOrdenadosPorDescricao()
        {
            var nomeBanco = Guid.NewGuid().ToString();
            var servicoAuditoria = CriarServicoAuditoria();
            using var contexto = CriarContexto(nomeBanco);

            contexto.OpcoesResposta.AddRange(
                new OpcaoResposta(1, "C", null, null, null),
                new OpcaoResposta(2, "A", null, null, null),
                new OpcaoResposta(3, "B", null, null, null) { Excluido = true }
            );

            await contexto.SaveChangesAsync();
            var conextoBase = CriarConextoBase();
            var repositorio = new RepositorioOpcaoResposta(contexto, servicoAuditoria, conextoBase);

            var resultado = (await repositorio.ListarAsync()).ToList();

            Assert.Equal(2, resultado.Count);
            Assert.Equal("C", resultado[0].DescricaoOpcaoResposta);
            Assert.Equal("A", resultado[1].DescricaoOpcaoResposta);
        }

        #endregion

        #region ObterPorIdAsync

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarRegistroQuandoExistir()
        {
            var nomeBanco = Guid.NewGuid().ToString();
            using var contexto = CriarContexto(nomeBanco);
            var servicoAuditoria = CriarServicoAuditoria();

            var entidade = new OpcaoResposta(1, "Opção", null, null, null);
            contexto.OpcoesResposta.Add(entidade);
            await contexto.SaveChangesAsync();
            var conextoBase = CriarConextoBase();
            var repositorio = new RepositorioOpcaoResposta(contexto, servicoAuditoria, conextoBase);

            var resultado = await repositorio.ObterPorIdAsync(entidade.Id);

            Assert.NotNull(resultado);
            Assert.Equal(entidade.Id, resultado!.Id);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarNull_QuandoNaoExistir()
        {
            var repositorio = CriarRepositorio(Guid.NewGuid().ToString());

            var resultado = await repositorio.ObterPorIdAsync(999);

            Assert.Null(resultado);
        }

        #endregion

        #region CriarAsync

        [Fact]
        public async Task CriarAsync_DevePersistirEntidadeERetornarId()
        {
            var repositorio = CriarRepositorio(Guid.NewGuid().ToString());

            var entidade = new OpcaoResposta(1, "Nova opção", null, null, null);

            var id = await repositorio.SalvarAsync(entidade);

            Assert.True(id > 0);
        }

        #endregion

        #region AtualizarAsync

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarCamposQuandoExistir()
        {
            var nomeBanco = Guid.NewGuid().ToString();
            using var contexto = CriarContexto(nomeBanco);
            var servicoAuditoria = CriarServicoAuditoria();

            var entidade = new OpcaoResposta(1, "Antiga", null, null, null);
            contexto.OpcoesResposta.Add(entidade);
            await contexto.SaveChangesAsync();

            entidade.AlteradoEm = DateTime.UtcNow;
            entidade.AlteradoPor = "Sistema";
            entidade.AlteradoRF = "123";
            entidade.Atualizar(2, "Atualizada", "legenda", "#fff", "#000");
            var conextoBase = CriarConextoBase();
            var repositorio = new RepositorioOpcaoResposta(contexto, servicoAuditoria, conextoBase);

            var resultado = await repositorio.SalvarAsync(entidade);

            Assert.True(resultado > 0);

            var atualizado = contexto.OpcoesResposta.Single();
            Assert.Equal("Atualizada", atualizado.DescricaoOpcaoResposta);
            Assert.Equal("Sistema", atualizado.AlteradoPor);
        }

        [Fact]
        public async Task AtualizarAsync_DeveRetornarFalse_QuandoNaoExistir()
        {
            var repositorio = CriarRepositorio(Guid.NewGuid().ToString());

            var entidade = new OpcaoResposta(1, "Inexistente", null, null, null);

            var resultado = await repositorio.SalvarAsync(entidade);

            Assert.NotEqual(0, resultado);
        }

        #endregion

        #region ExcluirAsync

        [Fact]
        public async Task ExcluirAsync_DeveMarcarComoExcluido()
        {
            var nomeBanco = Guid.NewGuid().ToString();
            using var contexto = CriarContexto(nomeBanco);
            var servicoAuditoria = CriarServicoAuditoria();
            var conextoBase = CriarConextoBase();
            var entidade = new OpcaoResposta(1, "Excluir", null, null, null);
            contexto.OpcoesResposta.Add(entidade);
            await contexto.SaveChangesAsync();

            var repositorio = new RepositorioOpcaoResposta(contexto, servicoAuditoria, conextoBase);

            var resultado = await repositorio.RemoverLogico(entidade.Id);

            Assert.True(resultado > 0);
            Assert.True(entidade.Excluido);
        }

        [Fact]
        public async Task ExcluirAsync_DeveRetornarFalse_QuandoNaoExistir()
        {
            var repositorio = CriarRepositorio(Guid.NewGuid().ToString());

            var resultado = await repositorio.RemoverLogico(999);

            Assert.False(resultado > 0);
        }

        #endregion

    }
}
