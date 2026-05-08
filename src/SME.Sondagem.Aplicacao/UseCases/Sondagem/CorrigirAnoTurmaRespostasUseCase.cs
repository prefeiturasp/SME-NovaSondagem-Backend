using Microsoft.Extensions.Logging;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem;

public class CorrigirAnoTurmaRespostasUseCase : ICorrigirAnoTurmaRespostasUseCase
{
    private readonly IRepositorioRespostaAluno _repositorioRespostaAluno;
    private readonly IUeComDreEolService _ueComDreEolService;
    private readonly RepositoriosElastic _repositoriosElastic;
    private readonly ILogger<CorrigirAnoTurmaRespostasUseCase> _logger;

    public CorrigirAnoTurmaRespostasUseCase(
        IRepositorioRespostaAluno repositorioRespostaAluno,
        IUeComDreEolService ueComDreEolService,
        RepositoriosElastic repositoriosElastic,
        ILogger<CorrigirAnoTurmaRespostasUseCase> logger)
    {
        _repositorioRespostaAluno = repositorioRespostaAluno;
        _ueComDreEolService = ueComDreEolService;
        _repositoriosElastic = repositoriosElastic;
        _logger = logger;
    }

    public async Task<CorrigirAnoTurmaRespostasOutput> ExecutarAsync(
        int pagina, int tamanhoLote, CancellationToken cancellationToken)
    {
        var divergentes = (await _repositorioRespostaAluno
            .ObterRespostasComAnoTurmaDivergentePaginadoAsync(pagina, tamanhoLote, cancellationToken))
            .ToList();

        if (divergentes.Count == 0)
            return new CorrigirAnoTurmaRespostasOutput();

        var codigosAlunos = divergentes.Select(d => d.AlunoId).Distinct().ToList();
        var matriculasElastic = await ObterMatriculasAlunosAsync(codigosAlunos, cancellationToken);

        var turmasIds = matriculasElastic.Select(m => m.CodigoTurma).Where(c => c > 0).Distinct();
        var turmasElastic = (await _repositoriosElastic.RepositorioElasticTurma
            .ObterTurmasPorIds(turmasIds, cancellationToken))
            .ToList();

        var turmasPorCodigo = turmasElastic
            .Where(t => t.CodigoTurma > 0)
            .GroupBy(t => t.CodigoTurma)
            .ToDictionary(g => g.Key, g => g.First());

        var codigosUes = turmasElastic
            .Select(t => t.CodigoEscola)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct();

        var uesComDre = await _ueComDreEolService.ObterUesComDrePorCodigosUes(codigosUes, cancellationToken);
        var uesPorCodigo = uesComDre
            .Where(u => !string.IsNullOrEmpty(u.CodigoEscola))
            .ToDictionary(u => u.CodigoEscola!, u => u);

        var matriculasPorAluno = matriculasElastic
            .GroupBy(m => m.CodigoAluno)
            .ToDictionary(g => g.Key, g => g.ToList());

        var atualizacoes = new List<AtualizarContextoTurmaRespostaAlunoDto>();
        var semTurma = 0;

        foreach (var resposta in divergentes)
        {
            var atualizacao = MontarAtualizacao(resposta, matriculasPorAluno, turmasPorCodigo, uesPorCodigo);
            if (atualizacao != null)
            {
                atualizacoes.Add(atualizacao);
            }
            else
            {
                semTurma++;
                _logger.LogWarning(
                    "Turma não encontrada no Elastic para AlunoId={AlunoId}, SerieAno={SerieAno}, RespostaId={RespostaId}",
                    resposta.AlunoId, resposta.SerieAnoCorreto, resposta.Id);
            }
        }

        var corrigidos = 0;
        if (atualizacoes.Count > 0)
            corrigidos = await _repositorioRespostaAluno.AtualizarContextoTurmaLoteAsync(atualizacoes, cancellationToken);

        return new CorrigirAnoTurmaRespostasOutput
        {
            TotalDivergentes = divergentes.Count,
            TotalCorrigidos = corrigidos,
            TotalSemTurmaNoElastic = semTurma
        };
    }

    private async Task<List<AlunoElasticDto>> ObterMatriculasAlunosAsync(
        List<int> codigosAlunos, CancellationToken cancellationToken)
    {
        var matriculas = await _repositoriosElastic.RepositorioElasticAluno
            .ObterAlunosPorCodigosAlunos(codigosAlunos, cancellationToken);

        return matriculas.ToList();
    }

    private static AtualizarContextoTurmaRespostaAlunoDto? MontarAtualizacao(
        RespostaAlunoAnoTurmaDivergenteDto resposta,
        Dictionary<int, List<AlunoElasticDto>> matriculasPorAluno,
        Dictionary<int, TurmaElasticDto> turmasPorCodigo,
        Dictionary<string, Infrastructure.Dtos.UeComDreEolDto> uesPorCodigo)
    {
        if (!matriculasPorAluno.TryGetValue(resposta.AlunoId, out var matriculasAluno))
            return null;

        var turmaCorreta = EncontrarTurmaCorrespondente(
            matriculasAluno, turmasPorCodigo, resposta.SerieAnoCorreto);

        if (turmaCorreta == null)
            return null;

        uesPorCodigo.TryGetValue(turmaCorreta.CodigoEscola, out var ueComDre);

        return new AtualizarContextoTurmaRespostaAlunoDto
        {
            Id = resposta.Id,
            TurmaId = turmaCorreta.CodigoTurma.ToString(),
            UeId = turmaCorreta.CodigoEscola,
            DreId = ueComDre?.CodigoDRE ?? string.Empty,
            AnoTurma = int.TryParse(turmaCorreta.AnoTurma, out var anoTurma) ? anoTurma : null
        };
    }

    private static TurmaElasticDto? EncontrarTurmaCorrespondente(
        IEnumerable<AlunoElasticDto> matriculasAluno,
        Dictionary<int, TurmaElasticDto> turmasPorCodigo,
        string serieAnoCorreto)
    {
        foreach (var matricula in matriculasAluno)
        {
            if (!turmasPorCodigo.TryGetValue(matricula.CodigoTurma, out var turma))
                continue;

            if (string.Equals(turma.AnoTurma, serieAnoCorreto, StringComparison.OrdinalIgnoreCase))
                return turma;
        }

        return null;
    }
}
