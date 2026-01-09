using Moq;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Services.EOL;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using System.Net;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Services
{
    public class AlunoPapServiceTeste
    {
        private readonly Mock<IHttpClientFactory> httpClientFactoryMock;

        public AlunoPapServiceTeste()
        {
            httpClientFactoryMock = new Mock<IHttpClientFactory>();
        }

        [Fact]
        public async Task VerificarAlunosPossuemProgramaPapAsync_CodigosNulos_DeveRetornarVazio()
        {
            var service = new AlunoPapService(httpClientFactoryMock.Object);

            var result = await service.VerificarAlunosPossuemProgramaPapAsync(null, 2024);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerificarAlunosPossuemProgramaPapAsync_CodigosVazios_DeveRetornarVazio()
        {
            var service = new AlunoPapService(httpClientFactoryMock.Object);

            var result = await service.VerificarAlunosPossuemProgramaPapAsync(Array.Empty<int>(), 2024);

            Assert.Empty(result);
        }

        [Fact]
        public async Task VerificarAlunosPossuemProgramaPapAsync_ResponseOk_ComConteudo_DeveMapearCorretamente()
        {
            var codigos = new[] { 1, 2, 3 };

            var alunosPap = new[]
            {
            new DadosMatriculaAlunoTipoPapDto { CodigoAluno = 1 },
            new DadosMatriculaAlunoTipoPapDto { CodigoAluno = 3 }
        };

            var json = JsonConvert.SerializeObject(alunosPap);

            var httpClient = HttpClientMockHelper.Create(HttpStatusCode.OK, json);

            httpClientFactoryMock
                .Setup(x => x.CreateClient(ServicoEolConstants.SERVICO))
                .Returns(httpClient);

            var service = new AlunoPapService(httpClientFactoryMock.Object);

            var result = await service.VerificarAlunosPossuemProgramaPapAsync(codigos, 2024);

            Assert.True(result[1]);
            Assert.False(result[2]);
            Assert.True(result[3]);
        }

        [Fact]
        public async Task VerificarAlunosPossuemProgramaPapAsync_ResponseNoContent_DeveRetornarTodosFalse()
        {
            var codigos = new[] { 10, 20 };

            var httpClient = HttpClientMockHelper.Create(HttpStatusCode.NoContent);

            httpClientFactoryMock
                .Setup(x => x.CreateClient(ServicoEolConstants.SERVICO))
                .Returns(httpClient);

            var service = new AlunoPapService(httpClientFactoryMock.Object);

            var result = await service.VerificarAlunosPossuemProgramaPapAsync(codigos, 2024);

            Assert.All(result.Values, Assert.False);
        }

        [Fact]
        public async Task VerificarAlunosPossuemProgramaPapAsync_ResponseErro_DeveRetornarTodosFalse()
        {
            var codigos = new[] { 5, 6 };

            var httpClient = HttpClientMockHelper.Create(HttpStatusCode.InternalServerError);

            httpClientFactoryMock
                .Setup(x => x.CreateClient(ServicoEolConstants.SERVICO))
                .Returns(httpClient);

            var service = new AlunoPapService(httpClientFactoryMock.Object);

            var result = await service.VerificarAlunosPossuemProgramaPapAsync(codigos, 2024);

            Assert.All(result.Values, Assert.False);
        }

        [Fact]
        public async Task VerificarAlunosPossuemProgramaPapAsync_ResponseOk_ConteudoVazio_DeveRetornarTodosFalse()
        {
            var codigos = new[] { 7, 8 };

            var httpClient = HttpClientMockHelper.Create(HttpStatusCode.OK, string.Empty);

            httpClientFactoryMock
                .Setup(x => x.CreateClient(ServicoEolConstants.SERVICO))
                .Returns(httpClient);

            var service = new AlunoPapService(httpClientFactoryMock.Object);

            var result = await service.VerificarAlunosPossuemProgramaPapAsync(codigos, 2024);

            Assert.All(result.Values, Assert.False);
        }
    }
}
