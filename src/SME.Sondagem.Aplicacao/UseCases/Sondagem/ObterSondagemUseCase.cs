using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem;

public class ObterSondagemUseCase : IObterSondagensUseCase
{
    private readonly IRepositorioSondagem sondagemRepositorio;

    public ObterSondagemUseCase(IRepositorioSondagem sondagemRepositorio)
    {
        this.sondagemRepositorio = sondagemRepositorio;
    }

    public async Task<IEnumerable<SondagemDto>> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var questoes = await sondagemRepositorio.ObterTodosAsync(cancellationToken: cancellationToken);

        return questoes.Select(p => new SondagemDto
        {
            Id = p.Id,
            DataAplicacao = p.DataAplicacao,
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