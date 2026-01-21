using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem;

public class ObterSondagemPorIdUseCase : IObterSondagemPorIdUseCase
{
    private readonly IRepositorioSondagem sondagemRepositorio;

    public ObterSondagemPorIdUseCase(IRepositorioSondagem sondagemRepositorio)
    {
        this.sondagemRepositorio = sondagemRepositorio;
    }

    public async Task<SondagemDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var sondagem = await sondagemRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (sondagem == null)
            return null;

        return new SondagemDto
        {
            Id = sondagem.Id,
            DataAplicacao = sondagem.DataAplicacao,
            Descricao = sondagem.Descricao,
            CriadoEm = sondagem.CriadoEm,
            CriadoPor = sondagem.CriadoPor,
            CriadoRF = sondagem.CriadoRF,
            AlteradoEm = sondagem.AlteradoEm,
            AlteradoPor = sondagem.AlteradoPor,
            AlteradoRF = sondagem.AlteradoRF
        };
    }
}