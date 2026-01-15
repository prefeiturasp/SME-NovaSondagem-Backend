using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioRespostaAlunoTeste : RepositorioBaseTeste
    {
        #region Helpers

        private static Questao CriarQuestao(
            int id,
            TipoQuestao tipo)
        {
            var questao = new Questao(
                questionarioId: 1,
                ordem: 1,
                nome: "Questão",
                observacao: "",
                obrigatorio: true,
                tipo: tipo,
                opcionais: "{}",
                somenteLeitura: false,
                dimensao: 12
            );

            typeof(Questao).GetProperty("Id")!.SetValue(questao, id);
            return questao;
        }

        private static RespostaAluno CriarRespostaAluno(
            int alunoId,
            int questaoId,
            int sondagemId = 1,
            bool excluido = false)
        {
            var resposta = new RespostaAluno(
                sondagemId,
                alunoId,
                questaoId,
                opcaoRespostaId: 1,
                dataResposta: DateTime.Now
            );

            typeof(RespostaAluno).GetProperty("Excluido")!.SetValue(resposta, excluido);
            return resposta;
        }

        #endregion

        #region VerificarAlunoTemRespostaPorTipoQuestaoAsync

        [Fact]
        public async Task VerificarAlunoTemRespostaPorTipoQuestaoAsync_deve_retornar_true_quando_existir()
        {
            var context =
                CriarContexto(nameof(VerificarAlunoTemRespostaPorTipoQuestaoAsync_deve_retornar_true_quando_existir));

            var servicoAuditoria = CriarServicoAuditoria();
            var questao = CriarQuestao(1, TipoQuestao.Combo);
            context.Questoes.Add(questao);

            var resposta = CriarRespostaAluno(alunoId: 10, questaoId: 1);
            typeof(RespostaAluno).GetProperty("Questao")!.SetValue(resposta, questao);

            context.RespostasAluno.Add(resposta);
            await context.SaveChangesAsync();

            var repo = new RepositorioRespostaAluno(context, servicoAuditoria);

            var resultado = await repo.VerificarAlunoTemRespostaPorTipoQuestaoAsync(
                alunoId: 10,
                tipoQuestao: TipoQuestao.Combo,
                CancellationToken.None);

            Assert.True(resultado);
        }

        [Fact]
        public async Task VerificarAlunoTemRespostaPorTipoQuestaoAsync_deve_retornar_false_quando_nao_existir()
        {
            var context =
                CriarContexto(
                    nameof(VerificarAlunoTemRespostaPorTipoQuestaoAsync_deve_retornar_false_quando_nao_existir));

            var servicoAuditoria = CriarServicoAuditoria();
            var repo = new RepositorioRespostaAluno(context, servicoAuditoria);

            var resultado = await repo.VerificarAlunoTemRespostaPorTipoQuestaoAsync(
                alunoId: 99,
                tipoQuestao: TipoQuestao.Texto,
                CancellationToken.None);

            Assert.False(resultado);
        }

        #endregion

        #region VerificarAlunosTemRespostaPorTipoQuestaoAsync

        [Fact]
        public async Task VerificarAlunosTemRespostaPorTipoQuestaoAsync_deve_retornar_dicionario_com_true_e_false()
        {
            var context =
                CriarContexto(
                    nameof(VerificarAlunosTemRespostaPorTipoQuestaoAsync_deve_retornar_dicionario_com_true_e_false));

            var servicoAuditoria = CriarServicoAuditoria();
            var questao = CriarQuestao(1, TipoQuestao.Combo);
            context.Questoes.Add(questao);

            var resposta = CriarRespostaAluno(alunoId: 1, questaoId: 1);
            typeof(RespostaAluno).GetProperty("Questao")!.SetValue(resposta, questao);

            context.RespostasAluno.Add(resposta);
            await context.SaveChangesAsync();

            var repo = new RepositorioRespostaAluno(context, servicoAuditoria);

            var alunosIds = new List<int> { 1, 2 };

            var resultado = await repo.VerificarAlunosTemRespostaPorTipoQuestaoAsync(
                alunosIds,
                TipoQuestao.Combo,
                CancellationToken.None);

            Assert.Equal(2, resultado.Count);
            Assert.True(resultado[1]);
            Assert.False(resultado[2]);
        }

        #endregion

        #region ObterRespostasAlunosPorQuestoesAsync

        [Fact]
        public async Task ObterRespostasAlunosPorQuestoesAsync_deve_retornar_dicionario_com_chave_composta()
        {
            var context =
                CriarContexto(nameof(ObterRespostasAlunosPorQuestoesAsync_deve_retornar_dicionario_com_chave_composta));

            var servicoAuditoria = CriarServicoAuditoria();
            var resposta1 = CriarRespostaAluno(alunoId: 10, questaoId: 100, sondagemId: 1);
            var resposta2 = CriarRespostaAluno(alunoId: 20, questaoId: 200, sondagemId: 1);

            context.RespostasAluno.AddRange(resposta1, resposta2);
            await context.SaveChangesAsync();

            var repo = new RepositorioRespostaAluno(context, servicoAuditoria);

            var resultado = await repo.ObterRespostasAlunosPorQuestoesAsync(
                codigosAlunos: new List<long> { 10, 20 },
                questoesIds: new List<long> { 100, 200 },
                sondagemId: 1,
                CancellationToken.None);

            Assert.Equal(2, resultado.Count);
            Assert.Contains((10L, 100L), resultado.Keys);
            Assert.Contains((20L, 200L), resultado.Keys);
        }

        [Fact]
        public async Task ObterRespostasAlunosPorQuestoesAsync_nao_deve_retornar_respostas_excluidas()
        {
            var context =
                CriarContexto(nameof(ObterRespostasAlunosPorQuestoesAsync_nao_deve_retornar_respostas_excluidas));

            var servicoAuditoria = CriarServicoAuditoria();
            var resposta = CriarRespostaAluno(alunoId: 10, questaoId: 100, sondagemId: 1, excluido: true);

            context.RespostasAluno.Add(resposta);
            await context.SaveChangesAsync();

            var repo = new RepositorioRespostaAluno(context, servicoAuditoria);

            var resultado = await repo.ObterRespostasAlunosPorQuestoesAsync(
                codigosAlunos: new List<long> { 10 },
                questoesIds: new List<long> { 100 },
                sondagemId: 1,
                CancellationToken.None);

            Assert.Empty(resultado);
        }

        #endregion
    }
}