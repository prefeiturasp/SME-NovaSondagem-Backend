using SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.OpcaoResposta;

public class ObterOpcaoRespostasUseCase : IObterOpcaoRespostaUseCase
{
    private readonly IRepositorioOpcaoResposta opcaoRespostaRepositorio;

    public ObterOpcaoRespostasUseCase(IRepositorioOpcaoResposta opcaoRespostaRepositorio)
    {
        this.opcaoRespostaRepositorio = opcaoRespostaRepositorio;
    }

    public async Task<IEnumerable<OpcaoRespostaDto>> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var opcaoRespostas = await opcaoRespostaRepositorio.ObterTodosAsync(cancellationToken: cancellationToken);

        return opcaoRespostas.Select(p => new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = p.DescricaoOpcaoResposta,
            Legenda = p.Legenda,
            CorFundo = p.CorFundo,
            CorTexto = p.CorTexto,
            CriadoEm = p.CriadoEm,
            CriadoPor = p.CriadoPor,
            CriadoRF = p.CriadoRF,
            AlteradoEm = p.AlteradoEm,
            AlteradoPor = p.AlteradoPor,
            AlteradoRF = p.AlteradoRF
        });
    }
}