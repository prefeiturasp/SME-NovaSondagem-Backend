using FluentValidation;
using SME.Sondagem.Aplicacao.Interfaces.ComponenteCurricular;
using SME.Sondagem.Dados.Interfaces;
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

    public async Task<ComponenteCurricularDto> CriarAsync(CriarComponenteCurricularDto dto)
    {
        var validationResult = await _validatorCriar.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var existe = await _repositorio.ExisteComCodigoEolAsync(dto.CodigoEol);
        if (existe)
        {
            throw new NegocioException(
                $"Já existe um componente curricular com o código EOL {dto.CodigoEol}",
                HttpStatusCode.Conflict // 409
            );
        }

        var entidade = new Dominio.Entidades.ComponenteCurricular(
            nome: dto.Nome,
            ano: dto.Ano,
            modalidade: dto.Modalidade,
            codigoEol: dto.CodigoEol
        );

        var id = await _repositorio.SalvarAsync(entidade);

        var entidadeSalva = await _repositorio.ObterPorIdAsync(id);
        return MapearParaDto(entidadeSalva!);
    }

    public async Task<ComponenteCurricularDto> AtualizarAsync(int id, AtualizarComponenteCurricularDto dto)
    {
        var validationResult = await _validatorAtualizar.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var entidade = await _repositorio.ObterPorIdAsync(id);
        if (entidade == null)
        {
            throw new NegocioException(
                $"Componente curricular com ID {id} não encontrado",
                HttpStatusCode.NotFound
            );
        }

        var existe = await _repositorio.ExisteComCodigoEolAsync(dto.CodigoEol, id);
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

        await _repositorio.SalvarAsync(entidadeAtualizada);

        var entidadeSalva = await _repositorio.ObterPorIdAsync(id);
        return MapearParaDto(entidadeSalva!);
    }

    public async Task<ComponenteCurricularDto?> ObterPorIdAsync(int id)
    {
        var entidade = await _repositorio.ObterPorIdAsync(id);
        return entidade != null ? MapearParaDto(entidade) : null;
    }

    public async Task<IEnumerable<ComponenteCurricularDto>> ListarAsync()
    {
        var entidades = await _repositorio.ListarAsync();
        return entidades.Select(MapearParaDto);
    }

    public async Task<bool> ExcluirAsync(int id)
    {
        var resultado = await _repositorio.RemoverLogico(id);
        return resultado > 0;
    }

    public async Task<ComponenteCurricularDto?> ObterPorCodigoEolAsync(int codigoEol)
    {
        var entidade = await _repositorio.ObterPorCodigoEolAsync(codigoEol);
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