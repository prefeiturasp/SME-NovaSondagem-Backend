using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Services.EOL;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.ValueObjects;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using System.Net;
using Xunit;


namespace SME.Sondagem.Aplicacao.Teste.Services

{
    public class DadosAlunosServiceTeste
    {
        private readonly Mock<IAlunoTurmaService> _alunoTurmaService;
        private readonly Mock<IHttpClientFactory> _httpClientFactory;
        private readonly Mock<IRepositorioGeneroSexo> _repositorioGeneroSexo;
        private readonly Mock<IRepositorioRacaCor> _repositorioRacaCor;
        private readonly Mock<ILogger<DadosAlunosService>> _logger;

        public DadosAlunosServiceTeste()
        {
            _alunoTurmaService = new Mock<IAlunoTurmaService>();
            _repositorioRacaCor = new Mock<IRepositorioRacaCor>();
            _repositorioGeneroSexo = new Mock<IRepositorioGeneroSexo>();
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _logger = new Mock<ILogger<DadosAlunosService>>();
            _logger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        }


        private DadosAlunosService CriarService()
        {
            return new DadosAlunosService(_httpClientFactory.Object, _alunoTurmaService.Object, _repositorioGeneroSexo.Object, _repositorioRacaCor.Object, _logger.Object);
        }


        private void ConfigurarCatalogosRacaGenero()
        {
            _repositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RacaCor>
                {
                    new() { Id = 2, Descricao = "BRANCA", CodigoEolRacaCor = 1 },
                    new() { Id = 4, Descricao = "PARDA", CodigoEolRacaCor = 3 }
                });
            _repositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GeneroSexo>
                {
                    new() { Id = 1, Descricao = "Masculino", Sigla = "M" },
                    new() { Id = 2, Descricao = "Feminino", Sigla = "F" }
                });
        }

        private static RespostaAluno CriarRespostaSalva(int alunoId, int? racaCorId, int? generoSexoId) =>
            new(1, alunoId, 3, 2, DateTime.UtcNow, new ContextoEducacional { RacaCorId = racaCorId, GeneroSexoId = generoSexoId });

        [Fact]
        public async Task ForaDaListaTurma_DeveUsarValoresSalvos_SemConsultarEol()
        {
            ConfigurarCatalogosRacaGenero();

            var resultado = await CriarService().ObterDadosRacaGeneroAlunosForaDaListaTurma(
                [101],
                [CriarRespostaSalva(101, null, null), CriarRespostaSalva(101, 2, 1)]);

            var item = Assert.Single(resultado);
            Assert.Equal(101, item.CodigoAluno);
            Assert.Equal(2, item.RacaId);
            Assert.Equal(1, item.SexoId);
            Assert.Equal("Branca", item.Raca);
            Assert.Equal("Masculino", item.Sexo);
            _alunoTurmaService.Verify(x => x.ObterRacaSexoAluno(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ForaDaListaTurma_DeveConsultarEol_QuandoNaoHouverValoresSalvos()
        {
            ConfigurarCatalogosRacaGenero();
            _alunoTurmaService.Setup(x => x.ObterRacaSexoAluno(101, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AlunoRacaSexoEolDto { Sexo = "M", GrupoEtnico = "BRANCA" });

            var resultado = await CriarService().ObterDadosRacaGeneroAlunosForaDaListaTurma([101], []);

            var item = Assert.Single(resultado);
            Assert.Equal(2, item.RacaId);
            Assert.Equal(1, item.SexoId);
            Assert.Equal("Branca", item.Raca);
            Assert.Equal("Masculino", item.Sexo);
        }

        [Fact]
        public async Task ForaDaListaTurma_DeveManterValorSalvo_EBuscarNoEolApenasOQueFalta()
        {
            ConfigurarCatalogosRacaGenero();
            _alunoTurmaService.Setup(x => x.ObterRacaSexoAluno(101, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AlunoRacaSexoEolDto { Sexo = "F", GrupoEtnico = "BRANCA" });

            var resultado = await CriarService().ObterDadosRacaGeneroAlunosForaDaListaTurma(
                [101],
                [CriarRespostaSalva(101, 4, null)]);

            var item = Assert.Single(resultado);
            Assert.Equal(4, item.RacaId);
            Assert.Equal(2, item.SexoId);
        }

        [Fact]
        public async Task ForaDaListaTurma_NaoDeveFalhar_QuandoEolLancarExcecao()
        {
            ConfigurarCatalogosRacaGenero();
            _alunoTurmaService.Setup(x => x.ObterRacaSexoAluno(101, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("EOL indisponível"));

            var resultado = await CriarService().ObterDadosRacaGeneroAlunosForaDaListaTurma([101], []);

            var item = Assert.Single(resultado);
            Assert.Null(item.RacaId);
            Assert.Null(item.SexoId);
            Assert.Equal(string.Empty, item.Raca);
            Assert.Equal(string.Empty, item.Sexo);
        }

        [Fact]
        public async Task ForaDaListaTurma_NaoDeveFalhar_QuandoConsultaAoEolEstourarTempo()
        {
            ConfigurarCatalogosRacaGenero();
            _alunoTurmaService.Setup(x => x.ObterRacaSexoAluno(101, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Tempo esgotado"));

            var resultado = await CriarService().ObterDadosRacaGeneroAlunosForaDaListaTurma([101], []);

            var item = Assert.Single(resultado);
            Assert.Null(item.RacaId);
            Assert.Null(item.SexoId);
            VerificarAvisosRegistrados(Times.Once());
        }

        [Fact]
        public async Task ForaDaListaTurma_DeveRepassarCancelamento_QuandoFeitoPorQuemChamou()
        {
            ConfigurarCatalogosRacaGenero();
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                CriarService().ObterDadosRacaGeneroAlunosForaDaListaTurma([101], [], cts.Token));
        }

        private void VerificarAvisosRegistrados(Times vezes)
        {
            _logger.VerifyLog<DadosAlunosService, Exception>(LogLevel.Warning, vezes);
        }

        [Fact]
        public async Task ForaDaListaTurma_DeveDeixarIdNulo_E_NaoCriarCatalogo_QuandoValorNaoExistir()
        {
            ConfigurarCatalogosRacaGenero();
            _alunoTurmaService.Setup(x => x.ObterRacaSexoAluno(101, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AlunoRacaSexoEolDto { Sexo = "X", GrupoEtnico = "INEXISTENTE" });

            var resultado = await CriarService().ObterDadosRacaGeneroAlunosForaDaListaTurma([101], []);

            var item = Assert.Single(resultado);
            Assert.Null(item.RacaId);
            Assert.Null(item.SexoId);
            VerificarAvisosRegistrados(Times.Exactly(2));
            _repositorioRacaCor.Verify(x => x.SalvarAsync(It.IsAny<RacaCor>(), It.IsAny<CancellationToken>()), Times.Never);
            _repositorioGeneroSexo.Verify(x => x.SalvarAsync(It.IsAny<GeneroSexo>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ForaDaListaTurma_DeveRetornarVazio_SemConsultas_QuandoNaoHouverAlunos()
        {
            var resultado = await CriarService().ObterDadosRacaGeneroAlunosForaDaListaTurma([], []);

            Assert.Empty(resultado);
            _repositorioRacaCor.Verify(x => x.ListarAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeveRetornarVazio_QuandoDadosAlunosNulo()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

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
                new() { CodigoAluno = 101, Raca = "PARDA", Sexo = string.Empty, NomeAluno = "Teste" }
                        });

            var resultado = (await CriarService().ObterDadosRacaGeneroAlunos(1)).ToList();

            var item = Assert.Single(resultado);
            Assert.Equal(string.Empty, item.Sexo);
        }

        [Fact]
        public async Task DeveRetornarAlunoUmaVez_QuandoEolRetornarAlunoDuplicadoNaTurma()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                    new() { CodigoAluno = 101, NumeroChamada = 1, Raca = "PARDA", Sexo = "F", NomeAluno = "Teste" },
                    new() { CodigoAluno = 101, NumeroChamada = 2, Raca = "PARDA", Sexo = "F", NomeAluno = "Teste" },
                    new() { CodigoAluno = 102, NumeroChamada = 3, Raca = "BRANCA", Sexo = "M", NomeAluno = "Outro" }
                });

            var resultado = (await CriarService().ObterDadosRacaGeneroAlunos(1)).ToList();

            Assert.Equal(2, resultado.Count);
            var aluno = Assert.Single(resultado, r => r.CodigoAluno == 101);
            Assert.Equal("Parda", aluno.Raca);
            Assert.Equal("Feminino", aluno.Sexo);
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
                new() { CodigoAluno = 101, Raca = string.Empty, Sexo = "M", NomeAluno = "Teste" }
                        });

            var resultado = (await CriarService().ObterDadosRacaGeneroAlunos(1)).ToList();

            var item = Assert.Single(resultado);
            Assert.Equal(string.Empty, item.Raca);
        }


        [Fact]
        public async Task DeveRetornarListaVazia_QuandoCodigoAlunosVazio()
        {
            var resultado = await CriarService().ObterDadosAlunosPorCodigoUe(new List<int>(), 2024);

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task DeveRetornarAlunos_QuandoRespostaHttpComSucesso()
        {
            var alunosEsperados = new List<AlunoEolDto>
            {
                new() { CodigoAluno = 123, NomeAluno = "João" }
            };
            var json = JsonConvert.SerializeObject(alunosEsperados);
            var httpClient = HttpClientMockHelper.Create(HttpStatusCode.OK,json);
            _httpClientFactory
                    .Setup(x => x.CreateClient(ServicoEolConstants.SERVICO))
                    .Returns(httpClient);

            var resultado = await CriarService().ObterDadosAlunosPorCodigoUe(new List<int> { 123 }, 2024);

            Assert.Single(resultado);
        }

        [Fact]
        public async Task DeveRetornarListaVazia_QuandoRespostaHttpForErro()
        {

            var httpClient = HttpClientMockHelper.Create(HttpStatusCode.NoContent);
            _httpClientFactory
                    .Setup(x => x.CreateClient(ServicoEolConstants.SERVICO))
                    .Returns(httpClient);

            var resultado = await CriarService().ObterDadosAlunosPorCodigoUe(new List<int> { 123 }, 2024);

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task DeveLancarOperationCanceled_QuandoCancelado()
        {
            var httpClient = HttpClientMockHelper.Create(HttpStatusCode.OK);
            _httpClientFactory
                    .Setup(x => x.CreateClient(ServicoEolConstants.SERVICO))
                    .Returns(httpClient);

            using (var cts = new CancellationTokenSource())
            {
                await cts.CancelAsync();

                await Assert.ThrowsAsync<OperationCanceledException>(() =>
                    CriarService().ObterDadosAlunosPorCodigoUe(new List<int> { 123   }, 2024, cts.Token));
            }
        }


        [Fact]
        public void DeveLancarArgumentNullException_QuandoHttpClientFactoryNulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DadosAlunosService(null!, _alunoTurmaService.Object, _repositorioGeneroSexo.Object, _repositorioRacaCor.Object, _logger.Object));
        }

        [Fact]
        public void DeveLancarArgumentNullException_QuandoAlunoTurmaServiceNulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DadosAlunosService(_httpClientFactory.Object, null!, _repositorioGeneroSexo.Object, _repositorioRacaCor.Object, _logger.Object));
        }

        [Fact]
        public async Task SincronizarRacaEGenero_DeveSalvarNovaRaca_QuandoRacaNaoExisteNoRepositorio()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                    new() { CodigoAluno = 101, Raca = "BRANCA", CodigoRaca = 1, Sexo = "M", NomeAluno = "Teste" }
                });
            _repositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
            _repositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

            await CriarService().ObterDadosRacaGeneroAlunos(1);

            _repositorioRacaCor.Verify(x => x.SalvarAsync(
                It.Is<RacaCor>(r => r.Descricao == "BRANCA" && r.CodigoEolRacaCor == 1 && r.CriadoPor == "Sistema" && !r.Excluido),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SincronizarRacaEGenero_NaoDeveSalvarRaca_QuandoTodasRacasJaExistem()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                    new() { CodigoAluno = 101, Raca = "BRANCA", Sexo = "M", NomeAluno = "Teste" }
                });
            _repositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RacaCor>
                {
                    new() { Id = 10, Descricao = "BRANCA", CodigoEolRacaCor = 1 }
                });
            _repositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GeneroSexo>
                {
                    new() { Id = 20, Descricao = "Masculino", Sigla = "M" }
                });

            await CriarService().ObterDadosRacaGeneroAlunos(1);

            _repositorioRacaCor.Verify(x => x.SalvarAsync(It.IsAny<RacaCor>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SincronizarRacaEGenero_DeveCompararRacaCaseInsensitive_NaoDuplicandoRegistro()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                    new() { CodigoAluno = 101, Raca = "branca", Sexo = "M", NomeAluno = "Teste" }
                });
            _repositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RacaCor>
                {
                    new() { Id = 10, Descricao = "BRANCA", CodigoEolRacaCor = 1 }
                });
            _repositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

            await CriarService().ObterDadosRacaGeneroAlunos(1);

            _repositorioRacaCor.Verify(x => x.SalvarAsync(It.IsAny<RacaCor>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SincronizarRacaEGenero_DeveSalvarRacaDistintaApenasUmaVez_QuandoMultiplosAlunosComMesmaRaca()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                    new() { CodigoAluno = 101, Raca = "PARDA", Sexo = "M", NomeAluno = "Teste1" },
                    new() { CodigoAluno = 102, Raca = "PARDA", Sexo = "F", NomeAluno = "Teste2" },
                    new() { CodigoAluno = 103, Raca = "PARDA", Sexo = "M", NomeAluno = "Teste3" }
                });
            _repositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
            _repositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

            await CriarService().ObterDadosRacaGeneroAlunos(1);

            _repositorioRacaCor.Verify(x => x.SalvarAsync(
                It.Is<RacaCor>(r => r.Descricao == "PARDA"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SincronizarRacaEGenero_NaoDeveSalvarRaca_QuandoDescricaoVaziaOuNula()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                    new() { CodigoAluno = 101, Raca = string.Empty, Sexo = "M", NomeAluno = "Teste1" },
                    new() { CodigoAluno = 102, Raca = null!, Sexo = "F", NomeAluno = "Teste2" }
                });
            _repositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
            _repositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

            await CriarService().ObterDadosRacaGeneroAlunos(1);

            _repositorioRacaCor.Verify(x => x.SalvarAsync(It.IsAny<RacaCor>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("BRANCA", 1)]
        [InlineData("PRETA", 3)]
        [InlineData("PARDA", 2)]
        [InlineData("AMARELA", 4)]
        [InlineData("INDIGENA", 5)]
        [InlineData("OUTRA", 99)]
        public async Task SincronizarRacaEGenero_DeveSalvarComCodigoEolDoDto(string descricaoRaca, int codigoRaca)
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                    new() { CodigoAluno = 101, Raca = descricaoRaca, CodigoRaca = codigoRaca, Sexo = "M", NomeAluno = "Teste" }
                });
            _repositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
            _repositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

            await CriarService().ObterDadosRacaGeneroAlunos(1);

            _repositorioRacaCor.Verify(x => x.SalvarAsync(
                It.Is<RacaCor>(r => r.Descricao == descricaoRaca && r.CodigoEolRacaCor == codigoRaca),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SincronizarRacaEGenero_DeveSalvarNovoGenero_QuandoSiglaNaoExisteNoRepositorio()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                    new() { CodigoAluno = 101, Raca = "PARDA", Sexo = "M", NomeAluno = "Teste" }
                });
            _repositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
            _repositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

            await CriarService().ObterDadosRacaGeneroAlunos(1);

            _repositorioGeneroSexo.Verify(x => x.SalvarAsync(
                It.Is<GeneroSexo>(g => g.Sigla == "M" && g.Descricao == "Masculino" && g.CriadoPor == "Sistema" && g.CriadoRF == "0" && !g.Excluido),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SincronizarRacaEGenero_NaoDeveSalvarGenero_QuandoSiglaJaExiste()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                    new() { CodigoAluno = 101, Raca = "PARDA", Sexo = "F", NomeAluno = "Teste" }
                });
            _repositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RacaCor>
                {
                    new() { Id = 10, Descricao = "PARDA", CodigoEolRacaCor = 2 }
                });
            _repositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GeneroSexo>
                {
                    new() { Id = 30, Descricao = "Feminino", Sigla = "F" }
                });

            await CriarService().ObterDadosRacaGeneroAlunos(1);

            _repositorioGeneroSexo.Verify(x => x.SalvarAsync(It.IsAny<GeneroSexo>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SincronizarRacaEGenero_DeveCompararSiglaCaseInsensitive_NaoDuplicandoGenero()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                    new() { CodigoAluno = 101, Raca = "PARDA", Sexo = "m", NomeAluno = "Teste" }
                });
            _repositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RacaCor>
                {
                    new() { Id = 10, Descricao = "PARDA", CodigoEolRacaCor = 2 }
                });
            _repositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GeneroSexo>
                {
                    new() { Id = 20, Descricao = "Masculino", Sigla = "M" }
                });

            await CriarService().ObterDadosRacaGeneroAlunos(1);

            _repositorioGeneroSexo.Verify(x => x.SalvarAsync(It.IsAny<GeneroSexo>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SincronizarRacaEGenero_DeveSalvarGeneroDistintoApenasUmaVez_QuandoMultiplosAlunosComMesmaSigla()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                    new() { CodigoAluno = 101, Raca = "PARDA", Sexo = "F", NomeAluno = "Teste1" },
                    new() { CodigoAluno = 102, Raca = "PARDA", Sexo = "F", NomeAluno = "Teste2" }
                });
            _repositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RacaCor>
                {
                    new() { Id = 10, Descricao = "PARDA", CodigoEolRacaCor = 2 }
                });
            _repositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

            await CriarService().ObterDadosRacaGeneroAlunos(1);

            _repositorioGeneroSexo.Verify(x => x.SalvarAsync(
                It.Is<GeneroSexo>(g => g.Sigla == "F"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SincronizarRacaEGenero_NaoDeveSalvarGenero_QuandoSiglaVaziaOuNula()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                    new() { CodigoAluno = 101, Raca = "PARDA", Sexo = string.Empty, NomeAluno = "Teste1" },
                    new() { CodigoAluno = 102, Raca = "PARDA", Sexo = null!, NomeAluno = "Teste2" }
                });
            _repositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
            _repositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

            await CriarService().ObterDadosRacaGeneroAlunos(1);

            _repositorioGeneroSexo.Verify(x => x.SalvarAsync(It.IsAny<GeneroSexo>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SincronizarRacaEGenero_DeveAtribuirIdsAposSalvarNovosRegistros()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                    new() { CodigoAluno = 101, Raca = "PARDA", Sexo = "M", NomeAluno = "Teste" }
                });
            _repositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
            _repositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
            _repositorioRacaCor
                .Setup(x => x.SalvarAsync(It.IsAny<RacaCor>(), It.IsAny<CancellationToken>()))
                .Callback<RacaCor, CancellationToken>((r, _) => r.Id = 77)
                .ReturnsAsync(77);
            _repositorioGeneroSexo
                .Setup(x => x.SalvarAsync(It.IsAny<GeneroSexo>(), It.IsAny<CancellationToken>()))
                .Callback<GeneroSexo, CancellationToken>((g, _) => g.Id = 88)
                .ReturnsAsync(88);

            var resultado = (await CriarService().ObterDadosRacaGeneroAlunos(1)).ToList();

            var item = Assert.Single(resultado);
            Assert.Equal(77, item.RacaId);
            Assert.Equal(88, item.SexoId);
        }

        [Fact]
        public async Task SincronizarRacaEGenero_DeveTratarExcecao_QuandoSalvarRacaFalhar()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                    new() { CodigoAluno = 101, Raca = "BRANCA", Sexo = "M", NomeAluno = "Teste" }
                });
            _repositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
            _repositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
            _repositorioRacaCor
                .Setup(x => x.SalvarAsync(It.IsAny<RacaCor>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Falha ao salvar"));

            var resultado = await CriarService().ObterDadosRacaGeneroAlunos(1);

            Assert.Single(resultado);
            _logger.VerifyLog<DadosAlunosService, InvalidOperationException>(LogLevel.Error, Times.Once());
        }

        [Fact]
        public async Task SincronizarRacaEGenero_DeveTratarExcecao_QuandoSalvarGeneroFalhar()
        {
            _alunoTurmaService
                .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosAlunoPorTurmaDto>
                {
                    new() { CodigoAluno = 101, Raca = "BRANCA", Sexo = "M", NomeAluno = "Teste" }
                });
            _repositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RacaCor>
                {
                    new() { Id = 10, Descricao = "BRANCA", CodigoEolRacaCor = 1 }
                });
            _repositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
            _repositorioGeneroSexo
                .Setup(x => x.SalvarAsync(It.IsAny<GeneroSexo>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Falha ao salvar"));

            var resultado = await CriarService().ObterDadosRacaGeneroAlunos(1);

            Assert.Single(resultado);
            _logger.VerifyLog<DadosAlunosService, InvalidOperationException>(LogLevel.Error, Times.Once());
        }
    }
}