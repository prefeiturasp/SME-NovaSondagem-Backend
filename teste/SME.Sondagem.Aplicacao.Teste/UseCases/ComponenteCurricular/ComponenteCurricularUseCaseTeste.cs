using FluentValidation;
using FluentValidation.Results;
using Moq;
using SME.Sondagem.Aplicacao.UseCases.ComponenteCurricular;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infrastructure.Dtos.ComponenteCurricular;
using System.Net;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.ComponenteCurricular;

public class ComponenteCurricularUseCaseTeste
{
    private readonly Mock<IRepositorioComponenteCurricular> _repositorioMock;
    private readonly Mock<IValidator<CriarComponenteCurricularDto>> _validatorCriarMock;
    private readonly Mock<IValidator<AtualizarComponenteCurricularDto>> _validatorAtualizarMock;
    private readonly ComponenteCurricularUseCase _useCase;

    public ComponenteCurricularUseCaseTeste()
    {
        _repositorioMock = new Mock<IRepositorioComponenteCurricular>();
        _validatorCriarMock = new Mock<IValidator<CriarComponenteCurricularDto>>();
        _validatorAtualizarMock = new Mock<IValidator<AtualizarComponenteCurricularDto>>();

        _useCase = new ComponenteCurricularUseCase(
            _repositorioMock.Object,
            _validatorCriarMock.Object,
            _validatorAtualizarMock.Object);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNull_QuandoNaoEncontrar()
    {
        const long id = 99;

        _repositorioMock
            .Setup(r => r.ObterPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Dominio.Entidades.ComponenteCurricular?)null);

        var resultado = await _useCase.ObterPorIdAsync((int)id, CancellationToken.None);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ListarAsync_DeveMapearLista()
    {
        var entidades = new List<Dominio.Entidades.ComponenteCurricular>
        {
            new(nome: "Língua Portuguesa", ano: null, modalidade: "Fundamental", codigoEol: 1),
            new(nome: "Matemática", ano: null, modalidade: "Fundamental", codigoEol: 2)
        };

        typeof(Dominio.Entidades.ComponenteCurricular).GetProperty("Id")!.SetValue(entidades[0], 1);
        typeof(Dominio.Entidades.ComponenteCurricular).GetProperty("Id")!.SetValue(entidades[1], 3);

        var vinculo0 = CriarVinculoModalidade(Modalidade.Fundamental);
        var vinculo1 = CriarVinculoModalidade(Modalidade.Fundamental);

        typeof(Dominio.Entidades.ComponenteCurricular).GetProperty("ModalidadeComponenteCurricular")!.SetValue(entidades[0], vinculo0);
        typeof(Dominio.Entidades.ComponenteCurricular).GetProperty("ModalidadeComponenteCurricular")!.SetValue(entidades[1], vinculo1);

        _repositorioMock
            .Setup(r => r.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entidades);

        var resultado = (await _useCase.ListarAsync(Modalidade.Fundamental, CancellationToken.None)).ToList();

        Assert.Equal(2, resultado.Count);

        Assert.Equal(1, resultado[0].Id);
        Assert.Equal("Língua Portuguesa", resultado[0].Nome);
        Assert.Equal("Fundamental", resultado[0].Modalidade);

        Assert.Equal(3, resultado[1].Id);
        Assert.Equal("Matemática", resultado[1].Nome);
        Assert.Equal("Fundamental", resultado[1].Modalidade);
    }

    private static List<Dominio.Entidades.ModalidadeComponenteCurricular> CriarVinculoModalidade(Modalidade modalidade)
    {
        var instancia = Activator.CreateInstance(typeof(Dominio.Entidades.ModalidadeComponenteCurricular), nonPublic: true)!;
        typeof(Dominio.Entidades.ModalidadeComponenteCurricular)
            .GetProperty("ModalidadeId")!
            .SetValue(instancia, modalidade);

        return [(Dominio.Entidades.ModalidadeComponenteCurricular)instancia];
    }

    [Fact]
    public async Task ObterPorModalidadeAsync_DeveRetornarLista_Mapeada()
    {
        const string modalidade = "EJA";

        var entidades = new List<Dominio.Entidades.ComponenteCurricular>
        {
            new(nome: "Língua Portuguesa", ano: null, modalidade: modalidade, codigoEol: 1)
        };

        typeof(Dominio.Entidades.ComponenteCurricular).GetProperty("Id")!.SetValue(entidades[0], 3);

        _repositorioMock
            .Setup(r => r.ObterPorModalidadeAsync(modalidade, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entidades);

        var resultado = (await _useCase.ObterPorModalidadeAsync(modalidade, CancellationToken.None)).ToList();

        Assert.Single(resultado);
        Assert.Equal(3, resultado[0].Id);
        Assert.Equal("Língua Portuguesa", resultado[0].Nome);
        Assert.Equal(modalidade, resultado[0].Modalidade);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarRegraNegocioException_QuandoCodigoEolJaExiste()
    {
        var dto = new CriarComponenteCurricularDto
        {
            Nome = "Língua Portuguesa",
            Ano = null,
            Modalidade = "EJA",
            CodigoEol = 1
        };

        _validatorCriarMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repositorioMock
            .Setup(r => r.ExisteComCodigoEolAsync(dto.CodigoEol, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<RegraNegocioException>(() => _useCase.CriarAsync(dto, CancellationToken.None));

        Assert.Equal((int)HttpStatusCode.Conflict, ex.StatusCode);
                
        Assert.Contains("existe um componente curricular", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(dto.CodigoEol.ToString(), ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarDto_QuandoSucesso()
    {
        var dto = new CriarComponenteCurricularDto
        {
            Nome = "Língua Portuguesa",
            Ano = 1,
            Modalidade = "EJA",
            CodigoEol = 1
        };

        _validatorCriarMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repositorioMock
            .Setup(r => r.ExisteComCodigoEolAsync(dto.CodigoEol, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        const long idSalvo = 10;

        _repositorioMock
            .Setup(r => r.SalvarAsync(It.IsAny<Dominio.Entidades.ComponenteCurricular>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(idSalvo);

        var entidadeSalva = new Dominio.Entidades.ComponenteCurricular(dto.Nome, dto.Ano, dto.Modalidade, dto.CodigoEol);
        typeof(Dominio.Entidades.ComponenteCurricular).GetProperty("Id")!.SetValue(entidadeSalva, (int)idSalvo);

        _repositorioMock
            .Setup(r => r.ObterPorIdAsync(idSalvo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entidadeSalva);

        var resultado = await _useCase.CriarAsync(dto, CancellationToken.None);

        Assert.NotNull(resultado);
        Assert.Equal((int)idSalvo, resultado.Id);
        Assert.Equal(dto.Nome, resultado.Nome);
        Assert.Equal(dto.Ano, resultado.Ano);
        Assert.Equal(dto.Modalidade, resultado.Modalidade);
        Assert.Equal(dto.CodigoEol, resultado.CodigoEol);
    }
}