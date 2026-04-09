using Microsoft.EntityFrameworkCore;
using Moq;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Dominio.ValueObjects;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
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

        private  static RespostaAluno CriarRespostaAluno(
            int alunoId,
            int questaoId,
            int sondagemId = 1,
            bool excluido = false)
        {
            var contextoEdu = CriarContextoEducacional();

            var resposta = new RespostaAluno(
                sondagemId,
                alunoId,
                questaoId,
                opcaoRespostaId: 1,
                dataResposta: DateTime.Now,contextoEdu
            );

            typeof(RespostaAluno).GetProperty("Excluido")!.SetValue(resposta, excluido);
            return resposta;
        }

        private  static RepositorioRespostaAluno CriarRepositorio(SondagemDbContext context)
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
            Assert.Contains((10L, 100L, 2), resultado.Keys);
            Assert.Contains((20L, 200L, 2), resultado.Keys);
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

            var contextoEdu = CriarContextoEducacional();

            var respostaValida = new RespostaAluno(
                sondagemId: 1,
                alunoId: 10,
                questaoId: 100,
                opcaoRespostaId: 1,
                dataResposta: DateTime.Now,
                contextoEdu
            );

            var respostaOutroAluno = new RespostaAluno(
                sondagemId: 1,
                alunoId: 99,
                questaoId: 100,
                opcaoRespostaId: 1,
                dataResposta: DateTime.Now,
                contextoEdu
            );

            var respostaOutraQuestao = new RespostaAluno(
                sondagemId: 1,
                alunoId: 10,
                questaoId: 999,
                opcaoRespostaId: 1,
                dataResposta: DateTime.Now,
                contextoEdu
            );

            var respostaExcluida = new RespostaAluno(
                sondagemId: 1,
                alunoId: 10,
                questaoId: 100,
                opcaoRespostaId: 1,
                dataResposta: DateTime.Now,
                contextoEdu
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

        #region ObterRespostasParaRelatorioConsolidadoAsync

        [Fact]
        public async Task ObterRespostasParaRelatorioConsolidadoAsync_DeveFiltrarPorAnoLetivo()
        {
            // Arrange
            var context = CriarContexto(nameof(ObterRespostasParaRelatorioConsolidadoAsync_DeveFiltrarPorAnoLetivo));
            
            var compCurricular = new ComponenteCurricular("Português", null, "1", 1);
            typeof(ComponenteCurricular).GetProperty("Id")!.SetValue(compCurricular, 1);
            context.ComponentesCurriculares.Add(compCurricular);

            var proficiencia = new SME.Sondagem.Dominio.Entidades.Proficiencia("Leitura", 1, 1);
            typeof(SME.Sondagem.Dominio.Entidades.Proficiencia).GetProperty("Id")!.SetValue(proficiencia, 1);
            context.Proficiencias.Add(proficiencia);

            var sondagem = new Dominio.Entidades.Sondagem.Sondagem("Sondagem", DateTime.Now);
            typeof(Dominio.Entidades.Sondagem.Sondagem).GetProperty("Id")!.SetValue(sondagem, 1);
            context.Sondagens.Add(sondagem);

            var questionario = new Questionario("Questionario", TipoQuestionario.SondagemLeitura, 2026, 1, 1, 1);
            typeof(Questionario).GetProperty("Id")!.SetValue(questionario, 1);
            context.Questionarios.Add(questionario);

            var questao = new Questao(1, 1, "Questao", "", true, TipoQuestao.Combo, "{}", false, 12);
            typeof(Questao).GetProperty("Id")!.SetValue(questao, 1);
            context.Questoes.Add(questao);

            var opcao = new OpcaoResposta(1, "Desc", "L", null, null);
            typeof(OpcaoResposta).GetProperty("Id")!.SetValue(opcao, 1);
            context.OpcoesResposta.Add(opcao);

            // r1: AnoLetivo 2026 (via contextoEducacional)
            var r1 = CriarRespostaAluno(alunoId: 1, questaoId: 1, sondagemId: 1);
            
            // r2: AnoLetivo 2025 (override)
            var contexto2025 = new ContextoEducacional { TurmaId = "1", UeId = "3", DreId = "2", AnoLetivo = 2025, ModalidadeId = "4", BimestreId = 2 };
            var r2 = new RespostaAluno(1, 2, 1, 1, DateTime.Now, contexto2025);

            context.RespostasAluno.AddRange(r1, r2);
            await context.SaveChangesAsync();
            var repo = CriarRepositorio(context);

            // Act
            var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026 };
            var resultado = await repo.ObterRespostasParaRelatorioConsolidadoAsync(filtro);

            // Assert
            var lista = resultado.ToList();
            Assert.Single(lista);
            Assert.Equal(2026, lista[0].AnoLetivo);
        }

        [Fact]
        public async Task ObterRespostasParaRelatorioConsolidadoAsync_DeveProjetarCorretamenteParaDto()
        {
            // Arrange
            var context = CriarContexto(nameof(ObterRespostasParaRelatorioConsolidadoAsync_DeveProjetarCorretamenteParaDto));
            
            var compCurricular = new ComponenteCurricular("Português", null, "1", 1);
            typeof(ComponenteCurricular).GetProperty("Id")!.SetValue(compCurricular, 1);
            context.ComponentesCurriculares.Add(compCurricular);

            var proficiencia = new SME.Sondagem.Dominio.Entidades.Proficiencia("Leitura", 1, 1);
            typeof(SME.Sondagem.Dominio.Entidades.Proficiencia).GetProperty("Id")!.SetValue(proficiencia, 1);
            context.Proficiencias.Add(proficiencia);

            var sondagem = new Dominio.Entidades.Sondagem.Sondagem("Sondagem", DateTime.Now);
            typeof(Dominio.Entidades.Sondagem.Sondagem).GetProperty("Id")!.SetValue(sondagem, 1);
            context.Sondagens.Add(sondagem);

            var questionario = new Questionario("Questionario", TipoQuestionario.SondagemLeitura, 2026, 1, 1, 1);
            typeof(Questionario).GetProperty("Id")!.SetValue(questionario, 1);
            context.Questionarios.Add(questionario);

            var questao = new Questao(1, 1, "Questao", "", true, TipoQuestao.Combo, "{}", false, 12);
            typeof(Questao).GetProperty("Id")!.SetValue(questao, 1);
            context.Questoes.Add(questao);

            var opcao = new OpcaoResposta(1, "Opcao", "L", null, null);
            typeof(OpcaoResposta).GetProperty("Id")!.SetValue(opcao, 1);
            context.OpcoesResposta.Add(opcao);

            var raca = new RacaCor { Descricao = "Indígena", CodigoEolRacaCor = 5 };
            typeof(RacaCor).GetProperty("Id")!.SetValue(raca, 5);
            context.RacaCor.Add(raca);

            var contextoEdu = new ContextoEducacional { TurmaId = "1", UeId = "3", DreId = "2", AnoLetivo = 2026, ModalidadeId = "4", BimestreId = 2, RacaCorId = 5 };
            var resposta = new RespostaAluno(1, 100, 1, 1, DateTime.Now, contextoEdu);

            context.RespostasAluno.Add(resposta);
            await context.SaveChangesAsync();
            var repo = CriarRepositorio(context);

            // Act
            var resultado = await repo.ObterRespostasParaRelatorioConsolidadoAsync(new FiltroConsolidadoDto());

            // Assert
            var dto = resultado.First();
            Assert.Equal("Indígena", dto.RacaCor!.Descricao);
            Assert.Equal("Opcao", dto.OpcaoRespostaDescricao);
        }

        [Fact]
        public async Task ObterRespostasParaRelatorioConsolidadoAsync_NaoDeveRetornarExcluidos()
        {
            // Arrange
            var context = CriarContexto(nameof(ObterRespostasParaRelatorioConsolidadoAsync_NaoDeveRetornarExcluidos));
            var resposta = CriarRespostaAluno(1, 1);
            resposta.Excluido = true;

            context.RespostasAluno.Add(resposta);
            await context.SaveChangesAsync();
            var repo = CriarRepositorio(context);

            // Act
            var resultado = await repo.ObterRespostasParaRelatorioConsolidadoAsync(new FiltroConsolidadoDto());

            // Assert
            Assert.Empty(resultado);
        }

        #endregion

        private static ContextoEducacional CriarContextoEducacional()
        {
            return new ContextoEducacional
            {
                TurmaId = "1",
                UeId = "3",
                DreId = "2",
                AnoLetivo = 2026,
                ModalidadeId = 4,
                RacaCorId = 1,
                GeneroSexoId = 1,
                BimestreId = 2
            };
        }

    }
}