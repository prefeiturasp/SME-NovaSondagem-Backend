using Microsoft.EntityFrameworkCore;
using Moq;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dados.Teste.Services.Auditoria;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioRespostaAlunoTeste : RepositorioBaseTeste
    {
        #region Constantes

        private static readonly int[] QuestoesIdsPadrao = [1, 2];
        private static readonly int[] AlunosIdsPadrao = [10, 20];
        private static readonly int[] AlunoIdUnico = [10];
        private static readonly int[] QuestaoIdUnica = [100];

        #endregion

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

        private  RepositorioRespostaAluno CriarRepositorio(SondagemDbContext context)
        {
            var auditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBase = CriarConextoBase();
            return new RepositorioRespostaAluno(context, auditoriaMock.Object, contextoBase);
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
            var contextoBase = CriarConextoBase();
            context.Questoes.Add(questao);

            var resposta = CriarRespostaAluno(alunoId: 10, questaoId: 1);
            typeof(RespostaAluno).GetProperty("Questao")!.SetValue(resposta, questao);

            context.RespostasAluno.Add(resposta);
            await context.SaveChangesAsync();

            var repo = new RepositorioRespostaAluno(context, servicoAuditoria,contextoBase);

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
            var contextoBase = CriarConextoBase();
            var repo = new RepositorioRespostaAluno(context, servicoAuditoria, contextoBase);

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

            var questao = CriarQuestao(1, TipoQuestao.LinguaPortuguesaSegundaLingua);
            context.Questoes.Add(questao);

            var opcaoResposta = new OpcaoResposta(
                ordem: 1,
                descricaoOpcaoResposta: "Sim",
                legenda: null,
                corFundo: null,
                corTexto: null
            );
            typeof(OpcaoResposta).GetProperty("Id")!.SetValue(opcaoResposta, 1);
            context.OpcoesResposta.Add(opcaoResposta);

            var resposta = CriarRespostaAluno(alunoId: 1, questaoId: 1);
            typeof(RespostaAluno).GetProperty("Questao")!.SetValue(resposta, questao);
            typeof(RespostaAluno).GetProperty("OpcaoResposta")!.SetValue(resposta, opcaoResposta);

            context.RespostasAluno.Add(resposta);
            await context.SaveChangesAsync();
            var contextoBase = CriarConextoBase();
            var repo = new RepositorioRespostaAluno(context, servicoAuditoria, contextoBase);

            var alunosIds = new List<int> { 1, 2 };

            var resultado = await repo.VerificarAlunosPossuiLinguaPortuguesaAsync(
                alunosIds,
                questao,
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
            var contextoBase = CriarConextoBase();
            var repo = new RepositorioRespostaAluno(context, servicoAuditoria, contextoBase);

            var resultado = await repo.ObterRespostasAlunosPorQuestoesAsync(
                codigosAlunos: new List<long> { 10, 20 },
                questoesIds: new List<long> { 100, 200 },
                sondagemId: 1,
                CancellationToken.None);

            Assert.Equal(2, resultado.Count);
            Assert.Contains((10L, 100L, null), resultado.Keys);
            Assert.Contains((20L, 200L, null), resultado.Keys);
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
            var contextoBase = CriarConextoBase();
            var repo = new RepositorioRespostaAluno(context, servicoAuditoria, contextoBase);

            var resultado = await repo.ObterRespostasAlunosPorQuestoesAsync(
                codigosAlunos: new List<long> { 10 },
                questoesIds: new List<long> { 100 },
                sondagemId: 1,
                CancellationToken.None);

            Assert.Empty(resultado);
        }

        #endregion

        [Fact]
        public async Task ObterRespostasPorSondagemEAlunosAsync_DeveRetornarVazio_QuandoAlunosIdsVazio()
        {
            // Arrange
            var context = CriarContexto(nameof(ObterRespostasPorSondagemEAlunosAsync_DeveRetornarVazio_QuandoAlunosIdsVazio));
            var repo = CriarRepositorio(context);

            // Act
            var resultado = await repo.ObterRespostasPorSondagemEAlunosAsync(
                sondagemId: 1,
                alunosIds: [],
                questoesIds: QuestoesIdsPadrao
            );

            // Assert
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task ObterRespostasPorSondagemEAlunosAsync_DeveRetornarVazio_QuandoQuestoesIdsVazio()
        {
            // Arrange
            var context = CriarContexto(nameof(ObterRespostasPorSondagemEAlunosAsync_DeveRetornarVazio_QuandoQuestoesIdsVazio));
            var repo = CriarRepositorio(context);

            // Act
            var resultado = await repo.ObterRespostasPorSondagemEAlunosAsync(
                sondagemId: 1,
                alunosIds: AlunosIdsPadrao,
                questoesIds: []
            );

            // Assert
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task ObterRespostasPorSondagemEAlunosAsync_DeveRetornarRespostasCorretas()
        {
            // Arrange
            var context = CriarContexto(nameof(ObterRespostasPorSondagemEAlunosAsync_DeveRetornarRespostasCorretas));

            var respostaValida = new RespostaAluno(
                sondagemId: 1,
                alunoId: 10,
                questaoId: 100,
                opcaoRespostaId: 1,
                dataResposta: DateTime.Now
            );

            var respostaOutroAluno = new RespostaAluno(
                sondagemId: 1,
                alunoId: 99,
                questaoId: 100,
                opcaoRespostaId: 1,
                dataResposta: DateTime.Now
            );

            var respostaOutraQuestao = new RespostaAluno(
                sondagemId: 1,
                alunoId: 10,
                questaoId: 999,
                opcaoRespostaId: 1,
                dataResposta: DateTime.Now
            );

            var respostaExcluida = new RespostaAluno(
                sondagemId: 1,
                alunoId: 10,
                questaoId: 100,
                opcaoRespostaId: 1,
                dataResposta: DateTime.Now
            );
            respostaExcluida.Excluido = true;

            context.RespostasAluno.AddRange(
                respostaValida,
                respostaOutroAluno,
                respostaOutraQuestao,
                respostaExcluida
            );
            await context.SaveChangesAsync();

            var repo = CriarRepositorio(context);

            // Act
            var resultado = await repo.ObterRespostasPorSondagemEAlunosAsync(
                sondagemId: 1,
                alunosIds: AlunoIdUnico,
                questoesIds: QuestaoIdUnica
            );

            // Assert
            var lista = resultado.ToList();

            Assert.Single(lista);
            Assert.Equal(respostaValida.Id, lista[0].Id);
        }

        private static ContextoFake CriarConextoBase()
        {
            var contexto = new ContextoFake();
            contexto.AdicionarVariaveis(new Dictionary<string, object>
                {
                    { "NomeUsuario", "Usuario Teste" },
                    { "RF", "123456" },
                    { "Administrador", "true" }
                });

            return contexto;
        }
    }
}