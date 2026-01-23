using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers.Integracao;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.API.Teste.Controller
{
    public class QuestaoIntegracaoControllerTeste
    {
        private readonly Mock<IObterQuestoesUseCase> _obterMock;
        private readonly Mock<IObterQuestaoPorIdUseCase> _obterPorIdMock;
        private readonly Mock<ICriarQuestaoUseCase> _criarMock;
        private readonly Mock<IAtualizarQuestaoUseCase> _atualizarMock;
        private readonly Mock<IExcluirQuestaoUseCase> _excluirMock;

        public QuestaoIntegracaoControllerTeste()
        {
            _obterMock = new Mock<IObterQuestoesUseCase>();
            _obterPorIdMock = new Mock<IObterQuestaoPorIdUseCase>();
            _criarMock = new Mock<ICriarQuestaoUseCase>();
            _atualizarMock = new Mock<IAtualizarQuestaoUseCase>();
            _excluirMock = new Mock<IExcluirQuestaoUseCase>();
        }

        private QuestaoIntegracaoController CriarController()
            => new(
                _obterMock.Object,
                _obterPorIdMock.Object,
                _criarMock.Object,
                _atualizarMock.Object,
                _excluirMock.Object
            );

        private static QuestaoDto CriarDto(int id = 1)
        {
            return new QuestaoDto
            {
                Id = id,
                Nome = "Nome Teste",
                Tipo = Dominio.Enums.TipoQuestao.Periodo
            };
        }

        #region GET - Listar Questões

        [Fact(DisplayName = "GET - Deve retornar Ok com lista de questões")]
        public async Task Get_DeveRetornarOkComListaDeQuestoes()
        {
            // Arrange
            var questoesEsperadas = new[] { CriarDto(1), CriarDto(2) };
            _obterMock.Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(questoesEsperadas);
            var controller = CriarController();

            // Act
            var result = await controller.Get(CancellationToken.None);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(questoesEsperadas);
            _obterMock.Verify(x => x.ExecutarAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "GET - Deve retornar Ok com lista vazia quando não houver questões")]
        public async Task Get_DeveRetornarOkComListaVazia()
        {
            // Arrange
            _obterMock.Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Array.Empty<QuestaoDto>());
            var controller = CriarController();

            // Act
            var result = await controller.Get(CancellationToken.None);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var questoes = okResult.Value.Should().BeAssignableTo<IEnumerable<QuestaoDto>>().Subject;
            questoes.Should().BeEmpty();
        }

        [Fact(DisplayName = "GET - Deve retornar 499 quando requisição for cancelada")]
        public async Task Get_DeveRetornar499QuandoRequisicaoCancelada()
        {
            // Arrange
            _obterMock.Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new OperationCanceledException());
            var controller = CriarController();

            // Act
            var result = await controller.Get(CancellationToken.None);

            // Assert
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(499);
            objectResult.Value.Should().BeEquivalentTo(new { mensagem = MensagemNegocioComuns.REQUISICAO_CANCELADA });
        }

        [Fact(DisplayName = "GET - Deve retornar 500 quando ocorrer exceção genérica")]
        public async Task Get_DeveRetornar500QuandoOcorrerExcecao()
        {
            // Arrange
            _obterMock.Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new Exception("Erro inesperado"));
            var controller = CriarController();

            // Act
            var result = await controller.Get(CancellationToken.None);

            // Assert
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().BeEquivalentTo(new { mensagem = "Erro ao listar questões" });
        }

        #endregion

        #region GET BY ID - Obter Questão por ID

        [Fact(DisplayName = "GET BY ID - Deve retornar Ok com questão encontrada")]
        public async Task GetById_DeveRetornarOkComQuestao()
        {
            // Arrange
            const int idQuestao = 1;
            var questaoEsperada = CriarDto(idQuestao);
            _obterPorIdMock.Setup(x => x.ExecutarAsync(idQuestao, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(questaoEsperada);
            var controller = CriarController();

            // Act
            var result = await controller.GetById(idQuestao, CancellationToken.None);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(questaoEsperada);
            _obterPorIdMock.Verify(x => x.ExecutarAsync(idQuestao, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "GET BY ID - Deve retornar NotFound quando questão não existir")]
        public async Task GetById_DeveRetornarNotFoundQuandoNaoEncontrada()
        {
            // Arrange
            const long idQuestao = 999;
            _obterPorIdMock.Setup(x => x.ExecutarAsync(idQuestao, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((QuestaoDto?)null);
            var controller = CriarController();

            // Act
            var result = await controller.GetById(idQuestao, CancellationToken.None);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            notFoundResult.Value.Should().BeEquivalentTo(new { mensagem = string.Format(MensagemNegocioComuns.QUESTAO_NAO_ENCONTRADA, idQuestao) });
        }

        [Fact(DisplayName = "GET BY ID - Deve retornar 499 quando requisição for cancelada")]
        public async Task GetById_DeveRetornar499QuandoRequisicaoCancelada()
        {
            // Arrange
            _obterPorIdMock.Setup(x => x.ExecutarAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new OperationCanceledException());
            var controller = CriarController();

            // Act
            var result = await controller.GetById(1, CancellationToken.None);

            // Assert
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(499);
        }

        [Fact(DisplayName = "GET BY ID - Deve retornar 500 quando ocorrer exceção genérica")]
        public async Task GetById_DeveRetornar500QuandoOcorrerExcecao()
        {
            // Arrange
            _obterPorIdMock.Setup(x => x.ExecutarAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new Exception("Erro inesperado"));
            var controller = CriarController();

            // Act
            var result = await controller.GetById(1, CancellationToken.None);

            // Assert
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().BeEquivalentTo(new { mensagem = "Erro ao obter questão" });
        }

        #endregion

        #region CREATE - Criar Questão

        [Fact(DisplayName = "CREATE - Deve retornar Created com questão criada")]
        public async Task Create_DeveRetornarCreatedComQuestao()
        {
            // Arrange
            const long idCriado = 1;
            var questaoDto = CriarDto();
            _criarMock.Setup(x => x.ExecutarAsync(It.IsAny<QuestaoDto>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(idCriado);
            var controller = CriarController();

            // Act
            var result = await controller.Create(questaoDto, CancellationToken.None);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            createdResult.ActionName.Should().Be(nameof(QuestaoIntegracaoController.GetById));
            createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(idCriado);
            createdResult.Value.Should().Be(idCriado);
            _criarMock.Verify(x => x.ExecutarAsync(questaoDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "CREATE - Deve retornar BadRequest quando validação falhar")]
        public async Task Create_DeveRetornarBadRequestQuandoValidacaoFalhar()
        {
            // Arrange
            var questaoDto = CriarDto();
            var validationException = new FluentValidation.ValidationException(
                new[]
                {
                    new FluentValidation.Results.ValidationFailure("Nome", "Nome é obrigatório"),
                    new FluentValidation.Results.ValidationFailure("Tipo", "Tipo inválido")
                }
            );
            _criarMock.Setup(x => x.ExecutarAsync(It.IsAny<QuestaoDto>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(validationException);
            var controller = CriarController();

            // Act
            var result = await controller.Create(questaoDto, CancellationToken.None);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact(DisplayName = "CREATE - Deve retornar status code da RegraNegocioException")]
        public async Task Create_DeveRetornarStatusCodeDaRegraNegocioException()
        {
            // Arrange
            var questaoDto = CriarDto();
            var regraNegocioException = new RegraNegocioException("Questão duplicada", StatusCodes.Status409Conflict);
            _criarMock.Setup(x => x.ExecutarAsync(It.IsAny<QuestaoDto>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(regraNegocioException);
            var controller = CriarController();

            // Act
            var result = await controller.Create(questaoDto, CancellationToken.None);

            // Assert
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            objectResult.Value.Should().BeEquivalentTo(new { mensagem = "Questão duplicada" });
        }

        [Fact(DisplayName = "CREATE - Deve retornar 499 quando requisição for cancelada")]
        public async Task Create_DeveRetornar499QuandoRequisicaoCancelada()
        {
            // Arrange
            _criarMock.Setup(x => x.ExecutarAsync(It.IsAny<QuestaoDto>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new OperationCanceledException());
            var controller = CriarController();

            // Act
            var result = await controller.Create(CriarDto(), CancellationToken.None);

            // Assert
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(499);
        }

        [Fact(DisplayName = "CREATE - Deve retornar 500 quando ocorrer exceção genérica")]
        public async Task Create_DeveRetornar500QuandoOcorrerExcecao()
        {
            // Arrange
            _criarMock.Setup(x => x.ExecutarAsync(It.IsAny<QuestaoDto>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new Exception("Erro inesperado"));
            var controller = CriarController();

            // Act
            var result = await controller.Create(CriarDto(), CancellationToken.None);

            // Assert
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().BeEquivalentTo(new { mensagem = "Erro ao criar questão" });
        }

        #endregion

        #region UPDATE - Atualizar Questão

        [Fact(DisplayName = "UPDATE - Deve retornar Ok com questão atualizada")]
        public async Task Atualizar_DeveRetornarOkComQuestaoAtualizada()
        {
            // Arrange
            const int idQuestao = 1;
            var questaoDto = CriarDto(idQuestao);
            var questaoAtualizada = CriarDto(idQuestao);
            questaoAtualizada.Nome = "Nome Atualizado";
            _atualizarMock.Setup(x => x.ExecutarAsync(idQuestao, It.IsAny<QuestaoDto>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(questaoAtualizada);
            var controller = CriarController();

            // Act
            var result = await controller.Atualizar(idQuestao, questaoDto, CancellationToken.None);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(questaoAtualizada);
            _atualizarMock.Verify(x => x.ExecutarAsync(idQuestao, questaoDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "UPDATE - Deve retornar NotFound quando questão não existir")]
        public async Task Atualizar_DeveRetornarNotFoundQuandoNaoEncontrada()
        {
            // Arrange
            const int idQuestao = 999;
            _atualizarMock.Setup(x => x.ExecutarAsync(idQuestao, It.IsAny<QuestaoDto>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((QuestaoDto?)null);
            var controller = CriarController();

            // Act
            var result = await controller.Atualizar(idQuestao, CriarDto(), CancellationToken.None);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            notFoundResult.Value.Should().BeEquivalentTo(new { mensagem = string.Format(MensagemNegocioComuns.QUESTAO_NAO_ENCONTRADA, idQuestao) });
        }

        [Fact(DisplayName = "UPDATE - Deve retornar BadRequest quando validação falhar")]
        public async Task Atualizar_DeveRetornarBadRequestQuandoValidacaoFalhar()
        {
            // Arrange
            const int idQuestao = 1;
            var questaoDto = CriarDto(idQuestao);
            var validationException = new FluentValidation.ValidationException(
                new[] { new FluentValidation.Results.ValidationFailure("Nome", "Nome é obrigatório") }
            );

            // Simular que a questão existe primeiro (não retorna null)
            _atualizarMock.Setup(x => x.ExecutarAsync(idQuestao, It.IsAny<QuestaoDto>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(validationException);

            var controller = CriarController();

            // Act
            var result = await controller.Atualizar(idQuestao, questaoDto, CancellationToken.None);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact(DisplayName = "UPDATE - Deve retornar status code da RegraNegocioException")]
        public async Task Atualizar_DeveRetornarStatusCodeDaRegraNegocioException()
        {
            // Arrange
            const int idQuestao = 1;
            var questaoDto = CriarDto(idQuestao);
            var regraNegocioException = new RegraNegocioException("Conflito ao atualizar", StatusCodes.Status409Conflict);

            _atualizarMock.Setup(x => x.ExecutarAsync(idQuestao, It.IsAny<QuestaoDto>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(regraNegocioException);

            var controller = CriarController();

            // Act
            var result = await controller.Atualizar(idQuestao, questaoDto, CancellationToken.None);

            // Assert
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        }

        [Fact(DisplayName = "UPDATE - Deve retornar 499 quando requisição for cancelada")]
        public async Task Atualizar_DeveRetornar499QuandoRequisicaoCancelada()
        {
            // Arrange
            const int idQuestao = 1;
            var questaoDto = CriarDto(idQuestao);

            _atualizarMock.Setup(x => x.ExecutarAsync(idQuestao, It.IsAny<QuestaoDto>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new OperationCanceledException());

            var controller = CriarController();

            // Act
            var result = await controller.Atualizar(idQuestao, questaoDto, CancellationToken.None);

            // Assert
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(499);
        }

        [Fact(DisplayName = "UPDATE - Deve retornar 500 quando ocorrer exceção genérica")]
        public async Task Atualizar_DeveRetornar500QuandoOcorrerExcecao()
        {
            // Arrange
            const int idQuestao = 1;
            var questaoDto = CriarDto(idQuestao);

            _atualizarMock.Setup(x => x.ExecutarAsync(idQuestao, It.IsAny<QuestaoDto>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Erro inesperado"));

            var controller = CriarController();

            // Act
            var result = await controller.Atualizar(idQuestao, questaoDto, CancellationToken.None);

            // Assert
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().BeEquivalentTo(new { mensagem = "Erro ao atualizar questão" });
        }

        #endregion

        #region DELETE - Excluir Questão

        [Fact(DisplayName = "DELETE - Deve retornar NoContent quando questão for excluída")]
        public async Task Excluir_DeveRetornarNoContentQuandoExcluida()
        {
            // Arrange
            const int idQuestao = 1;
            _excluirMock.Setup(x => x.ExecutarAsync(idQuestao, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);
            var controller = CriarController();

            // Act
            var result = await controller.Excluir(idQuestao, CancellationToken.None);

            // Assert
            var noContentResult = result.Should().BeOfType<NoContentResult>().Subject;
            noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
            _excluirMock.Verify(x => x.ExecutarAsync(idQuestao, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "DELETE - Deve retornar NotFound quando questão não existir")]
        public async Task Excluir_DeveRetornarNotFoundQuandoNaoEncontrada()
        {
            // Arrange
            const int idQuestao = 999;
            _excluirMock.Setup(x => x.ExecutarAsync(idQuestao, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false);
            var controller = CriarController();

            // Act
            var result = await controller.Excluir(idQuestao, CancellationToken.None);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            notFoundResult.Value.Should().BeEquivalentTo(new { mensagem = string.Format(MensagemNegocioComuns.QUESTAO_NAO_ENCONTRADA, idQuestao) });
        }

        [Fact(DisplayName = "DELETE - Deve retornar 499 quando requisição for cancelada")]
        public async Task Excluir_DeveRetornar499QuandoRequisicaoCancelada()
        {
            // Arrange
            const int idQuestao = 1;

            _excluirMock.Setup(x => x.ExecutarAsync(idQuestao, It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new OperationCanceledException());

            var controller = CriarController();

            // Act
            var result = await controller.Excluir(idQuestao, CancellationToken.None);

            // Assert
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(499);
        }

        [Fact(DisplayName = "DELETE - Deve retornar 500 quando ocorrer exceção genérica")]
        public async Task Excluir_DeveRetornar500QuandoOcorrerExcecao()
        {
            // Arrange
            const int idQuestao = 1;

            _excluirMock.Setup(x => x.ExecutarAsync(idQuestao, It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new Exception("Erro inesperado"));

            var controller = CriarController();

            // Act
            var result = await controller.Excluir(idQuestao, CancellationToken.None);

            // Assert
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().BeEquivalentTo(new { mensagem = "Erro ao excluir questão" });
        }

        #endregion
    }
}
