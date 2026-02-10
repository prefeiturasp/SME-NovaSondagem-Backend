using SME.Sondagem.Aplicacao.Interfaces.ParametroSondagemQuestionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.ParametroSondagemQuestionario;

public class ObterParametrosSondagemQuestionarioUseCase : IObterParametrosSondagemQuestionarioUseCase
{
    private readonly IRepositorioParametroSondagemQuestionario parametroSondagemRepositorio;

    public ObterParametrosSondagemQuestionarioUseCase(IRepositorioParametroSondagemQuestionario parametroSondagemRepositorio)
    {
        this.parametroSondagemRepositorio = parametroSondagemRepositorio;
    }

    public async Task<IEnumerable<ParametroSondagemQuestionarioDto>> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var parametroSondagems = await parametroSondagemRepositorio.ListarAsync(cancellationToken: cancellationToken);

        return parametroSondagems.Select(p => new ParametroSondagemQuestionarioDto
        {
            Id = p.Id,
            IdParametroSondagem = p.IdParametroSondagem,
            IdQuestionario = p.IdQuestionario,
            Valor = p.Valor
        });
    }
}