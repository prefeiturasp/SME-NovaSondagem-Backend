using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Proficiencia;

namespace SME.Sondagem.Aplicacao.UseCases.Proficiencia;

public class ObterProficienciasUseCase : IObterProficienciasUseCase
{
    private readonly IRepositorioProficiencia proficienciaRepositorio;

    public ObterProficienciasUseCase(IRepositorioProficiencia proficienciaRepositorio)
    {
        this.proficienciaRepositorio = proficienciaRepositorio;
    }

    public async Task<IEnumerable<ProficienciaDto>> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var proficiencias = await proficienciaRepositorio.ListarAsync(cancellationToken: cancellationToken);

        return proficiencias.Select(p => new ProficienciaDto
        {
            Id = p.Id,
            Nome = p.Nome,
            ComponenteCurricularId = p.ComponenteCurricularId,
            CriadoEm = p.CriadoEm,
            CriadoPor = p.CriadoPor,
            CriadoRF = p.CriadoRF,
            AlteradoEm = p.AlteradoEm,
            AlteradoPor = p.AlteradoPor,
            AlteradoRF = p.AlteradoRF
        });
    }
}