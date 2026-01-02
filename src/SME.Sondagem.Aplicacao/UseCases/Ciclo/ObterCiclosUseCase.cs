using SME.Sondagem.Aplicacao.Interfaces.Ciclo;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Ciclo;

namespace SME.Sondagem.Aplicacao.UseCases.Ciclo;

public class ObterCiclosUseCase : IObterCiclosUseCase
{
    private readonly IRepositorioCiclo cicloRepositorio;

    public ObterCiclosUseCase(IRepositorioCiclo cicloRepositorio)
    {
        this.cicloRepositorio = cicloRepositorio;
    }

    public async Task<IEnumerable<CicloDto>> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var ciclos = await cicloRepositorio.ObterTodosAsync(cancellationToken: cancellationToken);

        return ciclos.Select(p => new CicloDto
        {
            Id = p.Id,
            CodCicloEnsinoEol = p.CodCicloEnsinoEol,
            DescCiclo = p.DescCiclo,
            CriadoEm = p.CriadoEm,
            CriadoPor = p.CriadoPor,
            CriadoRF = p.CriadoRF,
            AlteradoEm = p.AlteradoEm,
            AlteradoPor = p.AlteradoPor,
            AlteradoRF = p.AlteradoRF
        });
    }
}