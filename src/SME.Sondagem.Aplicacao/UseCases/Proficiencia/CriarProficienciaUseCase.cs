using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Proficiencia;

namespace SME.Sondagem.Aplicacao.UseCases.Proficiencia;

public class CriarProficienciaUseCase : ICriarProficienciaUseCase
{
    private readonly IRepositorioProficiencia proficienciaRepositorio;

    public CriarProficienciaUseCase(IRepositorioProficiencia proficienciaRepositorio)
    {
        this.proficienciaRepositorio = proficienciaRepositorio;
    }

    public async Task<long> ExecutarAsync(ProficienciaDto proficienciaDto, CancellationToken cancellationToken = default)
    {
        var proficiencia = new SME.Sondagem.Dominio.Entidades.Proficiencia(
            proficienciaDto.Nome,
            proficienciaDto.ComponenteCurricularId);

        return await proficienciaRepositorio.CriarAsync(proficiencia, cancellationToken: cancellationToken);
    }
}