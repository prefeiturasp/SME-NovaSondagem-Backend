using SME.Sondagem.Aplicacao.Interfaces.ParametroSondagemQuestionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.ParametroSondagemQuestionario;

public class ObterParametroSondagemQuestionarioPorIdUseCase : IObterParametroSondagemQuestionarioPorIdUseCase
{
    private readonly IRepositorioParametroSondagemQuestionario parametroSondagemQuestionarioRepositorio;

    public ObterParametroSondagemQuestionarioPorIdUseCase(IRepositorioParametroSondagemQuestionario parametroSondagemQuestionarioRepositorio)
    {
        this.parametroSondagemQuestionarioRepositorio = parametroSondagemQuestionarioRepositorio;
    }

    public async Task<ParametroSondagemQuestionarioDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var p = await parametroSondagemQuestionarioRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (p == null)
            return null;

        return new ParametroSondagemQuestionarioDto
        {
            Id = p.Id,
            IdParametroSondagem = p.IdParametroSondagem,
            IdQuestionario = p.IdQuestionario,
            Valor = p.Valor
        };
    }
}