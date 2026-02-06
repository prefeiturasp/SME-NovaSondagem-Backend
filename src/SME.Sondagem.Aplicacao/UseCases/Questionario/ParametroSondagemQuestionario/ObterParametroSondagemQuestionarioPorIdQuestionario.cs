using SME.Sondagem.Aplicacao.Interfaces.ParametroSondagemQuestionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.ParametroSondagemQuestionario;

public class ObterParametroSondagemQuestionarioPorIdQuestionarioUseCase : IObterParametroSondagemQuestionarioPorIdQuestionarioUseCase
{
    private readonly IRepositorioParametroSondagemQuestionario parametroSondagemQuestionarioRepositorio;

    public ObterParametroSondagemQuestionarioPorIdQuestionarioUseCase(IRepositorioParametroSondagemQuestionario parametroSondagemQuestionarioRepositorio)
    {
        this.parametroSondagemQuestionarioRepositorio = parametroSondagemQuestionarioRepositorio;
    }

    public async Task<IEnumerable<ParametroSondagemQuestionarioCompletoDto>> ExecutarAsync(long idQuestionario, CancellationToken cancellationToken = default)
    {
        var parametros = await parametroSondagemQuestionarioRepositorio.ObterPorIdQuestionarioAsync(idQuestionario, cancellationToken);

        return parametros.Select(p => new ParametroSondagemQuestionarioCompletoDto
        {
            Id = p.Id,
            IdQuestionario = p.IdQuestionario,
            Valor = p.Valor,
            Tipo = (int)p.ParametroSondagem.Tipo
        });
    }
}