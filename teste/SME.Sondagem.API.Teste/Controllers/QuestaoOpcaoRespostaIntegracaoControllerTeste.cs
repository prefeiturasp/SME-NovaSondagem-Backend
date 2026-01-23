using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers.Integracao;
using SME.Sondagem.Aplicacao.Interfaces.QuestaoOpcaoResposta;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.API.Teste.Controllers
{
    public class QuestaoOpcaoRespostaIntegracaoControllerTeste
    {
        private readonly Mock<IObterQuestaoOpcaoRespostaUseCase> _obterMock;
        private readonly Mock<IObterQuestaoOpcaoRespostaPorIdUseCase> _obterPorIdMock;
        private readonly Mock<ICriarQuestaoOpcaoRespostaUseCase> _criarMock;
        private readonly Mock<IAtualizarQuestaoOpcaoRespostaUseCase> _atualizarMock;
        private readonly Mock<IExcluirQuestaoOpcaoRespostaUseCase> _excluirMock;
        private readonly QuestaoOpcaoRespostaIntegracaoController _controller;

        public QuestaoOpcaoRespostaIntegracaoControllerTeste()
        {
            _obterMock = new Mock<IObterQuestaoOpcaoRespostaUseCase>();
            _obterPorIdMock = new Mock<IObterQuestaoOpcaoRespostaPorIdUseCase>();
            _criarMock = new Mock<ICriarQuestaoOpcaoRespostaUseCase>();
            _atualizarMock = new Mock<IAtualizarQuestaoOpcaoRespostaUseCase>();
            _excluirMock = new Mock<IExcluirQuestaoOpcaoRespostaUseCase>();

            _controller = new QuestaoOpcaoRespostaIntegracaoController(
                _obterMock.Object,
                _obterPorIdMock.Object,
                _criarMock.Object,
                _atualizarMock.Object,
                _excluirMock.Object);
        }

        #region GET - Obter Todas

        [Fact]
        public async Task Get_DeveRetornarOkComListaVazia_QuandoNaoHouverRegistros()
        {
            // Arrange
            var listaVazia = new List<QuestaoOpcaoRespostaDto>();
            _obterMock
                .Setup(s => s.ExecutarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(listaVazia);

            // Act
            var result = await _controller.Get(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var lista = Assert.IsType<List<QuestaoOpcaoRespostaDto>>(okResult.Value);
            Assert.Empty(lista);
            _obterMock.Verify(x => x.ExecutarAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Get_DeveRetornarOkComListaPreenchida_QuandoHouverRegistros()
        {
            // Arrange
            var lista = new List<QuestaoOpcaoRespostaDto>
            {
                new() { Id = 1, QuestaoId = 10, OpcaoRespostaId = 100, Ordem = 1 },
                new() { Id = 2, QuestaoId = 10, OpcaoRespostaId = 101, Ordem = 2 },
                new() { Id = 3, QuestaoId = 10, OpcaoRespostaId = 102, Ordem = 3 }
            };
            _obterMock
                .Setup(s => s.ExecutarAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(lista);

            // Act
            var result = await _controller.Get(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsType<List<QuestaoOpcaoRespostaDto>>(okResult.Value);
            Assert.Equal(3, retorno.Count);
            Assert.Equal(lista, okResult.Value);
            _obterMock.Verify(x => x.ExecutarAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Get_DevePassarCancellationToken_ParaUseCase()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            _obterMock
                .Setup(s => s.ExecutarAsync(cancellationToken))
                .ReturnsAsync(new List<QuestaoOpcaoRespostaDto>());

            // Act
            await _controller.Get(cancellationToken);

            // Assert
            _obterMock.Verify(x => x.ExecutarAsync(cancellationToken), Times.Once);
        }

        #endregion

        #region GET BY ID - Obter Por Id

        [Fact]
        public async Task GetById_DeveRetornarOk_QuandoEncontrarRegistro()
        {
            // Arrange
            const int id = 1;
            var dto = new QuestaoOpcaoRespostaDto
            {
                Id = id,
                QuestaoId = 10,
                OpcaoRespostaId = 100,
                Ordem = 1
            };
            _obterPorIdMock
                .Setup(s => s.ExecutarAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            // Act
            var result = await _controller.GetById(id, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsType<QuestaoOpcaoRespostaDto>(okResult.Value);
            Assert.Equal(dto.Id, retorno.Id);
            Assert.Equal(dto.QuestaoId, retorno.QuestaoId);
            Assert.Equal(dto.OpcaoRespostaId, retorno.OpcaoRespostaId);
            Assert.Equal(dto.Ordem, retorno.Ordem);
            _obterPorIdMock.Verify(x => x.ExecutarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetById_DeveLancarRegraNegocioException_QuandoNaoEncontrar()
        {
            // Arrange
            const long id = 999;
            _obterPorIdMock
                .Setup(s => s.ExecutarAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((QuestaoOpcaoRespostaDto?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RegraNegocioException>(
                () => _controller.GetById(id, CancellationToken.None));

            Assert.Equal(404, exception.StatusCode);
            Assert.Contains(id.ToString(), exception.Message);
            _obterPorIdMock.Verify(x => x.ExecutarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public async Task GetById_DevePassarIdCorreto_ParaUseCase(int id)
        {
            // Arrange
            _obterPorIdMock
                .Setup(s => s.ExecutarAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QuestaoOpcaoRespostaDto { Id = id });

            // Act
            await _controller.GetById(id, CancellationToken.None);

            // Assert
            _obterPorIdMock.Verify(x => x.ExecutarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region CREATE - Criar

        [Fact]
        public async Task Create_DeveRetornarCreatedAtAction_QuandoCriarComSucesso()
        {
            // Arrange
            const long idCriado = 10;
            var dto = new QuestaoOpcaoRespostaDto
            {
                QuestaoId = 5,
                OpcaoRespostaId = 50,
                Ordem = 1
            };
            _criarMock
                .Setup(s => s.ExecutarAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(idCriado);

            // Act
            var result = await _controller.Create(dto, CancellationToken.None);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(QuestaoOpcaoRespostaIntegracaoController.GetById), createdResult.ActionName);
            Assert.Equal(idCriado, createdResult.Value);
            var routeValues = createdResult.RouteValues;
            Assert.NotNull(routeValues);
            Assert.Equal(idCriado, routeValues["id"]);
            _criarMock.Verify(x => x.ExecutarAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_DeveRetornarStatusCode201_QuandoCriarComSucesso()
        {
            // Arrange
            var dto = new QuestaoOpcaoRespostaDto { QuestaoId = 1, OpcaoRespostaId = 1, Ordem = 1 };
            _criarMock
                .Setup(s => s.ExecutarAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1L);

            // Act
            var result = await _controller.Create(dto, CancellationToken.None);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task Create_DevePassarDtoCorreto_ParaUseCase()
        {
            // Arrange
            var dto = new QuestaoOpcaoRespostaDto
            {
                QuestaoId = 15,
                OpcaoRespostaId = 150,
                Ordem = 5
            };
            _criarMock
                .Setup(s => s.ExecutarAsync(It.IsAny<QuestaoOpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1L);

            // Act
            await _controller.Create(dto, CancellationToken.None);

            // Assert
            _criarMock.Verify(x => x.ExecutarAsync(
                It.Is<QuestaoOpcaoRespostaDto>(d =>
                    d.QuestaoId == dto.QuestaoId &&
                    d.OpcaoRespostaId == dto.OpcaoRespostaId &&
                    d.Ordem == dto.Ordem),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Create_DevePassarCancellationToken_ParaUseCase()
        {
            // Arrange
            var dto = new QuestaoOpcaoRespostaDto();
            var cancellationToken = new CancellationToken();
            _criarMock
                .Setup(s => s.ExecutarAsync(dto, cancellationToken))
                .ReturnsAsync(1L);

            // Act
            await _controller.Create(dto, cancellationToken);

            // Assert
            _criarMock.Verify(x => x.ExecutarAsync(dto, cancellationToken), Times.Once);
        }

        #endregion

        #region ATUALIZAR - Update

        [Fact]
        public async Task Atualizar_DeveRetornarOk_QuandoAtualizarComSucesso()
        {
            // Arrange
            const int id = 1;
            var dto = new QuestaoOpcaoRespostaDto
            {
                Id = id,
                QuestaoId = 20,
                OpcaoRespostaId = 200,
                Ordem = 2
            };
            _atualizarMock
                .Setup(s => s.ExecutarAsync(id, dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            // Act
            var result = await _controller.Atualizar(id, dto, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsType<QuestaoOpcaoRespostaDto>(okResult.Value);
            Assert.Equal(dto.Id, retorno.Id);
            Assert.Equal(dto.QuestaoId, retorno.QuestaoId);
            Assert.Equal(dto.OpcaoRespostaId, retorno.OpcaoRespostaId);
            Assert.Equal(dto.Ordem, retorno.Ordem);
            _atualizarMock.Verify(x => x.ExecutarAsync(id, dto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Atualizar_DeveLancarRegraNegocioException_QuandoNaoEncontrar()
        {
            // Arrange
            const int id = 999;
            var dto = new QuestaoOpcaoRespostaDto();
            _atualizarMock
                .Setup(s => s.ExecutarAsync(id, dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync((QuestaoOpcaoRespostaDto?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RegraNegocioException>(
                () => _controller.Atualizar(id, dto, CancellationToken.None));

            Assert.Equal(404, exception.StatusCode);
            Assert.Contains(id.ToString(), exception.Message);
            _atualizarMock.Verify(x => x.ExecutarAsync(id, dto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task Atualizar_DevePassarIdCorreto_ParaUseCase(int id)
        {
            // Arrange
            var dto = new QuestaoOpcaoRespostaDto { Id = id };
            _atualizarMock
                .Setup(s => s.ExecutarAsync(id, dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            // Act
            await _controller.Atualizar(id, dto, CancellationToken.None);

            // Assert
            _atualizarMock.Verify(x => x.ExecutarAsync(id, dto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarDtoAtualizado_ComTodasPropriedades()
        {
            // Arrange
            const int id = 1;
            var dtoAtualizado = new QuestaoOpcaoRespostaDto
            {
                Id = id,
                QuestaoId = 25,
                OpcaoRespostaId = 250,
                Ordem = 10
            };
            _atualizarMock
                .Setup(s => s.ExecutarAsync(id, It.IsAny<QuestaoOpcaoRespostaDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dtoAtualizado);

            // Act
            var result = await _controller.Atualizar(id, new QuestaoOpcaoRespostaDto(), CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var retorno = Assert.IsType<QuestaoOpcaoRespostaDto>(okResult.Value);
            Assert.Equal(dtoAtualizado.QuestaoId, retorno.QuestaoId);
            Assert.Equal(dtoAtualizado.OpcaoRespostaId, retorno.OpcaoRespostaId);
            Assert.Equal(dtoAtualizado.Ordem, retorno.Ordem);
        }

        [Fact]
        public async Task Atualizar_DevePassarCancellationToken_ParaUseCase()
        {
            // Arrange
            const int id = 1;
            var dto = new QuestaoOpcaoRespostaDto();
            var cancellationToken = new CancellationToken();
            _atualizarMock
                .Setup(s => s.ExecutarAsync(id, dto, cancellationToken))
                .ReturnsAsync(dto);

            // Act
            await _controller.Atualizar(id, dto, cancellationToken);

            // Assert
            _atualizarMock.Verify(x => x.ExecutarAsync(id, dto, cancellationToken), Times.Once);
        }

        #endregion

        #region EXCLUIR - Delete

        [Fact]
        public async Task Excluir_DeveRetornarNoContent_QuandoExcluirComSucesso()
        {
            // Arrange
            const int id = 1;
            _excluirMock
                .Setup(s => s.ExecutarAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Excluir(id, CancellationToken.None);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
            _excluirMock.Verify(x => x.ExecutarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Excluir_DeveLancarRegraNegocioException_QuandoNaoEncontrar()
        {
            // Arrange
            const int id = 999;
            _excluirMock
                .Setup(s => s.ExecutarAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RegraNegocioException>(
                () => _controller.Excluir(id, CancellationToken.None));

            Assert.Equal(404, exception.StatusCode);
            Assert.Contains(id.ToString(), exception.Message);
            _excluirMock.Verify(x => x.ExecutarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(25)]
        [InlineData(100)]
        public async Task Excluir_DevePassarIdCorreto_ParaUseCase(int id)
        {
            // Arrange
            _excluirMock
                .Setup(s => s.ExecutarAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _controller.Excluir(id, CancellationToken.None);

            // Assert
            _excluirMock.Verify(x => x.ExecutarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Excluir_DevePassarCancellationToken_ParaUseCase()
        {
            // Arrange
            const int id = 1;
            var cancellationToken = new CancellationToken();
            _excluirMock
                .Setup(s => s.ExecutarAsync(id, cancellationToken))
                .ReturnsAsync(true);

            // Act
            await _controller.Excluir(id, cancellationToken);

            // Assert
            _excluirMock.Verify(x => x.ExecutarAsync(id, cancellationToken), Times.Once);
        }

        #endregion
    }
}
