using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem;

public class CriarSondagemUseCase : ICriarSondagemUseCase
{
    private readonly IRepositorioSondagem sondagemRepositorio;

    public CriarSondagemUseCase(IRepositorioSondagem sondagemRepositorio)
    {
        this.sondagemRepositorio = sondagemRepositorio;
    }

    public async Task<long> ExecutarAsync(SondagemDto sondagemDto, CancellationToken cancellationToken = default)
    {
        var sondagem = new SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem(
            descricao: sondagemDto.Descricao,
            dataAplicacao: sondagemDto.DataAplicacao
        );

        await sondagemRepositorio.SalvarAsync(sondagem, cancellationToken: cancellationToken);
        return sondagem.Id;
    }
}