using Moq;
using SME.Sondagem.Aplicacao.UseCases.ParametroSondagemQuestionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.ParametroSondagemQuestionario
{
    public class ObterParametroSondagemQuestionarioPorIdQuestionarioUseCaseTeste
    {
        private readonly Mock<IRepositorioParametroSondagemQuestionario> repositorioMock;
        private readonly Mock<IRepositorioCache> repositorioCacheMock;
        private readonly ObterParametroSondagemQuestionarioPorIdQuestionarioUseCase useCase;

        public ObterParametroSondagemQuestionarioPorIdQuestionarioUseCaseTeste()
        {
            repositorioMock = new Mock<IRepositorioParametroSondagemQuestionario>();
            repositorioCacheMock = new Mock<IRepositorioCache>();
            useCase = new ObterParametroSondagemQuestionarioPorIdQuestionarioUseCase(
                repositorioMock.Object,
                repositorioCacheMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_parametros_do_questionario_quando_existirem()
        {
            // Arrange
            const long idQuestionario = 10;
            var chaveCache = $"parametros-sondagem-questionario-{idQuestionario}";

            var parametrosEntidade = new List<Dominio.Entidades.ParametroSondagemQuestionario>
            {
                new()
                {
                    Id = 1,
                    IdQuestionario = (int)idQuestionario,
                    IdParametroSondagem = (int)Dominio.Enums.TipoParametroSondagem.ExibirTituloTabelaSondagem,
                    Valor = "A",
                    ParametroSondagem = new Dominio.Entidades.ParametroSondagem
                    {
                        Tipo = Dominio.Enums.TipoParametroSondagem.ExibirTituloTabelaSondagem
                    }
                },
                new()
                {
                    Id = 2,
                    IdQuestionario = (int)idQuestionario,
                    IdParametroSondagem = (int)Dominio.Enums.TipoParametroSondagem.ExibirDescricaoOpcaoResposta,
                    Valor = "B",
                    ParametroSondagem = new Dominio.Entidades.ParametroSondagem
                    {
                        Tipo = Dominio.Enums.TipoParametroSondagem.ExibirDescricaoOpcaoResposta
                    }
                }
            };

            var parametrosDto = parametrosEntidade.Select(p => new SME.Sondagem.Infrastructure.Dtos.Questionario.ParametroSondagemQuestionarioCompletoDto
            {
                Id = p.Id,
                IdQuestionario = p.IdQuestionario,
                Valor = p.Valor,
                Tipo = Enum.GetName(typeof(Dominio.Enums.TipoParametroSondagem), p.IdParametroSondagem)
            });

            repositorioMock
                .Setup(r => r.ObterPorIdQuestionarioAsync(idQuestionario, It.IsAny<CancellationToken>()))
                .ReturnsAsync(parametrosEntidade);

            repositorioCacheMock
                .Setup(r => r.ObterRedisAsync(chaveCache, It.IsAny<Func<Task<IEnumerable<SME.Sondagem.Infrastructure.Dtos.Questionario.ParametroSondagemQuestionarioCompletoDto>>>>(), It.IsAny<int>()))
                .ReturnsAsync(parametrosDto);

            // Act
            var resultado = await useCase.ExecutarAsync(idQuestionario);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());

            var primeiro = resultado.First();
            Assert.Equal(1, primeiro.Id);
            Assert.Equal((int)idQuestionario, primeiro.IdQuestionario);
            Assert.Equal("A", primeiro.Valor);
            Assert.Equal(
                Enum.GetName(typeof(Dominio.Enums.TipoParametroSondagem), (int)Dominio.Enums.TipoParametroSondagem.ExibirTituloTabelaSondagem),
                primeiro.Tipo
            );

            repositorioCacheMock.Verify(
                r => r.ObterRedisAsync(chaveCache, It.IsAny<Func<Task<IEnumerable<SME.Sondagem.Infrastructure.Dtos.Questionario.ParametroSondagemQuestionarioCompletoDto>>>>(), It.IsAny<int>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Deve_retornar_lista_vazia_quando_nao_existirem_parametros_para_o_questionario()
        {
            // Arrange
            const long idQuestionario = 99;
            var chaveCache = $"parametros-sondagem-questionario-{idQuestionario}";

            var parametrosVazio = new List<SME.Sondagem.Infrastructure.Dtos.Questionario.ParametroSondagemQuestionarioCompletoDto>();

            repositorioMock
                .Setup(r => r.ObterPorIdQuestionarioAsync(idQuestionario, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Dominio.Entidades.ParametroSondagemQuestionario>());

            repositorioCacheMock
                .Setup(r => r.ObterRedisAsync(chaveCache, It.IsAny<Func<Task<IEnumerable<SME.Sondagem.Infrastructure.Dtos.Questionario.ParametroSondagemQuestionarioCompletoDto>>>>(), It.IsAny<int>()))
                .ReturnsAsync(parametrosVazio);

            // Act
            var resultado = await useCase.ExecutarAsync(idQuestionario);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            repositorioCacheMock.Verify(
                r => r.ObterRedisAsync(chaveCache, It.IsAny<Func<Task<IEnumerable<SME.Sondagem.Infrastructure.Dtos.Questionario.ParametroSondagemQuestionarioCompletoDto>>>>(), It.IsAny<int>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Deve_buscar_dados_do_repositorio_quando_cache_estiver_vazio()
        {
            // Arrange
            const long idQuestionario = 2;
            var chaveCache = $"parametros-sondagem-questionario-{idQuestionario}";

            var parametrosEntidade = new List<Dominio.Entidades.ParametroSondagemQuestionario>
            {
                new()
                {
                    Id = 3,
                    IdQuestionario = (int)idQuestionario,
                    IdParametroSondagem = (int)Dominio.Enums.TipoParametroSondagem.ExibirTituloTabelaSondagem,
                    Valor = "C",
                    ParametroSondagem = new Dominio.Entidades.ParametroSondagem
                    {
                        Tipo = Dominio.Enums.TipoParametroSondagem.ExibirTituloTabelaSondagem
                    }
                }
            };

            repositorioMock
                .Setup(r => r.ObterPorIdQuestionarioAsync(idQuestionario, It.IsAny<CancellationToken>()))
                .ReturnsAsync(parametrosEntidade);

            // Setup para simular execução da função quando cache está vazio
            repositorioCacheMock
                .Setup(r => r.ObterRedisAsync(chaveCache, It.IsAny<Func<Task<IEnumerable<SME.Sondagem.Infrastructure.Dtos.Questionario.ParametroSondagemQuestionarioCompletoDto>>>>(), It.IsAny<int>()))
                .Returns<string, Func<Task<IEnumerable<SME.Sondagem.Infrastructure.Dtos.Questionario.ParametroSondagemQuestionarioCompletoDto>>>, int>(async (key, func, expiry) => await func());

            // Act
            var resultado = await useCase.ExecutarAsync(idQuestionario);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);

            var parametro = resultado.First();
            Assert.Equal(3, parametro.Id);
            Assert.Equal((int)idQuestionario, parametro.IdQuestionario);
            Assert.Equal("C", parametro.Valor);
            Assert.Equal(Enum.GetName(typeof(Dominio.Enums.TipoParametroSondagem), (int)Dominio.Enums.TipoParametroSondagem.ExibirTituloTabelaSondagem), parametro.Tipo);

            repositorioMock.Verify(
                r => r.ObterPorIdQuestionarioAsync(idQuestionario, It.IsAny<CancellationToken>()),
                Times.Once
            );

            repositorioCacheMock.Verify(
                r => r.ObterRedisAsync(chaveCache, It.IsAny<Func<Task<IEnumerable<SME.Sondagem.Infrastructure.Dtos.Questionario.ParametroSondagemQuestionarioCompletoDto>>>>(), It.IsAny<int>()),
                Times.Once
            );
        }
    }
}