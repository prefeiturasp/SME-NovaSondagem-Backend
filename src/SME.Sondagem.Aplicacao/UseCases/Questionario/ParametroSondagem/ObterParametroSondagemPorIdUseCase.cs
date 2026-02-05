using SME.Sondagem.Aplicacao.Interfaces.ParametroSondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.ParametroSondagem;

public class ObterParametroSondagemPorIdUseCase : IObterParametroSondagemPorIdUseCase
{
    private readonly IRepositorioParametroSondagem parametroSondagemRepositorio;

    public ObterParametroSondagemPorIdUseCase(IRepositorioParametroSondagem parametroSondagemRepositorio)
    {
        this.parametroSondagemRepositorio = parametroSondagemRepositorio;
    }

    public async Task<ParametroSondagemDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var p = await parametroSondagemRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (p == null)
            return null;

        return new ParametroSondagemDto
        {
            Id = p.Id,
            Nome = p.Nome,
            Descricao = p.Descricao,
            Ativo = p.Ativo,
            Tipo = p.Tipo,
            CriadoEm = p.CriadoEm,
            CriadoPor = p.CriadoPor,
            CriadoRF = p.CriadoRF,
            AlteradoEm = p.AlteradoEm,
            AlteradoPor = p.AlteradoPor,
            AlteradoRF = p.AlteradoRF
        };
    }
}