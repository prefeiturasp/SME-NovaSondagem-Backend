using SME.Sondagem.Aplicacao.Interfaces.ParametroSondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.ParametroSondagem;

public class ObterParametrosSondagemUseCase : IObterParametrosSondagemUseCase
{
    private readonly IRepositorioParametroSondagem parametroSondagemRepositorio;

    public ObterParametrosSondagemUseCase(IRepositorioParametroSondagem parametroSondagemRepositorio)
    {
        this.parametroSondagemRepositorio = parametroSondagemRepositorio;
    }

    public async Task<IEnumerable<ParametroSondagemDto>> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var parametroSondagems = await parametroSondagemRepositorio.ListarAsync(cancellationToken: cancellationToken);

        return parametroSondagems.Select(p => new ParametroSondagemDto
        {
            Id = p.Id,
            Nome = p.Nome,
            Descricao = p.Descricao,
            Ativo = p.Ativo,
            Tipo = p.Tipo
        });
    }
}