using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem;

public class AtualizarContextoRespostasLegadoUseCase : IAtualizarContextoRespostasLegadoUseCase
{
    private readonly IRepositorioRespostaAluno _repositorioRespostaAluno;
    private readonly IRepositorioSondagem _repositorioSondagem;
    private readonly IRepositorioElasticTurma _repositorioElasticTurma;
    private readonly IDadosAlunosService _dadosAlunosService;
    private readonly IAlunoPapService _alunoPapService;
    private readonly IRepositorioElasticAluno _repositorioElasticAluno;
    private readonly IUeComDreEolService _ueComDreEolService;

    public AtualizarContextoRespostasLegadoUseCase(
        IRepositorioRespostaAluno repositorioRespostaAluno,
        IRepositorioSondagem repositorioSondagem,
        IRepositorioElasticTurma repositorioElasticTurma,
        IDadosAlunosService dadosAlunosService,
        IAlunoPapService alunoPapService,
        IRepositorioElasticAluno repositorioElasticAluno,
        IUeComDreEolService ueComDreEolService)
    {
        _repositorioRespostaAluno = repositorioRespostaAluno;
        _repositorioSondagem = repositorioSondagem;
        _repositorioElasticTurma = repositorioElasticTurma;
        _dadosAlunosService = dadosAlunosService;
        _alunoPapService = alunoPapService;
        _repositorioElasticAluno = repositorioElasticAluno;
        _ueComDreEolService = ueComDreEolService;
    }

    public async Task<int> ExecutarAsync(int pagina, int tamanhoLote, CancellationToken cancellationToken)
    {
        var respostasLegadas = await _repositorioRespostaAluno.ObterRespostasSemContextoPaginadoAsync(pagina, tamanhoLote, cancellationToken);

        if (!respostasLegadas.Any())
            return 0;

        var sondagensIds = respostasLegadas.Select(r => r.SondagemId).Distinct().ToList();
        var sondagens = new Dictionary<int, Dominio.Entidades.Sondagem.Sondagem>();
        foreach (var sId in sondagensIds)
        {
            var sondagem = await _repositorioSondagem.ObterPorIdAsync(sId);
            if (sondagem != null)
                sondagens[sId] = sondagem;
        }

        var lotesAgrupadosAno = respostasLegadas.GroupBy(r => r.AnoLetivo);
        var atualizacoes = new List<AtualizarContextoRespostaAlunoDto>();

        foreach (var grupoAno in lotesAgrupadosAno)
        {
            var anoLetivo = grupoAno.Key;
            var alunosIdsStr = grupoAno.Select(g => g.AlunoId.ToString()).Distinct().ToList();

            var dadosEol = await _dadosAlunosService.ObterDadosAlunosPorCodigoUe(alunosIdsStr, anoLetivo, cancellationToken);
            var turmasCodigos = dadosEol.Select(d => (int)d.CodigoTurma).Distinct().ToList();

            var turmasElastic = await _repositorioElasticTurma.ObterTurmasPorIds(turmasCodigos, cancellationToken);
            var turmasPorCodigo = turmasElastic.ToDictionary(t => t.CodigoTurma, t => t);

            var codigosUes = turmasElastic.Select(t => t.CodigoEscola).Distinct().ToList();
            var uesComDre = await _ueComDreEolService.ObterUesComDrePorCodigosUes(codigosUes);
            var uesPorCodigo = uesComDre.Where(u => !string.IsNullOrEmpty(u.CodigoEscola)).ToDictionary(u => u.CodigoEscola!, u => u);

            var dadosRacaGeneroGlobal = new Dictionary<int, Infrastructure.Dtos.AlunoRacaGeneroDto>();
            var alunosPapGlobais = new Dictionary<int, bool>();
            var alunosElasticGlobal = new Dictionary<int, SME.Sondagem.Infra.Dtos.Questionario.AlunoElasticDto>();

            foreach (var turmaCodigo in turmasCodigos)
            {
                var racaGeneroTurma = await _dadosAlunosService.ObterDadosRacaGeneroAlunos(turmaCodigo, cancellationToken);
                foreach (var aluno in racaGeneroTurma)
                {
                    dadosRacaGeneroGlobal[(int)aluno.CodigoAluno] = aluno;
                }

                var elasticAlunosTurma = await _repositorioElasticAluno.ObterAlunosPorIdTurma(turmaCodigo, anoLetivo, cancellationToken);
                if (elasticAlunosTurma != null)
                {
                    foreach(var alunoE in elasticAlunosTurma)
                    {
                        alunosElasticGlobal[alunoE.CodigoAluno] = alunoE;
                    }
                }
            }

            var alunosPap = await _alunoPapService.VerificarAlunosPossuemProgramaPapAsync(
                grupoAno.Select(x => x.AlunoId).Distinct().ToList(), anoLetivo, cancellationToken);

            foreach (var resposta in grupoAno)
            {
                var alunoEol = dadosEol.FirstOrDefault(a => a.CodigoAluno == resposta.AlunoId);
                if (alunoEol == null) continue;

                var sondagem = sondagens.GetValueOrDefault(resposta.SondagemId);
                if (sondagem == null) continue;

                var dataInicioSondagem = sondagem.PeriodosBimestre?.OrderBy(p => p.DataInicio).FirstOrDefault()?.DataInicio ?? sondagem.DataAplicacao;

                if (alunoEol.DataMatricula.Date > dataInicioSondagem.Date)
                {
                    // Estudante remanejado, pular
                    continue;
                }

                if (!turmasPorCodigo.TryGetValue((int)alunoEol.CodigoTurma, out var turmaElastic))
                    continue;

                uesPorCodigo.TryGetValue(turmaElastic.CodigoEscola, out var ueComDre);

                dadosRacaGeneroGlobal.TryGetValue(resposta.AlunoId, out var demografico);
                alunosPap.TryGetValue(resposta.AlunoId, out var possuiPap);
                alunosElasticGlobal.TryGetValue(resposta.AlunoId, out var alunoElastic);

                atualizacoes.Add(new AtualizarContextoRespostaAlunoDto
                {
                    Id = resposta.Id,
                    TurmaId = turmaElastic.CodigoTurma.ToString(),
                    UeId = turmaElastic.CodigoEscola,
                    DreId = ueComDre?.CodigoDRE ?? string.Empty,
                    AnoLetivo = anoLetivo,
                    AnoTurma = int.TryParse(turmaElastic.AnoTurma, out var aTurma) ? aTurma : null,
                    ModalidadeId = turmaElastic.Modalidade,
                    RacaCorId = demografico?.RacaId,
                    GeneroSexoId = demografico?.SexoId,
                    Pap = possuiPap,
                    Aee = false, // Aee não presente no Elastic/EOL atual
                    Deficiente = alunoElastic != null && alunoElastic.PossuiDeficiencia == 1
                });
            }
        }

        return await _repositorioRespostaAluno.AtualizarContextoLoteAsync(atualizacoes, cancellationToken);
    }
}
