using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioQuestaoTeste : RepositorioBaseTeste
    {
        private static Questao CriarQuestao(
    int id = 1,
    bool excluido = false,
    TipoQuestao tipo = TipoQuestao.Combo,
    int ordem = 1)
        {
            var questao = new Questao(
                questionarioId: 1,
                ordem: ordem,
                nome: "Questão",
                observacao: "Obs",
                obrigatorio: true,
                tipo: tipo,
                opcionais: "{}",
                somenteLeitura: false,
                dimensao: 12
            );

            typeof(Questao).GetProperty("Id")!.SetValue(questao, id);
            typeof(Questao).GetProperty("Excluido")!.SetValue(questao, excluido);

            return questao;
        }

        #region Construtor

        [Fact]
        public void Deve_lancar_excecao_quando_contexto_for_nulo()
        {
            Assert.Throws<ArgumentNullException>(() => new RepositorioQuestao(null!));
        }

        #endregion

        #region ObterTodosAsync

        [Fact]
        public async Task ObterTodosAsync_deve_retornar_apenas_questoes_nao_excluidas()
        {
            var context = CriarContexto(nameof(ObterTodosAsync_deve_retornar_apenas_questoes_nao_excluidas));

            context.Questoes.AddRange(
                CriarQuestao(1, false),
                CriarQuestao(2, true)
            );

            await context.SaveChangesAsync();

            var repo = new RepositorioQuestao(context);

            var resultado = await repo.ObterTodosAsync();

            Assert.Single(resultado);
        }

        #endregion

        #region ObterPorIdAsync

        [Fact]
        public async Task ObterPorIdAsync_deve_retornar_questao_quando_existir()
        {
            var context = CriarContexto(nameof(ObterPorIdAsync_deve_retornar_questao_quando_existir));

            var questao = CriarQuestao(1);
            context.Questoes.Add(questao);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestao(context);

            var resultado = await repo.ObterPorIdAsync(1);

            Assert.NotNull(resultado);
        }

        [Fact]
        public async Task ObterPorIdAsync_deve_retornar_null_quando_excluida()
        {
            var context = CriarContexto(nameof(ObterPorIdAsync_deve_retornar_null_quando_excluida));

            context.Questoes.Add(CriarQuestao(1, true));
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestao(context);

            var resultado = await repo.ObterPorIdAsync(1);

            Assert.Null(resultado);
        }

        #endregion

        #region CriarAsync

        [Fact]
        public async Task CriarAsync_deve_persistir_e_retornar_id()
        {
            var context = CriarContexto(nameof(CriarAsync_deve_persistir_e_retornar_id));
            var repo = new RepositorioQuestao(context);

            var questao = CriarQuestao();

            var id = await repo.CriarAsync(questao);

            Assert.True(id > 0);
            Assert.Single(context.Questoes);
        }

        #endregion

        #region AtualizarAsync

        [Fact]
        public async Task AtualizarAsync_deve_retornar_false_quando_nao_existir()
        {
            var context = CriarContexto(nameof(AtualizarAsync_deve_retornar_false_quando_nao_existir));
            var repo = new RepositorioQuestao(context);

            var questao = CriarQuestao(99);

            var resultado = await repo.AtualizarAsync(questao);

            Assert.False(resultado);
        }

        [Fact]
        public async Task AtualizarAsync_deve_atualizar_campos_quando_existir()
        {
            var context = CriarContexto(nameof(AtualizarAsync_deve_atualizar_campos_quando_existir));

            var questao = CriarQuestao(1);
            context.Questoes.Add(questao);
            await context.SaveChangesAsync();

            questao.AlteradoPor = "TESTE";
            questao.AlteradoRF = "123";
            questao.AlteradoEm = DateTime.Now;

            var repo = new RepositorioQuestao(context);

            var resultado = await repo.AtualizarAsync(questao);

            Assert.True(resultado);
        }

        #endregion

        #region ExcluirAsync

        [Fact]
        public async Task ExcluirAsync_deve_retornar_false_quando_nao_existir()
        {
            var context = CriarContexto(nameof(ExcluirAsync_deve_retornar_false_quando_nao_existir));
            var repo = new RepositorioQuestao(context);

            var resultado = await repo.ExcluirAsync(1);

            Assert.False(resultado);
        }

        [Fact]
        public async Task ExcluirAsync_deve_marcar_como_excluido()
        {
            var context = CriarContexto(nameof(ExcluirAsync_deve_marcar_como_excluido));

            var questao = CriarQuestao(1);
            context.Questoes.Add(questao);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestao(context);

            var resultado = await repo.ExcluirAsync(1);

            var questaoExcluida = await context.Questoes
                .IgnoreQueryFilters() 
                .FirstOrDefaultAsync(q => q.Id == 1);

            Assert.True(resultado);
            Assert.NotNull(questaoExcluida);
            Assert.True(questaoExcluida!.Excluido);
        }

        #endregion

        #region ObterQuestoesAtivasPorFiltroAsync

        [Fact]
        public async Task ObterQuestoesAtivasPorFiltroAsync_deve_retornar_apenas_questoes_validas()
        {
            var context = CriarContexto(nameof(ObterQuestoesAtivasPorFiltroAsync_deve_retornar_apenas_questoes_validas));

            var questionario = new Questionario(
                nome: "Teste",
                tipo: TipoQuestionario.SondagemEscrita,
                anoLetivo: 2025,
                componenteCurricularId: 1,
                proficienciaId: 1,
                sondagemId: 1,
                modalidadeId: 1,
                serieAno: 2
            );

            context.Questionarios.Add(questionario);
            await context.SaveChangesAsync();

            var questao = CriarQuestao(tipo: TipoQuestao.Combo);
            typeof(Questao).GetProperty("Questionario")!.SetValue(questao, questionario);

            context.Questoes.Add(questao);
            await context.SaveChangesAsync();

            var repo = new RepositorioQuestao(context);

            var resultado = await repo.ObterQuestoesAtivasPorFiltroAsync(
                1, 2025, 1, 2
            );

            Assert.Single(resultado);
        }

        #endregion
    }
}
