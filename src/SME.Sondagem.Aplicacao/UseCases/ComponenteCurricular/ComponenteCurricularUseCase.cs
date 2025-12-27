using FluentValidation;
using SME.Sondagem.Aplicacao.Interfaces.ComponenteCurricular;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infrastructure.Dtos.ComponenteCurricular;
using System.Net;

namespace SME.Sondagem.Aplicacao.UseCases.ComponenteCurricular;

public class ComponenteCurricularUseCase : IComponenteCurricularUseCase
{
    private readonly IRepositorioComponenteCurricular _repositorio;
    private readonly IValidator<CriarComponenteCurricularDto> _validatorCriar;
    private readonly IValidator<AtualizarComponenteCurricularDto> _validatorAtualizar;

    public ComponenteCurricularUseCase(
        IRepositorioComponenteCurricular repositorio,
        IValidator<CriarComponenteCurricularDto> validatorCriar,
        IValidator<AtualizarComponenteCurricularDto> validatorAtualizar)
    {
        _repositorio = repositorio;
        _validatorCriar = validatorCriar;
        _validatorAtualizar = validatorAtualizar;
    }

    public async Task<ComponenteCurricularDto> CriarAsync(CriarComponenteCurricularDto dto, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validatorCriar.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var existe = await _repositorio.ExisteComCodigoEolAsync(dto.CodigoEol, cancellationToken: cancellationToken);
        if (existe)
        {
            throw new NegocioException(
                $"Já existe um componente curricular com o código EOL {dto.CodigoEol}",
                HttpStatusCode.Conflict
            );
        }

        var entidade = new Dominio.Entidades.ComponenteCurricular(
            nome: dto.Nome,
            ano: dto.Ano,
            modalidade: dto.Modalidade,
            codigoEol: dto.CodigoEol
        );

        var id = await _repositorio.SalvarAsync(entidade, cancellationToken);

        var entidadeSalva = await _repositorio.ObterPorIdAsync(id, cancellationToken);
        return MapearParaDto(entidadeSalva!);
    }

    public async Task<ComponenteCurricularDto> AtualizarAsync(int id, AtualizarComponenteCurricularDto dto, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validatorAtualizar.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var entidade = await _repositorio.ObterPorIdAsync(id, cancellationToken);
        if (entidade == null)
        {
            throw new NegocioException(
                $"Componente curricular com ID {id} não encontrado",
                HttpStatusCode.NotFound
            );
        }

        var existe = await _repositorio.ExisteComCodigoEolAsync(dto.CodigoEol, id, cancellationToken);
        if (existe)
        {
            throw new NegocioException(
                $"Já existe outro componente curricular com o código EOL {dto.CodigoEol}",
                HttpStatusCode.Conflict
            );
        }

        var entidadeAtualizada = new Dominio.Entidades.ComponenteCurricular(
            nome: dto.Nome,
            ano: dto.Ano,
            modalidade: dto.Modalidade,
            codigoEol: dto.CodigoEol
        );

        typeof(Dominio.Entidades.ComponenteCurricular)
            .GetProperty("Id")!
            .SetValue(entidadeAtualizada, id);

        await _repositorio.SalvarAsync(entidadeAtualizada, cancellationToken);

        var entidadeSalva = await _repositorio.ObterPorIdAsync(id, cancellationToken);
        return MapearParaDto(entidadeSalva!);
    }

    public async Task<ComponenteCurricularDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entidade = await _repositorio.ObterPorIdAsync(id, cancellationToken);
        return entidade != null ? MapearParaDto(entidade) : null;
    }

    public async Task<IEnumerable<ComponenteCurricularDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var entidades = await _repositorio.ListarAsync(cancellationToken);
        return entidades.Select(MapearParaDto);
    }

    public async Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        var resultado = await _repositorio.RemoverLogico(id, cancellationToken: cancellationToken);
        return resultado > 0;
    }

    public async Task<ComponenteCurricularDto?> ObterPorCodigoEolAsync(int codigoEol, CancellationToken cancellationToken = default)
    {
        var entidade = await _repositorio.ObterPorCodigoEolAsync(codigoEol, cancellationToken);
        return entidade != null ? MapearParaDto(entidade) : null;
    }

    private static ComponenteCurricularDto MapearParaDto(Dominio.Entidades.ComponenteCurricular entidade)
    {
        return new ComponenteCurricularDto
        {
            Id = entidade.Id,
            Nome = entidade.Nome,
            Ano = entidade.Ano,
            Modalidade = entidade.Modalidade,
            CodigoEol = entidade.CodigoEol,
            CriadoEm = entidade.CriadoEm,
            CriadoPor = entidade.CriadoPor,
            AlteradoEm = entidade.AlteradoEm,
            AlteradoPor = entidade.AlteradoPor
        };
    }
}