using Moq;
using SME.Sondagem.Aplicacao.UseCases.GeneroSexo;
using SME.Sondagem.Dados.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.GeneroSexo
{
    public class ObterListaGeneroSexoUseCaseTeste
    {
        private readonly Mock<IRepositorioGeneroSexo> _repositorioMock;
        private readonly ObterListaGeneroSexoUseCase _useCase;

        public ObterListaGeneroSexoUseCaseTeste()
        {
            _repositorioMock = new Mock<IRepositorioGeneroSexo>();
            _useCase = new ObterListaGeneroSexoUseCase(_repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Quando_Existem_GenerosSexos_Nao_Excluidos()
        {
            var generosSexos = new List<Dominio.Entidades.GeneroSexo>
        {
            new() { Id = 1, Descricao = "Masculino",  Sigla = "M", Excluido = false },
            new() { Id = 2, Descricao = "Feminino",   Sigla = "F", Excluido = false }
        };

            _repositorioMock
                .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(generosSexos);

            var resultado = await _useCase.Executar();

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            _repositorioMock.Verify(x => x.ListarAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Vazia_Quando_Nao_Existem_GenerosSexos()
        {
            _repositorioMock
                .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Dominio.Entidades.GeneroSexo>());

            var resultado = await _useCase.Executar();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
            _repositorioMock.Verify(x => x.ListarAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Filtrar_GenerosSexos_Excluidos()
        {
            var generosSexos = new List<Dominio.Entidades.GeneroSexo>
        {
            new() { Id = 1, Descricao = "Masculino", Sigla = "M", Excluido = false },
            new() { Id = 2, Descricao = "Excluido",  Sigla = "X", Excluido = true }
        };

            _repositorioMock
                .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(generosSexos);

            var resultado = await _useCase.Executar();

            Assert.Single(resultado);
            Assert.Equal("Masculino", resultado.First().Descricao);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Vazia_Quando_Todos_Os_GenerosSexos_Estao_Excluidos()
        {
            var generosSexos = new List<Dominio.Entidades.GeneroSexo>
        {
            new() { Id = 1, Descricao = "Masculino", Sigla = "M", Excluido = true },
            new() { Id = 2, Descricao = "Feminino",  Sigla = "F", Excluido = true }
        };

            _repositorioMock
                .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(generosSexos);

            var resultado = await _useCase.Executar();

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task Deve_Mapear_Corretamente_Id_E_Descricao_Para_ItemMenuDto()
        {
            var generosSexos = new List<Dominio.Entidades.GeneroSexo>
        {
            new() { Id = 3, Descricao = "Não Binário", Sigla = "NB", Excluido = false }
        };

            _repositorioMock
                .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(generosSexos);

            var resultado = await _useCase.Executar();

            var item = resultado.First();
            Assert.Equal(3, item.Id);
            Assert.Equal("Não Binário", item.Descricao);
        }

        [Fact]
        public async Task Deve_Passar_CancellationToken_Para_Repositorio()
        {
            var cancellationToken = new CancellationToken();

            _repositorioMock
                .Setup(x => x.ListarAsync(cancellationToken))
                .ReturnsAsync(new List<Dominio.Entidades.GeneroSexo>());

            await _useCase.Executar(cancellationToken);

            _repositorioMock.Verify(x => x.ListarAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public void Deve_Lancar_ArgumentNullException_Quando_Repositorio_For_Nulo()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ObterListaGeneroSexoUseCase(null!));

            Assert.Equal("repositorioGeneroSexo", exception.ParamName);
        }
    }
}
