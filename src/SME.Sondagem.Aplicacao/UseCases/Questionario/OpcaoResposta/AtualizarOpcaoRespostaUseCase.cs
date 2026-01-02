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

        opcaoRespostaExistente.Atualizar(opcaoRespostaDto.DescricaoOpcaoResposta, opcaoRespostaDto.Legenda, opcaoRespostaDto.CorFundo, opcaoRespostaDto.CorTexto);
        opcaoRespostaExistente.AlteradoEm = DateTime.Now;
        opcaoRespostaExistente.AlteradoPor = opcaoRespostaDto.AlteradoPor;
        opcaoRespostaExistente.AlteradoRF = opcaoRespostaDto.AlteradoRF;

        var sucesso = await opcaoRespostaRepositorio.AtualizarAsync(opcaoRespostaExistente, cancellationToken: cancellationToken);
        
        if (!sucesso)
            return null;

        return new OpcaoRespostaDto
        {
            DescricaoOpcaoResposta = opcaoRespostaExistente.DescricaoOpcaoResposta,
            Legenda = opcaoRespostaExistente.Legenda,
            CorFundo = opcaoRespostaExistente.CorFundo,
            CorTexto = opcaoRespostaExistente.CorTexto,
            CriadoEm = opcaoRespostaExistente.CriadoEm,
            CriadoPor = opcaoRespostaExistente.CriadoPor,
            CriadoRF = opcaoRespostaExistente.CriadoRF,
            AlteradoEm = opcaoRespostaExistente.AlteradoEm,
            AlteradoPor = opcaoRespostaExistente.AlteradoPor,
            AlteradoRF = opcaoRespostaExistente.AlteradoRF
        };
    }
}