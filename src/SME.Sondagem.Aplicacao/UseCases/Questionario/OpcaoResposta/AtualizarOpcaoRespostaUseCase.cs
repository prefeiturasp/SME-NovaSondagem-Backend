using SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.OpcaoResposta;

public class AtualizarOpcaoRespostaUseCase : IAtualizarOpcaoRespostaUseCase
{
    private readonly IRepositorioOpcaoResposta opcaoRespostaRepositorio;

    public AtualizarOpcaoRespostaUseCase(IRepositorioOpcaoResposta opcaoRespostaRepositorio)
    {
        this.opcaoRespostaRepositorio = opcaoRespostaRepositorio;
    }

    public async Task<OpcaoRespostaDto?> ExecutarAsync(long id, OpcaoRespostaDto opcaoRespostaDto, CancellationToken cancellationToken = default)
    {
        var opcaoRespostaExistente = await opcaoRespostaRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (opcaoRespostaExistente == null)
            return null;

        opcaoRespostaExistente.Atualizar(opcaoRespostaDto.Ordem, opcaoRespostaDto.DescricaoOpcaoResposta, opcaoRespostaDto.Legenda, opcaoRespostaDto.CorFundo, opcaoRespostaDto.CorTexto);

        var sucessoId = await opcaoRespostaRepositorio.SalvarAsync(opcaoRespostaExistente, cancellationToken: cancellationToken);
        
        if (sucessoId == 0)
            return null;

        return new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = opcaoRespostaExistente.DescricaoOpcaoResposta,
            Legenda = opcaoRespostaExistente.Legenda,
            CorFundo = opcaoRespostaExistente.CorFundo,
            CorTexto = opcaoRespostaExistente.CorTexto
        };
    }
}