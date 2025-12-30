using SME.Sondagem.Aplicacao.Interfaces.Ciclo;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Ciclo;

namespace SME.Sondagem.Aplicacao.UseCases.Ciclo;

public class ObterCicloPorIdUseCase : IObterCicloPorIdUseCase
{
    private readonly IRepositorioCiclo cicloRepositorio;

    public ObterCicloPorIdUseCase(IRepositorioCiclo cicloRepositorio)
    {
        this.cicloRepositorio = cicloRepositorio;
    }

    public async Task<CicloDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var ciclo = await cicloRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (ciclo == null)
            return null;

        return new CicloDto
        {
            Id = ciclo.Id,
            DescCiclo = ciclo.DescCiclo,
            CodCicloEnsinoEol = ciclo.CodCicloEnsinoEol,
            CriadoEm = ciclo.CriadoEm,
            CriadoPor = ciclo.CriadoPor,
            CriadoRF = ciclo.CriadoRF,
            AlteradoEm = ciclo.AlteradoEm,
            AlteradoPor = ciclo.AlteradoPor,
            AlteradoRF = ciclo.AlteradoRF
        };
    }
}