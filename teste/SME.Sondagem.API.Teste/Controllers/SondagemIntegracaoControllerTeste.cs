using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers.Integracao;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;
using Xunit;

namespace SME.Sondagem.API.Teste.Controller
{
    public class SondagemIntegracaoControllerTeste
    {
        private readonly Mock<IObterSondagensUseCase> _obterSondagensUseCaseMock;
        private readonly Mock<IObterSondagemPorIdUseCase> _obterSondagemPorIdUseCaseMock;
        private readonly Mock<ICriarSondagemUseCase> _criarSondagemUseCaseMock;
        private readonly Mock<IAtualizarSondagemUseCase> _atualizarSondagemUseCaseMock;
        private readonly Mock<IExcluirSondagemUseCase> _excluirSondagemUseCaseMock;
        private readonly SondagemIntegracaoController _controller;

        public SondagemIntegracaoControllerTeste()
        {
            _obterSondagensUseCaseMock = new Mock<IObterSondagensUseCase>();
            _obterSondagemPorIdUseCaseMock = new Mock<IObterSondagemPorIdUseCase>();
            _criarSondagemUseCaseMock = new Mock<ICriarSondagemUseCase>();
            _atualizarSondagemUseCaseMock = new Mock<IAtualizarSondagemUseCase>();
            _excluirSondagemUseCaseMock = new Mock<IExcluirSondagemUseCase>();
            _excluirSondagemUseCaseMock = new Mock<IExcluirSondagemUseCase>();

            _controller = new SondagemIntegracaoController(
                _obterSondagensUseCaseMock.Object,
                _obterSondagemPorIdUseCaseMock.Object,
                _criarSondagemUseCaseMock.Object,
                _atualizarSondagemUseCaseMock.Object,
                _excluirSondagemUseCaseMock.Object
            );
        }

        #region Get (ObterSondagens)

        [Fact]
        public async Task Get_DeveRetornarOkComListaDeSondagens()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var sondagensEsperadas = new List<SondagemDto>
    {
        new SondagemDto
        {
            Id = 1,
            Descricao = "Sondagem de Matemática 2025",
            DataAplicacao = new DateTime(2025, 3, 15),
            CriadoEm = DateTime.Now.AddDays(-10),
            CriadoPor = "Sistema",
            CriadoRF = "1234567"
        },
        new SondagemDto
        {
            Id = 2,
            Descricao = "Sondagem de Português 2025",
            DataAplicacao = new DateTime(2025, 4, 20),
            CriadoEm = DateTime.Now.AddDays(-5),
            CriadoPor = "Sistema",
            CriadoRF = "7654321"
        }
    };

            _obterSondagensUseCaseMock
                .Setup(x => x.ExecutarAsync(cancellationToken))
                .ReturnsAsync(sondagensEsperadas);

            // Act
            var result = await _controller.Get(cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsAssignableFrom<IEnumerable<SondagemDto>>(okResult.Value); // MUDANÇA AQUI
            Assert.Equal(2, retorno.Count());

            var primeiraSondagem = retorno.First();
            Assert.Equal(1, primeiraSondagem.Id);
            Assert.Equal("Sondagem de Matemática 2025", primeiraSondagem.Descricao);
            Assert.Equal(new DateTime(2025, 3, 15), primeiraSondagem.DataAplicacao);

            _obterSondagensUseCaseMock.Verify(
                x => x.ExecutarAsync(cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Get_DeveRetornarOkComListaVazia_QuandoNaoHouverSondagens()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var sondagensEsperadas = new List<SondagemDto>();

            _obterSondagensUseCaseMock
                .Setup(x => x.ExecutarAsync(cancellationToken))
                .ReturnsAsync(sondagensEsperadas);

            // Act
            var result = await _controller.Get(cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsAssignableFrom<IEnumerable<SondagemDto>>(okResult.Value); // MUDANÇA AQUI
            Assert.Empty(retorno);

            _obterSondagensUseCaseMock.Verify(
                x => x.ExecutarAsync(cancellationToken),
                Times.Once);
        }


        #endregion

        #region GetById (ObterSondagemPorId)

        [Fact]
        public async Task GetById_DeveRetornarOkComSondagem_QuandoSondagemExistir()
        {
            // Arrange
            var sondagemId = 1;
            var cancellationToken = CancellationToken.None;
            var sondagemEsperada = new SondagemDto
            {
                Id = sondagemId,
                Descricao = "Sondagem de Matemática 2025",
                DataAplicacao = new DateTime(2025, 3, 15),
                CriadoEm = DateTime.Now.AddDays(-10),
                CriadoPor = "Sistema",
                CriadoRF = "1234567",
                AlteradoEm = DateTime.Now.AddDays(-2),
                AlteradoPor = "Admin",
                AlteradoRF = "9876543"
            };

            _obterSondagemPorIdUseCaseMock
                .Setup(x => x.ExecutarAsync(sondagemId, cancellationToken))
                .ReturnsAsync(sondagemEsperada);

            // Act
            var result = await _controller.GetById(sondagemId, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsType<SondagemDto>(okResult.Value);
            Assert.Equal(sondagemId, retorno.Id);
            Assert.Equal("Sondagem de Matemática 2025", retorno.Descricao);
            Assert.Equal(new DateTime(2025, 3, 15), retorno.DataAplicacao);
            Assert.Equal("Sistema", retorno.CriadoPor);
            Assert.Equal("Admin", retorno.AlteradoPor);

            _obterSondagemPorIdUseCaseMock.Verify(
                x => x.ExecutarAsync(sondagemId, cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task GetById_DeveLancarErroNaoEncontradoException_QuandoSondagemNaoExistir()
        {
            // Arrange
            var sondagemId = 999L;
            var cancellationToken = CancellationToken.None;

            _obterSondagemPorIdUseCaseMock
                .Setup(x => x.ExecutarAsync(sondagemId, cancellationToken))
                .ReturnsAsync((SondagemDto?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ErroNaoEncontradoException>(
                () => _controller.GetById(sondagemId, cancellationToken)
            );

            Assert.Equal(
                string.Format(MensagemNegocioComuns.SONDAGEM_NAO_ENCONTRADA, sondagemId),
                exception.Message);

            _obterSondagemPorIdUseCaseMock.Verify(
                x => x.ExecutarAsync(sondagemId, cancellationToken),
                Times.Once);
        }

        #endregion

        #region Create (CriarSondagem)

        [Fact]
        public async Task Create_DeveRetornarCreatedComId_QuandoSondagemForCriada()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var sondagemDto = new SondagemDto
            {
                Descricao = "Nova Sondagem de Ciências",
                DataAplicacao = new DateTime(2025, 5, 10)
            };
            var idEsperado = 10L;

            _criarSondagemUseCaseMock
                .Setup(x => x.ExecutarAsync(sondagemDto, cancellationToken))
                .ReturnsAsync(idEsperado);

            // Act
            var result = await _controller.Create(sondagemDto, cancellationToken);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(idEsperado, createdResult.Value);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            Assert.Equal(idEsperado, createdResult.RouteValues?["id"]);

            _criarSondagemUseCaseMock.Verify(
                x => x.ExecutarAsync(sondagemDto, cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Create_DeveCriarSondagemComSucesso_QuandoDadosForemValidos()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var sondagemDto = new SondagemDto
            {
                Descricao = "Sondagem Completa",
                DataAplicacao = new DateTime(2025, 7, 1)
            };
            var idEsperado = 5L;

            _criarSondagemUseCaseMock
                .Setup(x => x.ExecutarAsync(It.IsAny<SondagemDto>(), cancellationToken))
                .ReturnsAsync(idEsperado);

            // Act
            var result = await _controller.Create(sondagemDto, cancellationToken);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(createdResult.Value);
            Assert.Equal(idEsperado, createdResult.Value);

            _criarSondagemUseCaseMock.Verify(
                x => x.ExecutarAsync(It.Is<SondagemDto>(s =>
                    s.Descricao == "Sondagem Completa" &&
                    s.DataAplicacao == new DateTime(2025, 7, 1)),
                    cancellationToken),
                Times.Once);
        }

        #endregion

        #region Atualizar (AtualizarSondagem)

        [Fact]
        public async Task Atualizar_DeveRetornarOkComSondagemAtualizada_QuandoSondagemExistir()
        {
            // Arrange
            var sondagemId = 1;
            var cancellationToken = CancellationToken.None;
            var sondagemDto = new SondagemDto
            {
                Descricao = "Sondagem Atualizada",
                DataAplicacao = new DateTime(2025, 6, 15)
            };
            var sondagemAtualizada = new SondagemDto
            {
                Id = sondagemId,
                Descricao = "Sondagem Atualizada",
                DataAplicacao = new DateTime(2025, 6, 15),
                CriadoEm = DateTime.Now.AddDays(-30),
                CriadoPor = "Sistema",
                CriadoRF = "1234567",
                AlteradoEm = DateTime.Now,
                AlteradoPor = "Admin",
                AlteradoRF = "9876543"
            };

            _atualizarSondagemUseCaseMock
                .Setup(x => x.ExecutarAsync(sondagemId, sondagemDto, cancellationToken))
                .ReturnsAsync(sondagemAtualizada);

            // Act
            var result = await _controller.Atualizar(sondagemId, sondagemDto, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsType<SondagemDto>(okResult.Value);
            Assert.Equal(sondagemId, retorno.Id);
            Assert.Equal("Sondagem Atualizada", retorno.Descricao);
            Assert.Equal(new DateTime(2025, 6, 15), retorno.DataAplicacao);
            Assert.NotNull(retorno.AlteradoEm);

            _atualizarSondagemUseCaseMock.Verify(
                x => x.ExecutarAsync(sondagemId, sondagemDto, cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Atualizar_DeveLancarErroNaoEncontradoException_QuandoSondagemNaoExistir()
        {
            // Arrange
            var sondagemId = 999;
            var cancellationToken = CancellationToken.None;
            var sondagemDto = new SondagemDto
            {
                Descricao = "Sondagem Atualizada",
                DataAplicacao = new DateTime(2025, 6, 15)
            };

            _atualizarSondagemUseCaseMock
                .Setup(x => x.ExecutarAsync(sondagemId, sondagemDto, cancellationToken))
                .ReturnsAsync((SondagemDto?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ErroNaoEncontradoException>(
                () => _controller.Atualizar(sondagemId, sondagemDto, cancellationToken)
            );

            Assert.Equal(
                string.Format(MensagemNegocioComuns.SONDAGEM_NAO_ENCONTRADA, sondagemId),
                exception.Message);

            _atualizarSondagemUseCaseMock.Verify(
                x => x.ExecutarAsync(sondagemId, sondagemDto, cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Atualizar_DeveAtualizarApenasCamposPermitidos()
        {
            // Arrange
            var sondagemId = 1;
            var cancellationToken = CancellationToken.None;
            var sondagemDto = new SondagemDto
            {
                Descricao = "Descrição Modificada",
                DataAplicacao = new DateTime(2025, 8, 20)
            };
            var sondagemAtualizada = new SondagemDto
            {
                Id = sondagemId,
                Descricao = "Descrição Modificada",
                DataAplicacao = new DateTime(2025, 8, 20),
                CriadoEm = DateTime.Now.AddDays(-60),
                CriadoPor = "Sistema Original",
                CriadoRF = "1111111",
                AlteradoEm = DateTime.Now,
                AlteradoPor = "Usuario Teste",
                AlteradoRF = "2222222"
            };

            _atualizarSondagemUseCaseMock
                .Setup(x => x.ExecutarAsync(sondagemId, sondagemDto, cancellationToken))
                .ReturnsAsync(sondagemAtualizada);

            // Act
            var result = await _controller.Atualizar(sondagemId, sondagemDto, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsType<SondagemDto>(okResult.Value);
            Assert.Equal("Descrição Modificada", retorno.Descricao);
            Assert.Equal(new DateTime(2025, 8, 20), retorno.DataAplicacao);
            Assert.Equal("Sistema Original", retorno.CriadoPor);
            Assert.Equal("Usuario Teste", retorno.AlteradoPor);

            _atualizarSondagemUseCaseMock.Verify(
                x => x.ExecutarAsync(sondagemId, sondagemDto, cancellationToken),
                Times.Once);
        }

        #endregion

        #region Excluir (ExcluirSondagem)

        [Fact]
        public async Task Excluir_DeveRetornarNoContent_QuandoSondagemForExcluida()
        {
            // Arrange
            var sondagemId = 1;
            var cancellationToken = CancellationToken.None;

            _excluirSondagemUseCaseMock
                .Setup(x => x.ExecutarAsync(sondagemId, cancellationToken))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Excluir(sondagemId, cancellationToken);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _excluirSondagemUseCaseMock.Verify(
                x => x.ExecutarAsync(sondagemId, cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Excluir_DeveLancarErroNaoEncontradoException_QuandoSondagemNaoExistir()
        {
            // Arrange
            var sondagemId = 999;
            var cancellationToken = CancellationToken.None;

            _excluirSondagemUseCaseMock
                .Setup(x => x.ExecutarAsync(sondagemId, cancellationToken))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ErroNaoEncontradoException>(
                () => _controller.Excluir(sondagemId, cancellationToken)
            );

            Assert.Equal(
                string.Format(MensagemNegocioComuns.SONDAGEM_NAO_ENCONTRADA, sondagemId),
                exception.Message);

            _excluirSondagemUseCaseMock.Verify(
                x => x.ExecutarAsync(sondagemId, cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task Excluir_DeveExcluirSondagemComSucesso()
        {
            // Arrange
            var sondagemId = 5;
            var cancellationToken = CancellationToken.None;

            _excluirSondagemUseCaseMock
                .Setup(x => x.ExecutarAsync(sondagemId, cancellationToken))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Excluir(sondagemId, cancellationToken);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);

            _excluirSondagemUseCaseMock.Verify(
                x => x.ExecutarAsync(sondagemId, cancellationToken),
                Times.Once);
        }

        #endregion
    }
}
