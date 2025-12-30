using SME.Sondagem.Aplicacao.Interfaces.Ciclo;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Ciclo;

namespace SME.Sondagem.Aplicacao.UseCases.Ciclo;

public class CriarCicloUseCase : ICriarCicloUseCase
{
    private readonly IRepositorioCiclo cicloRepositorio;

    public CriarCicloUseCase(IRepositorioCiclo cicloRepositorio)
    {
        this.cicloRepositorio = cicloRepositorio;
    }

    public async Task<long> ExecutarAsync(CicloDto cicloDto, CancellationToken cancellationToken = default)
    {
        var ciclo = new SME.Sondagem.Dominio.Entidades.Ciclo(
            cicloDto.CodCicloEnsinoEol,
            cicloDto.DescCiclo);

        return await cicloRepositorio.CriarAsync(ciclo, cancellationToken: cancellationToken);
    }
}