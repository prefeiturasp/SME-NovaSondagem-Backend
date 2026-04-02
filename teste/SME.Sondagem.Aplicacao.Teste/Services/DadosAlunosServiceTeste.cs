using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Services.EOL;
using SME.Sondagem.Infrastructure.Dtos;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using System.Net;
using System.Text;
using Xunit;


namespace SME.Sondagem.Aplicacao.Teste.Services

{
    public class DadosAlunosServiceTeste
    {
        private readonly Mock<IAlunoTurmaService> _alunoTurmaService;
        private readonly Mock<IHttpClientFactory> _httpClientFactory;
        private readonly Mock<HttpMessageHandler> _httpMessageHandler;

        public DadosAlunosServiceTeste()
        {
            _alunoTurmaService = new Mock<IAlunoTurmaService>();
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _httpMessageHandler = new Mock<HttpMessageHandler>();

            var httpClient = new HttpClient(_httpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://mock-local/")
            };

            _httpClientFactory
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);
        }

        private void ConfigurarRespostaHttp(HttpResponseMessage resposta)
        {
            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(resposta);
        }

        private DadosAlunosService CriarService()
        {
            return new DadosAlunosService(_httpClientFactory.Object, _alunoTurmaService.Object);
        }


        [Fact]
        public async Task DeveRetornarVazio_QuandoDadosAlunosNulo()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<DadosAlunoPorTurmaDto>?)null);

            var resultado = await CriarService().ObterDadosRacaGeneroAlunos(1);

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task DeveRetornarVazio_QuandoDadosAlunosVazio()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>());

            var resultado = await CriarService().ObterDadosRacaGeneroAlunos(1);

            Assert.Empty(resultado);
        }

        [Theory]
        [InlineData("M", "Masculino")]
        [InlineData("F", "Feminino")]
        [InlineData("X", "X")]
        [InlineData("", "")]
        public async Task DeveConverterGeneroCorretamente(string codigoGenero, string generoEsperado)
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
            new() { CodigoAluno = 101, Raca = "PARDA", Sexo = codigoGenero, NomeAluno = "Teste" }
                });

            var resultado = (await CriarService().ObterDadosRacaGeneroAlunos(1)).ToList();

            var item = Assert.Single(resultado);
            Assert.Equal(generoEsperado, item.Sexo);
        }

        [Fact]
        public async Task DeveConverterGeneroNulo_RetornarStringVazia()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                new() { CodigoAluno = 101, Raca = "PARDA", Sexo = null, NomeAluno = "Teste" }
                        });

            var resultado = (await CriarService().ObterDadosRacaGeneroAlunos(1)).ToList();

            var item = Assert.Single(resultado);
            Assert.Equal(string.Empty, item.Sexo);
        }

        [Theory]
        [InlineData("BRANCA", "Branca")]
        [InlineData("PRETA", "Preta")]
        [InlineData("PARDA", "Parda")]
        [InlineData("AMARELA", "Amarela")]
        [InlineData("INDIGENA", "Indígena")]
        [InlineData("INDÍGENA", "Indígena")]
        [InlineData("OUTRA", "Outra")]
        [InlineData("", "")]
        public async Task DeveConverterRacaCorretamente(string codigoRaca, string racaEsperada)
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                new() { CodigoAluno = 101, Raca = codigoRaca, Sexo = "M", NomeAluno = "Teste" }
                        });


            var resultado = (await CriarService().ObterDadosRacaGeneroAlunos(1)).ToList();

            var item = Assert.Single(resultado);
            Assert.Equal(racaEsperada, item.Raca);
        }

        [Fact]
        public async Task DeveConverterRacaNula_RetornarStringVazia()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                new() { CodigoAluno = 101, Raca = null, Sexo = "M", NomeAluno = "Teste" }
                        });

            var resultado = (await CriarService().ObterDadosRacaGeneroAlunos(1)).ToList();

            var item = Assert.Single(resultado);
            Assert.Equal(string.Empty, item.Raca);
        }


        [Fact]
        public async Task DeveRetornarListaVazia_QuandoCodigoAlunosVazio()
        {
            var resultado = await CriarService().ObterDadosAlunosPorCodigoUe(new List<string>());

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task DeveRetornarAlunos_QuandoRespostaHttpComSucesso()
        {
            var alunosEsperados = new List<AlunoEolDto>
        {
            new() { CodigoAluno = 123, NomeAluno = "João" }
        };

            ConfigurarRespostaHttp(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(alunosEsperados),
                    Encoding.UTF8,
                    "application/json")
            });

            var resultado = await CriarService().ObterDadosAlunosPorCodigoUe(new List<string> { "123" });

            Assert.Single(resultado);
        }

        [Fact]
        public async Task DeveRetornarListaVazia_QuandoRespostaHttpForErro()
        {
            ConfigurarRespostaHttp(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            var resultado = await CriarService().ObterDadosAlunosPorCodigoUe(new List<string> { "123" });

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task DeveLancarOperationCanceled_QuandoCancelado()
        {
            ConfigurarRespostaHttp(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]", Encoding.UTF8, "application/json")
            });

            var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                CriarService().ObterDadosAlunosPorCodigoUe(new List<string> { "123" }, cts.Token));
        }


        [Fact]
        public void DeveLancarArgumentNullException_QuandoHttpClientFactoryNulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DadosAlunosService(null!, _alunoTurmaService.Object));
        }

        [Fact]
        public void DeveLancarArgumentNullException_QuandoAlunoTurmaServiceNulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DadosAlunosService(_httpClientFactory.Object, null!));
        }
    }
}