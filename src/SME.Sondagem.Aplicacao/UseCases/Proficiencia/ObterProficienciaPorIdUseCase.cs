using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Proficiencia;

namespace SME.Sondagem.Aplicacao.UseCases.Proficiencia;

public class ObterProficienciaPorIdUseCase : IObterProficienciaPorIdUseCase
{
    private readonly IRepositorioProficiencia proficienciaRepositorio;

    public ObterProficienciaPorIdUseCase(IRepositorioProficiencia proficienciaRepositorio)
    {
        this.proficienciaRepositorio = proficienciaRepositorio;
    }

    public async Task<ProficienciaDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var proficiencia = await proficienciaRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (proficiencia == null)
            return null;

        return new ProficienciaDto
        {
            Id = proficiencia.Id,
            Nome = proficiencia.Nome,
            ComponenteCurricularId = proficiencia.ComponenteCurricularId,
            CriadoEm = proficiencia.CriadoEm,
            CriadoPor = proficiencia.CriadoPor,
            CriadoRF = proficiencia.CriadoRF,
            AlteradoEm = proficiencia.AlteradoEm,
            AlteradoPor = proficiencia.AlteradoPor,
            AlteradoRF = proficiencia.AlteradoRF
        };
    }
}