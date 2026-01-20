using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.API.Teste.Controller
{
    public class QuestionarioControllerTeste
    {
        private readonly Mock<IObterQuestionarioSondagemUseCase> _obterQuestionarioSondagemUseCaseMock;
        private readonly QuestionarioController _controller;

        public QuestionarioControllerTeste()
        {
            _obterQuestionarioSondagemUseCaseMock = new Mock<IObterQuestionarioSondagemUseCase>();
            _controller = new QuestionarioController();
        }

        [Fact]
        public async Task ObterQuestionario_DeveRetornarOkComQuestionario()
        {
            // Arrange
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                ProficienciaId = 1,
                ComponenteCurricularId = 1,
                Modalidade = 1,
                Ano = 1
            };
            var cancellationToken = CancellationToken.None;

            var questionarioEsperado = new QuestionarioSondagemDto
            {
                QuestaoId = 10,
                SondagemId = 5,
                TituloTabelaRespostas = "Questionário de Matemática",
                Estudantes = new List<EstudanteQuestionarioDto>
                {
                    new EstudanteQuestionarioDto
                    {
                        NumeroAlunoChamada = "1",
                        Codigo = 12345,
                        Nome = "João da Silva",
                        LinguaPortuguesaSegundaLingua = false,
                        Pap = false,
                        Aee = false,
                        PossuiDeficiencia = false,
                        Coluna = new List<ColunaQuestionarioDto>
                        {
                            new ColunaQuestionarioDto
                            {
                                IdCiclo = 1,
                                DescricaoColuna = "1º Bimestre",
                                PeriodoBimestreAtivo = true,
                                OpcaoResposta = new List<OpcaoRespostaDto>
                                {
                                    new OpcaoRespostaDto
                                    {
                                        Id = 1,
                                        Ordem = 1,
                                        DescricaoOpcaoResposta = "Sim",
                                        Legenda = "S",
                                        CorFundo = "#00FF00",
                                        CorTexto = "#000000"
                                    }
                                },
                                Resposta = new RespostaDto
                                {
                                    Id = 1,
                                    OpcaoRespostaId = 1
                                }
                            }
                        }
                    }
                }
            };

            _obterQuestionarioSondagemUseCaseMock
                .Setup(x => x.ObterQuestionarioSondagem(filtro, cancellationToken))
                .ReturnsAsync(questionarioEsperado);

            // Act
            var result = await _controller.ObterQuestionario(
                filtro,
                _obterQuestionarioSondagemUseCaseMock.Object,
                cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsType<QuestionarioSondagemDto>(okResult.Value);

            Assert.Equal(questionarioEsperado.QuestaoId, retorno.QuestaoId);
            Assert.Equal(questionarioEsperado.SondagemId, retorno.SondagemId);
            Assert.Equal(questionarioEsperado.TituloTabelaRespostas, retorno.TituloTabelaRespostas);
            Assert.NotNull(retorno.Estudantes);
            Assert.Single(retorno.Estudantes);

            var estudante = retorno.Estudantes.First();
            Assert.Equal("João da Silva", estudante.Nome);
            Assert.Equal(12345, estudante.Codigo);

            _obterQuestionarioSondagemUseCaseMock.Verify(
                x => x.ObterQuestionarioSondagem(filtro, cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task ObterQuestionario_DeveRetornarOkComListaVazia_QuandoNaoHouverEstudantes()
        {
            // Arrange
            var filtro = new FiltroQuestionario
            {
                TurmaId = 2,
                ProficienciaId = 1
            };
            var cancellationToken = CancellationToken.None;

            var questionarioEsperado = new QuestionarioSondagemDto
            {
                QuestaoId = 10,
                SondagemId = 5,
                TituloTabelaRespostas = "Questionário Vazio",
                Estudantes = new List<EstudanteQuestionarioDto>()
            };

            _obterQuestionarioSondagemUseCaseMock
                .Setup(x => x.ObterQuestionarioSondagem(filtro, cancellationToken))
                .ReturnsAsync(questionarioEsperado);

            // Act
            var result = await _controller.ObterQuestionario(
                filtro,
                _obterQuestionarioSondagemUseCaseMock.Object,
                cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsType<QuestionarioSondagemDto>(okResult.Value);

            Assert.NotNull(retorno.Estudantes);
            Assert.Empty(retorno.Estudantes);

            _obterQuestionarioSondagemUseCaseMock.Verify(
                x => x.ObterQuestionarioSondagem(filtro, cancellationToken),
                Times.Once);
        }
    }
}
