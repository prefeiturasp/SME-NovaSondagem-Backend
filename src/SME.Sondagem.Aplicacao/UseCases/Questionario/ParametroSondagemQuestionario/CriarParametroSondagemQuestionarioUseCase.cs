using SME.Sondagem.Aplicacao.Interfaces.ParametroSondagemQuestionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.ParametroSondagemQuestionario;

public class CriarParametroSondagemQuestionarioUseCase : ICriarParametroSondagemQuestionarioUseCase
{
    private readonly IRepositorioParametroSondagemQuestionario parametroSondagemQuestionarioRepositorio;

    public CriarParametroSondagemQuestionarioUseCase(IRepositorioParametroSondagemQuestionario parametroSondagemQuestionarioRepositorio)
    {
        this.parametroSondagemQuestionarioRepositorio = parametroSondagemQuestionarioRepositorio;
    }

    public async Task<long> ExecutarAsync(ParametroSondagemQuestionarioDto parametroSondagemQuestionarioDto, CancellationToken cancellationToken = default)
    {
        var parametroSondagemQuestionario = new SME.Sondagem.Dominio.Entidades.ParametroSondagemQuestionario();
        parametroSondagemQuestionario.IdParametroSondagem = parametroSondagemQuestionarioDto.IdParametroSondagem;
        parametroSondagemQuestionario.IdQuestionario = parametroSondagemQuestionarioDto.IdQuestionario;
        parametroSondagemQuestionario.Valor = parametroSondagemQuestionarioDto.Valor;

        return await parametroSondagemQuestionarioRepositorio.SalvarAsync(parametroSondagemQuestionario, cancellationToken: cancellationToken);
    }
}