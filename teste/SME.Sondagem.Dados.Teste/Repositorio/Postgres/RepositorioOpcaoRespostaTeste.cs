using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio.Entidades.Questionario;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioOpcaoRespostaTeste : RepositorioBaseTeste
    {
        private static RepositorioOpcaoResposta CriarRepositorio(string nomeBanco)
        {
            var contexto = CriarContexto(nomeBanco);
            return new RepositorioOpcaoResposta(contexto);
        }

        #region ObterTodosAsync

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarSomenteNaoExcluidosOrdenadosPorDescricao()
        {
            var nomeBanco = Guid.NewGuid().ToString();
            using var contexto = CriarContexto(nomeBanco);

            contexto.OpcoesResposta.AddRange(
                new OpcaoResposta(1, "C", null, null, null),
                new OpcaoResposta(2, "A", null, null, null),
                new OpcaoResposta(3, "B", null, null, null) { Excluido = true }
            );

            await contexto.SaveChangesAsync();

            var repositorio = new RepositorioOpcaoResposta(contexto);

            var resultado = (await repositorio.ObterTodosAsync()).ToList();

            Assert.Equal(2, resultado.Count);
            Assert.Equal("A", resultado[0].DescricaoOpcaoResposta);
            Assert.Equal("C", resultado[1].DescricaoOpcaoResposta);
        }

        #endregion

        #region ObterPorIdAsync

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarRegistroQuandoExistir()
        {
            var nomeBanco = Guid.NewGuid().ToString();
            using var contexto = CriarContexto(nomeBanco);

            var entidade = new OpcaoResposta(1, "Opção", null, null, null);
            contexto.OpcoesResposta.Add(entidade);
            await contexto.SaveChangesAsync();

            var repositorio = new RepositorioOpcaoResposta(contexto);

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

            var id = await repositorio.CriarAsync(entidade);

            Assert.True(id > 0);
        }

        #endregion

        #region AtualizarAsync

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarCamposQuandoExistir()
        {
            var nomeBanco = Guid.NewGuid().ToString();
            using var contexto = CriarContexto(nomeBanco);

            var entidade = new OpcaoResposta(1, "Antiga", null, null, null);
            contexto.OpcoesResposta.Add(entidade);
            await contexto.SaveChangesAsync();

            entidade.AlteradoEm = DateTime.UtcNow;
            entidade.AlteradoPor = "usuario";
            entidade.AlteradoRF = "123";
            entidade.Atualizar(2, "Atualizada", "legenda", "#fff", "#000");

            var repositorio = new RepositorioOpcaoResposta(contexto);

            var resultado = await repositorio.AtualizarAsync(entidade);

            Assert.True(resultado);

            var atualizado = contexto.OpcoesResposta.Single();
            Assert.Equal("Atualizada", atualizado.DescricaoOpcaoResposta);
            Assert.Equal("usuario", atualizado.AlteradoPor);
        }

        [Fact]
        public async Task AtualizarAsync_DeveRetornarFalse_QuandoNaoExistir()
        {
            var repositorio = CriarRepositorio(Guid.NewGuid().ToString());

            var entidade = new OpcaoResposta(1, "Inexistente", null, null, null);

            var resultado = await repositorio.AtualizarAsync(entidade);

            Assert.False(resultado);
        }

        #endregion

        #region ExcluirAsync

        [Fact]
        public async Task ExcluirAsync_DeveMarcarComoExcluido()
        {
            var nomeBanco = Guid.NewGuid().ToString();
            using var contexto = CriarContexto(nomeBanco);

            var entidade = new OpcaoResposta(1, "Excluir", null, null, null);
            contexto.OpcoesResposta.Add(entidade);
            await contexto.SaveChangesAsync();

            var repositorio = new RepositorioOpcaoResposta(contexto);

            var resultado = await repositorio.ExcluirAsync(entidade.Id);

            Assert.True(resultado);
            Assert.True(entidade.Excluido);
        }

        [Fact]
        public async Task ExcluirAsync_DeveRetornarFalse_QuandoNaoExistir()
        {
            var repositorio = CriarRepositorio(Guid.NewGuid().ToString());

            var resultado = await repositorio.ExcluirAsync(999);

            Assert.False(resultado);
        }

        #endregion
    }
}
