using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.API.Teste
{
    public class QuestionarioControllerTeste
    {
        private readonly Mock<IObterQuestionarioSondagemUseCase> obterQuestionarioSondagemUseCaseMock;
        private readonly QuestionarioController controller;

        public QuestionarioControllerTeste()
        {
            obterQuestionarioSondagemUseCaseMock = new Mock<IObterQuestionarioSondagemUseCase>();
            controller = new QuestionarioController();
        }

        [Fact]
        public async Task ObterQuestionario_DeveRetornarOkComQuestionario()
        {
            var filtro = new FiltroQuestionario();
            var cancellationToken = CancellationToken.None;

            var questionarioEsperado = new QuestionarioSondagemDto();

            obterQuestionarioSondagemUseCaseMock
                .Setup(x => x.ObterQuestionarioSondagem(filtro, cancellationToken))
                .ReturnsAsync(questionarioEsperado);

            var result = await controller.ObterQuestionario(
                filtro,
                obterQuestionarioSondagemUseCaseMock.Object,
                cancellationToken);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsType<QuestionarioSondagemDto>(okResult.Value);

            Assert.Equal(questionarioEsperado, retorno);

            obterQuestionarioSondagemUseCaseMock.Verify(
                x => x.ObterQuestionarioSondagem(filtro, cancellationToken),
                Times.Once);
        }
    }
}
