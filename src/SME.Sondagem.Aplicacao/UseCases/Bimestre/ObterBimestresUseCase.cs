using SME.Sondagem.Aplicacao.Interfaces.Bimestre;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Dominio.Strategies.Bimestre;
using SME.Sondagem.Infrastructure.Dtos.Bimestre;

namespace SME.Sondagem.Aplicacao.UseCases.Bimestre;

public class ObterBimestresUseCase : IObterBimestresUseCase
{
    private readonly IRepositorioBimestre bimestreRepositorio;

    public ObterBimestresUseCase(IRepositorioBimestre bimestreRepositorio)
    {
        this.bimestreRepositorio = bimestreRepositorio;
    }

    public async Task<IEnumerable<BimestreDto>> ExecutarAsync(int modalidade, int? semestre, CancellationToken cancellationToken = default)
    {
        var bimestres = await bimestreRepositorio.ListarAsync(cancellationToken: cancellationToken);

        var ehEja = modalidade == (int)Modalidade.EJA;

        if (ehEja && semestre is not 1 and not 2)
            throw new RegraNegocioException(MensagemNegocioComuns.SEMESTRE_OBRIGATORIO_EJA, 400);

        var ehSegundoSemestreEja = ehEja && semestre == 2;

        if (ehEja)
        {
            int[] bimestresEja = ehSegundoSemestreEja ? [4, 5] : [2, 3];
            bimestres = [.. bimestres.Where(b => bimestresEja.Contains(b.Id))];
        }

        return bimestres.Select(p => new BimestreDto
        {
            Id = p.Id,
            CodBimestreEnsinoEol = p.CodBimestreEnsinoEol,
            Descricao = ehSegundoSemestreEja ? BimestreModalidadeEjaStrategy.RenomearSegundoSemestre(p.Id, p.Descricao) : p.Descricao,
            CriadoEm = p.CriadoEm,
            CriadoPor = p.CriadoPor,
            CriadoRF = p.CriadoRF,
            AlteradoEm = p.AlteradoEm,
            AlteradoPor = p.AlteradoPor,
            AlteradoRF = p.AlteradoRF
        });
    }
}