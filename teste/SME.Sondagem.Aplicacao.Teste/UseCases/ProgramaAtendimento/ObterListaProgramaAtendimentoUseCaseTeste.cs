using Moq;
using SME.Sondagem.Aplicacao.UseCases.ProgramaAtendimento;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.ProgramaAtendimento
{
    public class ObterListaProgramaAtendimentoUseCaseTeste
    {
        private readonly Mock<IRepositorioProgramaAtendimento> _repositorioMock;
        private readonly ObterListaProgramaAtendimentoUseCase _useCase;

        public ObterListaProgramaAtendimentoUseCaseTeste()
        {
            _repositorioMock = new Mock<IRepositorioProgramaAtendimento>();
            _useCase = new ObterListaProgramaAtendimentoUseCase(_repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Quando_Existem_Programas_Nao_Excluidos()
        {
            var programas = new List<Dominio.Entidades.ProgramaAtendimento>
            {
                new() { Id = 1, Descricao = "Programa A", Excluido = false },
                new() { Id = 2, Descricao = "Programa B", Excluido = false }
            };

            _repositorioMock
                .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(programas);

            var resultado = await _useCase.Executar();

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            _repositorioMock.Verify(x => x.ListarAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Vazia_Quando_Nao_Existem_Programas()
        {
            _repositorioMock
                .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Dominio.Entidades.ProgramaAtendimento>());

            var resultado = await _useCase.Executar();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
            _repositorioMock.Verify(x => x.ListarAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Filtrar_Programas_Excluidos()
        {
            var programas = new List<Dominio.Entidades.ProgramaAtendimento>
            {
                new() { Id = 1, Descricao = "Programa Ativo",   Excluido = false },
                new() { Id = 2, Descricao = "Programa Excluido", Excluido = true }
            };

            _repositorioMock
                .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(programas);

            var resultado = await _useCase.Executar();

            Assert.Single(resultado);
            Assert.Equal("Programa Ativo", resultado.First().Descricao);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Vazia_Quando_Todos_Os_Programas_Estao_Excluidos()
        {
            var programas = new List<Dominio.Entidades.ProgramaAtendimento>
            {
                new() { Id = 1, Descricao = "Programa Excluido 1", Excluido = true },
                new() { Id = 2, Descricao = "Programa Excluido 2", Excluido = true }
            };

            _repositorioMock
                .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(programas);

            var resultado = await _useCase.Executar();

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task Deve_Mapear_Corretamente_Id_E_Descricao_Para_ItemMenuDto()
        {
            var programas = new List<Dominio.Entidades.ProgramaAtendimento>
            {
                new() { Id = 10, Descricao = "Programa Mapeado", Excluido = false }
            };

            _repositorioMock
                .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(programas);

            var resultado = await _useCase.Executar();

            var item = resultado.First();
            Assert.Equal(10, item.Id);
            Assert.Equal("Programa Mapeado", item.Descricao);
        }

        [Fact]
        public async Task Deve_Passar_CancellationToken_Para_Repositorio()
        {
            var cancellationToken = new CancellationToken();

            _repositorioMock
                .Setup(x => x.ListarAsync(cancellationToken))
                .ReturnsAsync(new List<Dominio.Entidades.ProgramaAtendimento>());

            await _useCase.Executar(cancellationToken);

            _repositorioMock.Verify(x => x.ListarAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public void Deve_Lancar_ArgumentNullException_Quando_Repositorio_For_Nulo()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ObterListaProgramaAtendimentoUseCase(null!));

            Assert.Equal("repositorioProgramaAtendimento", exception.ParamName);
        }

    }
}
