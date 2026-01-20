using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem;

public class AtualizarSondagemUseCase : IAtualizarSondagemUseCase
{
    private readonly IRepositorioSondagem sondagemRepositorio;

    public AtualizarSondagemUseCase(IRepositorioSondagem sondagemRepositorio)
    {
        this.sondagemRepositorio = sondagemRepositorio;
    }

    public async Task<SondagemDto?> ExecutarAsync(long id, SondagemDto sondagemDto, CancellationToken cancellationToken = default)
    {
        var sondagemExistente = await sondagemRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (sondagemExistente == null)
            return null;

        sondagemExistente.AtualizarDescricao(sondagemDto.Descricao);
        sondagemExistente.AtualizarDataAplicacao(sondagemDto.DataAplicacao);

        var sucesso = await sondagemRepositorio.AtualizarAsync(sondagemExistente, cancellationToken: cancellationToken);
        
        if (!sucesso)
            return null;

        return new SondagemDto
        {
            Id = sondagemExistente.Id,
            Descricao = sondagemExistente.Descricao,
            DataAplicacao = sondagemExistente.DataAplicacao,
            CriadoEm = sondagemExistente.CriadoEm,
            CriadoPor = sondagemExistente.CriadoPor,
            CriadoRF = sondagemExistente.CriadoRF,
            AlteradoEm = sondagemExistente.AlteradoEm,
            AlteradoPor = sondagemExistente.AlteradoPor,
            AlteradoRF = sondagemExistente.AlteradoRF
        };
    }
}