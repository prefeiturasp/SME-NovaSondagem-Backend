using SME.Sondagem.Aplicacao.Interfaces.Bimestre;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Bimestre;

namespace SME.Sondagem.Aplicacao.UseCases.Bimestre;

public class CriarBimestreUseCase : ICriarBimestreUseCase
{
    private readonly IRepositorioBimestre bimestreRepositorio;

    public CriarBimestreUseCase(IRepositorioBimestre bimestreRepositorio)
    {
        this.bimestreRepositorio = bimestreRepositorio;
    }

    public async Task<long> ExecutarAsync(BimestreDto bimestreDto, CancellationToken cancellationToken = default)
    {
        var bimestre = new Dominio.Entidades.Bimestre(
            bimestreDto.CodBimestreEnsinoEol,
            bimestreDto.Descricao);

        return await bimestreRepositorio.SalvarAsync(bimestre, cancellationToken: cancellationToken);
    }
}