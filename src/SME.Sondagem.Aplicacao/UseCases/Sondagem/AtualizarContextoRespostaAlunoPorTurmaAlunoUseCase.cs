using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem;

public class AtualizarContextoRespostaAlunoPorTurmaAlunoUseCase : IAtualizarContextoRespostaAlunoPorTurmaAlunoUseCase
{
    private readonly IRepositorioRespostaAluno _repositorioRespostaAluno;
    private readonly IDadosAlunosService _dadosAlunosService;
    private readonly IUeComDreEolService _ueComDreEolService;
    private readonly RepositoriosElastic _repositoriosElastic;

    public AtualizarContextoRespostaAlunoPorTurmaAlunoUseCase(
        IRepositorioRespostaAluno repositorioRespostaAluno,
        IDadosAlunosService dadosAlunosService,
        IUeComDreEolService ueComDreEolService,
        RepositoriosElastic repositoriosElastic)
    {
        _repositorioRespostaAluno = repositorioRespostaAluno;
        _dadosAlunosService = dadosAlunosService;
        _ueComDreEolService = ueComDreEolService;
        _repositoriosElastic = repositoriosElastic;
    }

    public async Task<AtualizacaoContextoRespostaPorTurmaAlunoOutput> ExecutarAsync(int codigoTurma, int codigoAluno,
        int? anoLetivo, CancellationToken cancellationToken)
    {
        if (codigoTurma <= 0 || codigoAluno <= 0)
            return new AtualizacaoContextoRespostaPorTurmaAlunoOutput { Sucesso = false, LinhasAtualizadas = 0 };

        var dadosCompletosTurmas =
            (await _repositoriosElastic.RepositorioElasticTurma.ObterTurmasPorIds([codigoTurma], cancellationToken))
            .Where(t => t.CodigoTurma == codigoTurma)
            .ToList();

        if (dadosCompletosTurmas.Count == 0)
            return new AtualizacaoContextoRespostaPorTurmaAlunoOutput { Sucesso = false, LinhasAtualizadas = 0 };

        var turmaElastic = SelecionarTurma(dadosCompletosTurmas, anoLetivo);
        if (turmaElastic == null)
            return new AtualizacaoContextoRespostaPorTurmaAlunoOutput { Sucesso = false, LinhasAtualizadas = 0 };

        var codigoEscolaTurma = turmaElastic.CodigoEscola;
        var uesComDre = await _ueComDreEolService.ObterUesComDrePorCodigosUes([codigoEscolaTurma], cancellationToken);
        var ueComDre = uesComDre.FirstOrDefault(u =>
            string.Equals(u.CodigoEscola, codigoEscolaTurma, StringComparison.OrdinalIgnoreCase));

        var racaGenero = await _dadosAlunosService.ObterDadosRacaGeneroAlunosPorCodigoAluno(codigoAluno, cancellationToken);

        var linhas = await _repositorioRespostaAluno.AtualizarContextoEducacionalPorAlunoIdAsync(
            codigoAluno,
            turmaElastic.CodigoTurma.ToString(),
            codigoEscolaTurma,
            ueComDre?.CodigoDRE ?? string.Empty,
            int.TryParse(turmaElastic.AnoTurma, out var anoTurma) ? anoTurma : null,
            racaGenero?.RacaId,
            racaGenero?.SexoId,
            cancellationToken);

        return new AtualizacaoContextoRespostaPorTurmaAlunoOutput { Sucesso = true, LinhasAtualizadas = linhas };
    }

    private static TurmaElasticDto? SelecionarTurma(IReadOnlyList<TurmaElasticDto> turmas, int? anoLetivoFiltro)
    {
        if (turmas.Count == 0)
            return null;

        if (anoLetivoFiltro.HasValue)
            return turmas.FirstOrDefault(t => t.AnoLetivo == anoLetivoFiltro.Value);

        var anoAtual = DateTime.Now.Year;
        return turmas.FirstOrDefault(t => t.AnoLetivo == anoAtual)
               ?? turmas.OrderByDescending(t => t.AnoLetivo).First();
    }
}
