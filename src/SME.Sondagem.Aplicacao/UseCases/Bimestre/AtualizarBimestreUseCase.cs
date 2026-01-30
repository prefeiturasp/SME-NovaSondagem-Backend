using SME.Sondagem.Aplicacao.Interfaces.Bimestre;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Bimestre;

namespace SME.Sondagem.Aplicacao.UseCases.Bimestre;

public class AtualizarBimestreUseCase : IAtualizarBimestreUseCase
{
    private readonly IRepositorioBimestre bimestreRepositorio;

    public AtualizarBimestreUseCase(IRepositorioBimestre bimestreRepositorio)
    {
        this.bimestreRepositorio = bimestreRepositorio;
    }

    public async Task<BimestreDto?> ExecutarAsync(long id, BimestreDto bimestreDto, CancellationToken cancellationToken = default)
    {
        var bimestreExistente = await bimestreRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (bimestreExistente == null)
            return null;

        bimestreExistente.Atualizar(bimestreDto.Descricao, bimestreDto.CodBimestreEnsinoEol);

        var sucesso = await bimestreRepositorio.SalvarAsync(bimestreExistente, cancellationToken: cancellationToken);
        
        if (sucesso == 0)
            return null;

        return new BimestreDto
        {
            Id = bimestreExistente.Id,
            Descricao = bimestreExistente.Descricao,
            CodBimestreEnsinoEol = bimestreExistente.CodBimestreEnsinoEol
        };
    }
}