using Moq;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Services.EOL;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using System.Net;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Services
{
    public class AlunoTurmaServiceTeste
    {
        private const string RespostaEol = """
            {
                "codigoAluno": 101,
                "nomeAluno": "ALUNO TESTE",
                "nomeMae": "MAE TESTE",
                "sexo": "M",
                "grupoEtnico": "BRANCA",
                "endereco": { "bairro": "BAIRRO TESTE", "logradouro": "RUA TESTE" },
                "nis": "00000000000",
                "cns": "000000000000000"
            }
            """;

        private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
        private readonly Mock<IRepositorioCache> _repositorioCache = new();

        private sealed class HandlerFake(HttpStatusCode status, string conteudo) : HttpMessageHandler
        {
            public int Chamadas { get; private set; }
            public Uri? UltimaUrl { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Chamadas++;
                UltimaUrl = request.RequestUri;
                return Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(conteudo) });
            }
        }

        private AlunoTurmaService CriarService(HandlerFake handler)
        {
            _httpClientFactory
                .Setup(x => x.CreateClient(ServicoEolConstants.SERVICO))
                .Returns(new HttpClient(handler) { BaseAddress = new Uri("http://eol.teste/api/") });

            return new AlunoTurmaService(_httpClientFactory.Object, _repositorioCache.Object);
        }

        [Fact]
        public async Task DeveConsultarEndpointDoAluno_ERetornarRacaESexo()
        {
            var handler = new HandlerFake(HttpStatusCode.OK, RespostaEol);

            var resultado = await CriarService(handler).ObterRacaSexoAluno(101);

            Assert.NotNull(resultado);
            Assert.Equal("M", resultado!.Sexo);
            Assert.Equal("BRANCA", resultado.GrupoEtnico);
            Assert.EndsWith("alunos/101/informacoes", handler.UltimaUrl!.AbsolutePath);
        }

        [Fact]
        public async Task DeveGuardarEmCacheApenasRacaESexo_SemDadosPessoais()
        {
            object? valorEmCache = null;
            _repositorioCache
                .Setup(x => x.SalvarRedisAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()))
                .Callback<string, object, int>((_, valor, _) => valorEmCache = valor)
                .Returns(Task.CompletedTask);

            await CriarService(new HandlerFake(HttpStatusCode.OK, RespostaEol)).ObterRacaSexoAluno(101);

            var serializado = JsonConvert.SerializeObject(valorEmCache);
            Assert.Contains("BRANCA", serializado);
            Assert.DoesNotContain("ALUNO TESTE", serializado);
            Assert.DoesNotContain("MAE TESTE", serializado);
            Assert.DoesNotContain("RUA TESTE", serializado);
            Assert.DoesNotContain("00000000000", serializado);
        }

        [Fact]
        public async Task DeveUsarCache_SemChamarEol()
        {
            _repositorioCache
                .Setup(x => x.ObterRedisAsync<AlunoRacaSexoEolDto>(It.IsAny<string>()))
                .ReturnsAsync(new AlunoRacaSexoEolDto { Sexo = "F", GrupoEtnico = "PARDA" });
            var handler = new HandlerFake(HttpStatusCode.OK, RespostaEol);

            var resultado = await CriarService(handler).ObterRacaSexoAluno(101);

            Assert.Equal("PARDA", resultado!.GrupoEtnico);
            Assert.Equal(0, handler.Chamadas);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.NoContent)]
        public async Task DeveRetornarNulo_SemGuardarCache_QuandoEolNaoRetornarDados(HttpStatusCode status)
        {
            var resultado = await CriarService(new HandlerFake(status, string.Empty)).ObterRacaSexoAluno(101);

            Assert.Null(resultado);
            _repositorioCache.Verify(x => x.SalvarRedisAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.Unauthorized)]
        public async Task DeveLancarExcecao_SemGuardarCache_QuandoEolRetornarErro(HttpStatusCode status)
        {
            var service = CriarService(new HandlerFake(status, string.Empty));

            await Assert.ThrowsAsync<HttpRequestException>(() => service.ObterRacaSexoAluno(101));

            _repositorioCache.Verify(x => x.SalvarRedisAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()), Times.Never);
        }

        private sealed class HandlerTravado : HttpMessageHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task DeveDesistirEmDezSegundos_QuandoEolTravar()
        {
            _httpClientFactory
                .Setup(x => x.CreateClient(ServicoEolConstants.SERVICO))
                .Returns(new HttpClient(new HandlerTravado()) { BaseAddress = new Uri("http://eol.teste/api/") });
            var service = new AlunoTurmaService(_httpClientFactory.Object, _repositorioCache.Object);
            var cronometro = System.Diagnostics.Stopwatch.StartNew();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.ObterRacaSexoAluno(101));

            Assert.True(cronometro.Elapsed < TimeSpan.FromSeconds(30));
        }
    }
}
