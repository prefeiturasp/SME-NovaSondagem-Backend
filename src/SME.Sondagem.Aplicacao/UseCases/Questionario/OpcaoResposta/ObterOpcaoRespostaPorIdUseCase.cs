using SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.OpcaoResposta;

public class ObterOpcaoRespostaPorIdUseCase : IObterOpcaoRespostaPorIdUseCase
{
    private readonly IRepositorioOpcaoResposta opcaoRespostaRepositorio;

    public ObterOpcaoRespostaPorIdUseCase(IRepositorioOpcaoResposta opcaoRespostaRepositorio)
    {
        this.opcaoRespostaRepositorio = opcaoRespostaRepositorio;
    }

    public async Task<OpcaoRespostaDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var opcaoResposta = await opcaoRespostaRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (opcaoResposta == null)
            return null;

        return new OpcaoRespostaDto
        {
            Id = opcaoResposta.Id,
            Ordem = opcaoResposta.Ordem,
            DescricaoOpcaoResposta = opcaoResposta.DescricaoOpcaoResposta,
            Legenda = opcaoResposta.Legenda,
            CorFundo = opcaoResposta.CorFundo,
            CorTexto = opcaoResposta.CorTexto,
            CriadoEm = opcaoResposta.CriadoEm,
            CriadoPor = opcaoResposta.CriadoPor,
            CriadoRF = opcaoResposta.CriadoRF,
            AlteradoEm = opcaoResposta.AlteradoEm,
            AlteradoPor = opcaoResposta.AlteradoPor,
            AlteradoRF = opcaoResposta.AlteradoRF
        };
    }
}