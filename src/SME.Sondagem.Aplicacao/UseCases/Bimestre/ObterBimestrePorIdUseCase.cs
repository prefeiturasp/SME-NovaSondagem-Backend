using SME.Sondagem.Aplicacao.Interfaces.Bimestre;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Bimestre;

namespace SME.Sondagem.Aplicacao.UseCases.Bimestre;

public class ObterBimestrePorIdUseCase : IObterBimestrePorIdUseCase
{
    private readonly IRepositorioBimestre bimestreRepositorio;

    public ObterBimestrePorIdUseCase(IRepositorioBimestre bimestreRepositorio)
    {
        this.bimestreRepositorio = bimestreRepositorio;
    }

    public async Task<BimestreDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var bimestre = await bimestreRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (bimestre == null)
            return null;

        return new BimestreDto
        {
            Id = bimestre.Id,
            Descricao = bimestre.Descricao,
            CodBimestreEnsinoEol = bimestre.CodBimestreEnsinoEol,
            CriadoEm = bimestre.CriadoEm,
            CriadoPor = bimestre.CriadoPor,
            CriadoRF = bimestre.CriadoRF,
            AlteradoEm = bimestre.AlteradoEm,
            AlteradoPor = bimestre.AlteradoPor,
            AlteradoRF = bimestre.AlteradoRF
        };
    }
}