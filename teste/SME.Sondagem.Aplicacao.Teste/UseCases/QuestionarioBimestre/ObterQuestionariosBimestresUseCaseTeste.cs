using Moq;
using SME.Sondagem.Aplicacao.UseCases.QuestionarioBimestre;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.QuestionarioBimestre
{
    public class ObterQuestionariosBimestresUseCaseTeste
    {
        private readonly Mock<IRepositorioQuestionarioBimestre> _repositorioMock;
        private readonly ObterQuestionariosBimestresUseCase _useCase;

        public ObterQuestionariosBimestresUseCaseTeste()
        {
            _repositorioMock = new Mock<IRepositorioQuestionarioBimestre>();
            _useCase = new ObterQuestionariosBimestresUseCase(_repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_De_Vinculos()
        {
            var vinculos = new List<Dominio.Entidades.Questionario.QuestionarioBimestre>
            {
                new Dominio.Entidades.Questionario.QuestionarioBimestre(1, 1),
                new Dominio.Entidades.Questionario.QuestionarioBimestre(1, 2)
            };

            _repositorioMock
                .Setup(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(vinculos);

            var resultado = (await _useCase.ExecutarAsync()).ToList();

            Assert.Equal(2, resultado.Count);
            Assert.Equal(1, resultado[0].QuestionarioId);
            Assert.Equal(1, resultado[0].BimestreId);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Vazia_Quando_Nao_Houver_Vinculos()
        {
            _repositorioMock
                .Setup(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Dominio.Entidades.Questionario.QuestionarioBimestre>());

            var resultado = (await _useCase.ExecutarAsync()).ToList();

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task Deve_Mapear_Propriedades_De_Auditoria()
        {
            var dataAtual = DateTime.UtcNow;
            var vinculo = new Dominio.Entidades.Questionario.QuestionarioBimestre(1, 1)
            {
                Id = 10,
                CriadoEm = dataAtual,
                CriadoPor = "Usuario Teste",
                CriadoRF = "123456",
                AlteradoEm = dataAtual.AddDays(1),
                AlteradoPor = "Usuario Alteracao",
                AlteradoRF = "654321"
            };

            _repositorioMock
                .Setup(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Dominio.Entidades.Questionario.QuestionarioBimestre> { vinculo });

            var resultado = (await _useCase.ExecutarAsync()).First();

            Assert.Equal(10, resultado.Id);
            Assert.Equal(dataAtual, resultado.CriadoEm);
            Assert.Equal("Usuario Teste", resultado.CriadoPor);
            Assert.Equal("123456", resultado.CriadoRF);
            Assert.Equal(dataAtual.AddDays(1), resultado.AlteradoEm);
            Assert.Equal("Usuario Alteracao", resultado.AlteradoPor);
            Assert.Equal("654321", resultado.AlteradoRF);
        }

        [Fact]
        public async Task Deve_Passar_CancellationToken_Para_Repositorio()
        {
            var cancellationToken = new CancellationToken();

            _repositorioMock
                .Setup(x => x.ObterTodosAsync(cancellationToken))
                .ReturnsAsync(new List<Dominio.Entidades.Questionario.QuestionarioBimestre>());

            await _useCase.ExecutarAsync(cancellationToken);

            _repositorioMock.Verify(x => x.ObterTodosAsync(cancellationToken), Times.Once);
        }
    }
}