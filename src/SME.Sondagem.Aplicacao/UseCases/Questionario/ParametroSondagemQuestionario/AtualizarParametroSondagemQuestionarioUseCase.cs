using SME.Sondagem.Aplicacao.Interfaces.ParametroSondagemQuestionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.ParametroSondagemQuestionario;

public class AtualizarParametroSondagemQuestionarioUseCase : IAtualizarParametroSondagemQuestionarioUseCase
{
    private readonly IRepositorioParametroSondagemQuestionario parametroSondagemQuestionarioRepositorio;

    public AtualizarParametroSondagemQuestionarioUseCase(IRepositorioParametroSondagemQuestionario parametroSondagemQuestionarioRepositorio)
    {
        this.parametroSondagemQuestionarioRepositorio = parametroSondagemQuestionarioRepositorio;
    }

    public async Task<ParametroSondagemQuestionarioDto?> ExecutarAsync(long id, ParametroSondagemQuestionarioDto parametroSondagemQuestionarioDto, CancellationToken cancellationToken = default)
    {
        var parametroSondagemQuestionario = await parametroSondagemQuestionarioRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (parametroSondagemQuestionario == null)
            return null;

        parametroSondagemQuestionario.Atualizar(
            parametroSondagemQuestionarioDto.IdParametroSondagem, 
            parametroSondagemQuestionarioDto.IdQuestionario, 
            parametroSondagemQuestionarioDto.Valor);

        var sucessoId = await parametroSondagemQuestionarioRepositorio.SalvarAsync(parametroSondagemQuestionario, cancellationToken: cancellationToken);
        
        if (sucessoId == 0)
            return null;

        return new ParametroSondagemQuestionarioDto
        {
            Id = parametroSondagemQuestionario.Id,
            IdParametroSondagem = parametroSondagemQuestionario.IdParametroSondagem,
            IdQuestionario = parametroSondagemQuestionario.IdQuestionario,
            Valor = parametroSondagemQuestionario.Valor
        };
    }
}