using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioSondagemTeste : RepositorioBaseTeste
    {
        #region Helpers

        private static Dominio.Entidades.Sondagem.Sondagem CriarSondagem(
            int id,
            bool excluido = false)
        {
            var sondagem = new Dominio.Entidades.Sondagem.Sondagem(
                descricao: "Sondagem Teste",
                dataAplicacao: DateTime.UtcNow
            );

            typeof(Dominio.Entidades.Sondagem.Sondagem).GetProperty("Id")!.SetValue(sondagem, id);
            typeof(Dominio.Entidades.Sondagem.Sondagem).GetProperty("Excluido")!.SetValue(sondagem, excluido);

            return sondagem;
        }

        private static SondagemPeriodoBimestre CriarPeriodoBimestre(
            DateTime inicio,
            DateTime fim,
            bool excluido = false)
        {
            var periodo = new SondagemPeriodoBimestre(
                sondagemId: 1, 
                bimestreId: 1, 
                dataInicio: inicio,
                dataFim: fim
            );

            typeof(SondagemPeriodoBimestre).GetProperty("Excluido")!.SetValue(periodo, excluido);

            return periodo;
        }

        #endregion

        #region Construtor

        [Fact]
        public void Construtor_deve_lancar_excecao_quando_contexto_nulo()
        {
            Assert.Throws<ArgumentNullException>(() => new RepositorioSondagem(null!));
        }

        #endregion

        #region InserirAsync

        [Fact]
        public async Task InserirAsync_deve_persistir_sondagem()
        {
            var context = CriarContexto(nameof(InserirAsync_deve_persistir_sondagem));
            var repo = new RepositorioSondagem(context);

            var sondagem = CriarSondagem(1);

            await repo.InserirAsync(sondagem);

            Assert.Single(context.Sondagens);
        }

        #endregion

        #region ObterPorIdAsync

        [Fact]
        public async Task ObterPorIdAsync_deve_retornar_sondagem_quando_existir()
        {
            var context = CriarContexto(nameof(ObterPorIdAsync_deve_retornar_sondagem_quando_existir));

            var sondagem = CriarSondagem(1);
            context.Sondagens.Add(sondagem);
            await context.SaveChangesAsync();

            var repo = new RepositorioSondagem(context);

            var resultado = await repo.ObterPorIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
        }

        #endregion

        #region ObterTodosAsync

        [Fact]
        public async Task ObterTodosAsync_deve_retornar_todas_sondagens()
        {
            var context = CriarContexto(nameof(ObterTodosAsync_deve_retornar_todas_sondagens));

            context.Sondagens.AddRange(
                CriarSondagem(1),
                CriarSondagem(2)
            );
            await context.SaveChangesAsync();

            var repo = new RepositorioSondagem(context);

            var resultado = await repo.ObterTodosAsync();

            Assert.Equal(2, resultado.Count());
        }

        #endregion

        #region ObterSondagemAtiva

        [Fact]
        public async Task ObterSondagemAtiva_deve_retornar_sondagem_com_periodo_ativo()
        {
            var context = CriarContexto(nameof(ObterSondagemAtiva_deve_retornar_sondagem_com_periodo_ativo));

            var sondagem = CriarSondagem(1);

            var periodoAtivo = CriarPeriodoBimestre(
                inicio: DateTime.UtcNow.AddDays(-1),
                fim: DateTime.UtcNow.AddDays(1)
            );

            sondagem.PeriodosBimestre.Add(periodoAtivo);

            context.Sondagens.Add(sondagem);
            await context.SaveChangesAsync();

            var repo = new RepositorioSondagem(context);

            var resultado = await repo.ObterSondagemAtiva();

            Assert.NotNull(resultado);
            Assert.Single(resultado.PeriodosBimestre);
        }

        [Fact]
        public async Task ObterSondagemAtiva_nao_deve_retornar_sondagem_quando_periodo_excluido()
        {
            var context = CriarContexto(nameof(ObterSondagemAtiva_nao_deve_retornar_sondagem_quando_periodo_excluido));

            var sondagem = CriarSondagem(1);

            var periodoExcluido = CriarPeriodoBimestre(
                inicio: DateTime.UtcNow.AddDays(-1),
                fim: DateTime.UtcNow.AddDays(1),
                excluido: true
            );

            sondagem.PeriodosBimestre.Add(periodoExcluido);

            context.Sondagens.Add(sondagem);
            await context.SaveChangesAsync();

            var repo = new RepositorioSondagem(context);

            var resultado = await repo.ObterSondagemAtiva();

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObterSondagemAtiva_nao_deve_retornar_sondagem_quando_sondagem_excluida()
        {
            var context = CriarContexto(nameof(ObterSondagemAtiva_nao_deve_retornar_sondagem_quando_sondagem_excluida));

            var sondagem = CriarSondagem(1, excluido: true);

            var periodoAtivo = CriarPeriodoBimestre(
                inicio: DateTime.UtcNow.AddDays(-1),
                fim: DateTime.UtcNow.AddDays(1)
            );

            sondagem.PeriodosBimestre.Add(periodoAtivo);

            context.Sondagens.Add(sondagem);
            await context.SaveChangesAsync();

            var repo = new RepositorioSondagem(context);

            var resultado = await repo.ObterSondagemAtiva();

            Assert.Null(resultado);
        }

        #endregion
    }
}
