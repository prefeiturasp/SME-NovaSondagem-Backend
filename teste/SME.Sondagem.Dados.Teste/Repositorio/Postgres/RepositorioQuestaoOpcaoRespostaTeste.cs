using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio.Entidades.Questionario;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioQuestaoOpcaoRespostaTeste : RepositorioBaseTeste
    {
        private static RepositorioQuestaoOpcaoResposta CriarRepositorio(string nomeBanco)
        {
            var contexto = CriarContexto(nomeBanco);
            var servicoAuditoria = CriarServicoAuditoria();
            var contextoBase = CriarConextoBase();
            return new RepositorioQuestaoOpcaoResposta(contexto,servicoAuditoria,contextoBase);
        }

        #region ObterTodosAsync

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarSomenteNaoExcluidosOrdenados()
        {
            var nomeBanco = Guid.NewGuid().ToString();
            using var contexto = CriarContexto(nomeBanco);

            contexto.QuestoesOpcoesResposta.AddRange(
                new QuestaoOpcaoResposta(1, 1, 2),
                new QuestaoOpcaoResposta(1, 2, 1),
                new QuestaoOpcaoResposta(1, 3, 3) { Excluido = true }
            );

            await contexto.SaveChangesAsync();
            var servicoAuditoria = CriarServicoAuditoria();
            var contextoBase = CriarConextoBase();
            var repositorio = new RepositorioQuestaoOpcaoResposta(contexto,servicoAuditoria, contextoBase);

            var resultado = (await repositorio.ListarAsync()).ToList();

            Assert.Equal(2, resultado.Count);
            Assert.Equal(2, resultado[0].Ordem);
            Assert.Equal(1, resultado[1].Ordem);
        }

        #endregion

        #region ObterPorIdAsync

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarRegistroQuandoExistir()
        {
            var nomeBanco = Guid.NewGuid().ToString();
            using var contexto = CriarContexto(nomeBanco);

            var entidade = new QuestaoOpcaoResposta(1, 1, 1);
            contexto.QuestoesOpcoesResposta.Add(entidade);
            await contexto.SaveChangesAsync();
            var servicoAuditoria = CriarServicoAuditoria();
            var contextoBase = CriarConextoBase();
            var repositorio = new RepositorioQuestaoOpcaoResposta(contexto,servicoAuditoria, contextoBase);

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

            var entidade = new QuestaoOpcaoResposta(1, 1, 1);

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

            var entidade = new QuestaoOpcaoResposta(1, 1, 1);
            contexto.QuestoesOpcoesResposta.Add(entidade);
            await contexto.SaveChangesAsync();

            entidade.AlteradoEm = DateTime.UtcNow;
            entidade.AlteradoPor = "usuario";
            entidade.AlteradoRF = "123456";
            var servicoAuditoria = CriarServicoAuditoria();
            var contextoBase = CriarConextoBase();
            var repositorio = new RepositorioQuestaoOpcaoResposta(contexto,servicoAuditoria, contextoBase);

            var resultado = await repositorio.SalvarAsync(entidade);

            Assert.True(resultado>0);

            var atualizado = contexto.QuestoesOpcoesResposta.Single();
            Assert.Equal("Sistema", atualizado.AlteradoPor);
            Assert.Equal("123456", atualizado.AlteradoRF);
        }

        [Fact]
        public async Task AtualizarAsync_DeveRetornarFalse_QuandoNaoExistir()
        {
            var repositorio = CriarRepositorio(Guid.NewGuid().ToString());

            var entidade = new QuestaoOpcaoResposta(1, 1, 1);
            entidade.Id = 100;
            var resultado = await repositorio.SalvarAsync(entidade);

            Assert.False(resultado >0);
        }

        #endregion

        #region ExcluirAsync

        [Fact]
        public async Task ExcluirAsync_DeveMarcarComoExcluido()
        {
            var nomeBanco = Guid.NewGuid().ToString();
            using var contexto = CriarContexto(nomeBanco);

            var entidade = new QuestaoOpcaoResposta(1, 1, 1);
            contexto.QuestoesOpcoesResposta.Add(entidade);
            await contexto.SaveChangesAsync();
            var servicoAuditoria = CriarServicoAuditoria();
            var contextoBase = CriarConextoBase();
            var repositorio = new RepositorioQuestaoOpcaoResposta(contexto,servicoAuditoria, contextoBase);

            var resultado = await repositorio.RemoverLogico(entidade.Id);

            Assert.True(resultado>0);
            Assert.True(entidade.Excluido);
        }

        [Fact]
        public async Task ExcluirAsync_DeveRetornarFalse_QuandoNaoExistir()
        {
            var repositorio = CriarRepositorio(Guid.NewGuid().ToString());

            var resultado = await repositorio.RemoverLogico(999);

            Assert.False(resultado>0);
        }

        #endregion
    }
}
