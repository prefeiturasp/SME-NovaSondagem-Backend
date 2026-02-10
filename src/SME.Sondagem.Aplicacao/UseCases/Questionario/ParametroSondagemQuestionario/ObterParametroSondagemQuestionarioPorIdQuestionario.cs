using SME.Sondagem.Aplicacao.Interfaces.ParametroSondagemQuestionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.ParametroSondagemQuestionario;

public class ObterParametroSondagemQuestionarioPorIdQuestionarioUseCase : IObterParametroSondagemQuestionarioPorIdQuestionarioUseCase
{
    private readonly IRepositorioParametroSondagemQuestionario parametroSondagemQuestionarioRepositorio;
    private readonly IRepositorioCache repositorioCache;

    public ObterParametroSondagemQuestionarioPorIdQuestionarioUseCase(
        IRepositorioParametroSondagemQuestionario parametroSondagemQuestionarioRepositorio,
        IRepositorioCache repositorioCache)
    {
        this.parametroSondagemQuestionarioRepositorio = parametroSondagemQuestionarioRepositorio;
        this.repositorioCache = repositorioCache;
    }

    public async Task<IEnumerable<ParametroSondagemQuestionarioCompletoDto>> ExecutarAsync(long idQuestionario, CancellationToken cancellationToken = default)
    {
        var chaveCache = $"parametros-sondagem-questionario-{idQuestionario}";

        return await repositorioCache.ObterRedisAsync(chaveCache, async () =>
        {
            var parametros = await parametroSondagemQuestionarioRepositorio.ObterPorIdQuestionarioAsync(idQuestionario, cancellationToken);

            return parametros.Select(p => new ParametroSondagemQuestionarioCompletoDto
            {
                Id = p.Id,
                IdQuestionario = p.IdQuestionario,
                Valor = p.Valor,
                Tipo = Enum.GetName(typeof(TipoParametroSondagem), p.IdParametroSondagem)
            });
        });
    }
}