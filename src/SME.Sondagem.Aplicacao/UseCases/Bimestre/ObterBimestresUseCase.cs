using SME.Sondagem.Aplicacao.Interfaces.Bimestre;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infrastructure.Dtos.Bimestre;

namespace SME.Sondagem.Aplicacao.UseCases.Bimestre;

public class ObterBimestresUseCase : IObterBimestresUseCase
{
    private readonly IRepositorioBimestre bimestreRepositorio;

    public ObterBimestresUseCase(IRepositorioBimestre bimestreRepositorio)
    {
        this.bimestreRepositorio = bimestreRepositorio;
    }

    public async Task<IEnumerable<BimestreDto>> ExecutarAsync(int modalidade, CancellationToken cancellationToken = default)
    {
        var bimestres = await bimestreRepositorio.ListarAsync(cancellationToken: cancellationToken);


        if (modalidade == (int)Modalidade.EJA)
        {
            int[] bimestresEja = [2, 3];
            bimestres = [.. bimestres.Where(b => bimestresEja.Contains(b.Id))];
        }

        return bimestres.Select(p => new BimestreDto
        {
            Id = p.Id,
            CodBimestreEnsinoEol = p.CodBimestreEnsinoEol,
            Descricao = p.Descricao,
            CriadoEm = p.CriadoEm,
            CriadoPor = p.CriadoPor,
            CriadoRF = p.CriadoRF,
            AlteradoEm = p.AlteradoEm,
            AlteradoPor = p.AlteradoPor,
            AlteradoRF = p.AlteradoRF
        });
    }
}