using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.Aplicacao.Interfaces.ComponenteCurricular;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infrastructure.Dtos.ComponenteCurricular;
using System.Net;
using Xunit;

namespace SME.Sondagem.API.Teste.Controllers;

public class ComponenteCurricularControllerTeste
{
    private readonly Mock<IComponenteCurricularUseCase> _useCaseMock;
    private readonly ComponenteCurricularController _controller;
    private readonly CancellationToken _cancellationToken;

    public ComponenteCurricularControllerTeste()
    {
        _useCaseMock = new Mock<IComponenteCurricularUseCase>();
        _controller = new ComponenteCurricularController(_useCaseMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task Listar_DeveRetornarOk_ComLista()
    {
        var lista = new List<ComponenteCurricularDto>
        {
            new() { Id = 1, Nome = "Língua Portuguesa", Modalidade = "Fundamental", CodigoEol = 1 },
            new() { Id = 3, Nome = "Língua Portuguesa", Modalidade = "EJA", CodigoEol = 1 }
        };

        _useCaseMock
            .Setup(x => x.ListarAsync(Dominio.Enums.Modalidade.EJA, _cancellationToken))
            .ReturnsAsync(lista);

        var result = await _controller.Listar(Dominio.Enums.Modalidade.EJA, _cancellationToken);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        Assert.Equal(lista, ok.Value);
    }

    [Fact]
    public async Task Listar_OperationCanceledException_DeveRetornar499()
    {
        _useCaseMock
            .Setup(x => x.ListarAsync(Dominio.Enums.Modalidade.EJA, _cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _controller.Listar(Dominio.Enums.Modalidade.EJA, _cancellationToken);

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(499, obj.StatusCode);

        var mensagemProperty = obj.Value!.GetType().GetProperty("mensagem");
        Assert.Equal(MensagemNegocioComuns.REQUISICAO_CANCELADA, mensagemProperty?.GetValue(obj.Value));
    }

    [Fact]
    public async Task Listar_Exception_DeveRetornar500()
    {
        _useCaseMock
            .Setup(x => x.ListarAsync(Dominio.Enums.Modalidade.EJA, _cancellationToken))
            .ThrowsAsync(new Exception("erro"));

        var result = await _controller.Listar(Dominio.Enums.Modalidade.EJA, _cancellationToken);

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, obj.StatusCode);

        var mensagemProperty = obj.Value!.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao listar componentes curriculares", mensagemProperty?.GetValue(obj.Value));
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarOk_QuandoEncontrar()
    {
        const int id = 1;
        var dto = new ComponenteCurricularDto
        {
            Id = id,
            Nome = "Língua Portuguesa",
            Modalidade = "Fundamental",
            CodigoEol = 1
        };

        _useCaseMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(dto);

        var result = await _controller.ObterPorId(id, _cancellationToken);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarNotFound_QuandoNaoEncontrar()
    {
        const int id = 999;

        _useCaseMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync((ComponenteCurricularDto?)null);

        var result = await _controller.ObterPorId(id, _cancellationToken);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);

        var mensagemProperty = notFound.Value!.GetType().GetProperty("mensagem");
        Assert.Equal(string.Format(MensagemNegocioComuns.COMPONENTE_CURRICULAR_NAO_ENCONTRADO, id), mensagemProperty?.GetValue(notFound.Value));
    }

    [Fact]
    public async Task ObterPorCodigoEol_DeveRetornarOk_QuandoEncontrar()
    {
        const int codigoEol = 1;

        var dto = new ComponenteCurricularDto
        {
            Id = 1,
            Nome = "Língua Portuguesa",
            Modalidade = "Fundamental",
            CodigoEol = codigoEol
        };

        _useCaseMock
            .Setup(x => x.ObterPorCodigoEolAsync(codigoEol, _cancellationToken))
            .ReturnsAsync(dto);

        var result = await _controller.ObterPorCodigoEol(codigoEol, _cancellationToken);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task ObterPorCodigoEol_DeveRetornarNotFound_QuandoNaoEncontrar()
    {
        const int codigoEol = 999;

        _useCaseMock
            .Setup(x => x.ObterPorCodigoEolAsync(codigoEol, _cancellationToken))
            .ReturnsAsync((ComponenteCurricularDto?)null);

        var result = await _controller.ObterPorCodigoEol(codigoEol, _cancellationToken);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);

        var mensagemProperty = notFound.Value!.GetType().GetProperty("mensagem");
        Assert.Equal(string.Format(MensagemNegocioComuns.COMPONENTE_CURRICULAR_EOL_NAO_ENCONTRADO, codigoEol), mensagemProperty?.GetValue(notFound.Value));
    }

    [Fact]
    public async Task ObterPorModalidade_DeveRetornarOk_ComLista()
    {
        const string modalidade = "EJA";

        var lista = new List<ComponenteCurricularDto>
        {
            new() { Id = 3, Nome = "Língua Portuguesa", Modalidade = modalidade, CodigoEol = 1 }
        };

        _useCaseMock
            .Setup(x => x.ObterPorModalidadeAsync(modalidade, _cancellationToken))
            .ReturnsAsync(lista);

        var result = await _controller.ObterPorModalidade(modalidade, _cancellationToken);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);

        var retornado = Assert.IsAssignableFrom<IEnumerable<ComponenteCurricularDto>>(ok.Value);
        Assert.Single(retornado);
    }

    [Fact]
    public async Task ObterPorModalidade_DeveRetornarNotFound_QuandoListaVazia()
    {
        const string modalidade = "EJA";

        _useCaseMock
            .Setup(x => x.ObterPorModalidadeAsync(modalidade, _cancellationToken))
            .ReturnsAsync(Array.Empty<ComponenteCurricularDto>());

        var result = await _controller.ObterPorModalidade(modalidade, _cancellationToken);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);

        var mensagemProperty = notFound.Value!.GetType().GetProperty("mensagem");
        Assert.Equal(string.Format(MensagemNegocioComuns.COMPONENTE_CURRICULAR_MODALIDADE_NAO_ENCONTRADO, modalidade), mensagemProperty?.GetValue(notFound.Value));
    }

    [Fact]
    public async Task Criar_DeveRetornarCreated_QuandoSucesso()
    {
        var dto = new CriarComponenteCurricularDto
        {
            Nome = "Língua Portuguesa",
            Ano = 1,
            Modalidade = "EJA",
            CodigoEol = 1
        };

        var retorno = new ComponenteCurricularDto
        {
            Id = 10,
            Nome = dto.Nome,
            Ano = dto.Ano,
            Modalidade = dto.Modalidade ?? string.Empty,
            CodigoEol = dto.CodigoEol
        };

        _useCaseMock
            .Setup(x => x.CriarAsync(dto, _cancellationToken))
            .ReturnsAsync(retorno);

        var result = await _controller.Criar(dto, _cancellationToken);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        Assert.Equal(nameof(ComponenteCurricularController.ObterPorId), created.ActionName);
        Assert.Equal(retorno, created.Value);

        Assert.NotNull(created.RouteValues);
        Assert.Equal(retorno.Id, created.RouteValues!["id"]);
    }

    [Fact]
    public async Task Criar_RegraNegocioException_DeveRetornarStatusDaExcecao()
    {
        var dto = new CriarComponenteCurricularDto
        {
            Nome = "Língua Portuguesa",
            Ano = 1,
            Modalidade = "EJA",
            CodigoEol = 1
        };

        var ex = new RegraNegocioException("Já existe", HttpStatusCode.Conflict);

        _useCaseMock
            .Setup(x => x.CriarAsync(dto, _cancellationToken))
            .ThrowsAsync(ex);

        var result = await _controller.Criar(dto, _cancellationToken);

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.Conflict, obj.StatusCode);

        var mensagemProperty = obj.Value!.GetType().GetProperty("mensagem");
        Assert.Equal(ex.Message, mensagemProperty?.GetValue(obj.Value));
    }

    [Fact]
    public async Task Criar_Exception_DeveRetornar500()
    {
        var dto = new CriarComponenteCurricularDto
        {
            Nome = "Língua Portuguesa",
            Ano = 1,
            Modalidade = "EJA",
            CodigoEol = 1
        };

        _useCaseMock
            .Setup(x => x.CriarAsync(dto, _cancellationToken))
            .ThrowsAsync(new Exception("erro"));

        var result = await _controller.Criar(dto, _cancellationToken);

        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, obj.StatusCode);

        var mensagemProperty = obj.Value!.GetType().GetProperty("mensagem");
        Assert.Equal("Erro ao criar componente curricular", mensagemProperty?.GetValue(obj.Value));
    }

    [Fact]
    public async Task Excluir_DeveRetornarNoContent_QuandoSucesso()
    {
        const int id = 1;

        _useCaseMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ReturnsAsync(true);

        var result = await _controller.Excluir(id, _cancellationToken);

        var noContent = Assert.IsType<NoContentResult>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContent.StatusCode);
    }

    [Fact]
    public async Task Excluir_DeveRetornarNotFound_QuandoNaoEncontrar()
    {
        const int id = 999;

        _useCaseMock
            .Setup(x => x.ExcluirAsync(id, _cancellationToken))
            .ReturnsAsync(false);

        var result = await _controller.Excluir(id, _cancellationToken);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);

        var mensagemProperty = notFound.Value!.GetType().GetProperty("mensagem");
        Assert.Equal(string.Format(MensagemNegocioComuns.COMPONENTE_CURRICULAR_NAO_ENCONTRADO, id), mensagemProperty?.GetValue(notFound.Value));
    }
}