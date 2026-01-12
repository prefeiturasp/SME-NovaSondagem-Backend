using SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.OpcaoResposta;

public class CriarOpcaoRespostaUseCase : ICriarOpcaoRespostaUseCase
{
    private readonly IRepositorioOpcaoResposta opcaoRespostaRepositorio;

    public CriarOpcaoRespostaUseCase(IRepositorioOpcaoResposta opcaoRespostaRepositorio)
    {
        this.opcaoRespostaRepositorio = opcaoRespostaRepositorio;
    }

    public async Task<long> ExecutarAsync(OpcaoRespostaDto opcaoRespostaDto, CancellationToken cancellationToken = default)
    {
        var opcaoResposta = new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
            opcaoRespostaDto.Ordem,
            opcaoRespostaDto.DescricaoOpcaoResposta,
            opcaoRespostaDto.Legenda,
            opcaoRespostaDto.CorFundo,
            opcaoRespostaDto.CorTexto);

        return await opcaoRespostaRepositorio.CriarAsync(opcaoResposta, cancellationToken: cancellationToken);
    }
}