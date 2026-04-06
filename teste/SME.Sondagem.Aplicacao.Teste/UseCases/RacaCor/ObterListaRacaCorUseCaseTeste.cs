using Moq;
using SME.Sondagem.Aplicacao.UseCases.RacaCor;
using SME.Sondagem.Dados.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.RacaCor
{
    public class ObterListaRacaCorUseCaseTeste
    {
        private readonly Mock<IRepositorioRacaCor> _repositorioMock;
        private readonly ObterListaRacaCorUseCase _useCase;

        public ObterListaRacaCorUseCaseTeste()
        {
            _repositorioMock = new Mock<IRepositorioRacaCor>();
            _useCase = new ObterListaRacaCorUseCase(_repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Quando_Existem_RacasCores_Nao_Excluidas()
        {
            var racasCores = new List<Dominio.Entidades.RacaCor>
        {
            new() { Id = 1, Descricao = "Branca", CodigoEolRacaCor = 1, Excluido = false },
            new() { Id = 2, Descricao = "Parda",  CodigoEolRacaCor = 2, Excluido = false },
            new() { Id = 3, Descricao = "Preta",  CodigoEolRacaCor = 3, Excluido = false }
        };

            _repositorioMock
                .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(racasCores);

            var resultado = await _useCase.Executar();

            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.Count());
            _repositorioMock.Verify(x => x.ListarAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Vazia_Quando_Nao_Existem_RacasCores()
        {
            _repositorioMock
                .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Dominio.Entidades.RacaCor>());

            var resultado = await _useCase.Executar();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
            _repositorioMock.Verify(x => x.ListarAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Filtrar_RacasCores_Excluidas()
        {
            var racasCores = new List<Dominio.Entidades.RacaCor>
        {
            new() { Id = 1, Descricao = "Branca",  CodigoEolRacaCor = 1, Excluido = false },
            new() { Id = 2, Descricao = "Excluida", CodigoEolRacaCor = 2, Excluido = true }
        };

            _repositorioMock
                .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(racasCores);

            var resultado = await _useCase.Executar();

            Assert.Single(resultado);
            Assert.Equal("Branca", resultado.First().Descricao);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Vazia_Quando_Todas_As_RacasCores_Estao_Excluidas()
        {
            var racasCores = new List<Dominio.Entidades.RacaCor>
        {
            new() { Id = 1, Descricao = "Branca", CodigoEolRacaCor = 1, Excluido = true },
            new() { Id = 2, Descricao = "Parda",  CodigoEolRacaCor = 2, Excluido = true }
        };

            _repositorioMock
                .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(racasCores);

            var resultado = await _useCase.Executar();

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task Deve_Mapear_Corretamente_Id_E_Descricao_Para_ItemMenuDto()
        {
            var racasCores = new List<Dominio.Entidades.RacaCor>
        {
            new() { Id = 5, Descricao = "Amarela", CodigoEolRacaCor = 4, Excluido = false }
        };

            _repositorioMock
                .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(racasCores);

            var resultado = await _useCase.Executar();

            var item = resultado.First();
            Assert.Equal(5, item.Id);
            Assert.Equal("Amarela", item.Descricao);
        }

        [Fact]
        public async Task Deve_Passar_CancellationToken_Para_Repositorio()
        {
            var cancellationToken = new CancellationToken();

            _repositorioMock
                .Setup(x => x.ListarAsync(cancellationToken))
                .ReturnsAsync(new List<Dominio.Entidades.RacaCor>());

            await _useCase.Executar(cancellationToken);

            _repositorioMock.Verify(x => x.ListarAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public void Deve_Lancar_ArgumentNullException_Quando_Repositorio_For_Nulo()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ObterListaRacaCorUseCase(null!));

            Assert.Equal("repositorioRacaCor", exception.ParamName);
        }
    }
}
