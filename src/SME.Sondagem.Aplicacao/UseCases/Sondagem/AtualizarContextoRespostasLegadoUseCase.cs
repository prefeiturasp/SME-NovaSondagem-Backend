using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos;
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
    protected readonly IRepositorioGeneroSexo _repositorioGeneroSexo;
    protected readonly IRepositorioRacaCor _repositorioRacaCor;
    private readonly RepositorioSondagemRelatorioPorTodasTurma _repositorioSondagemRelatorioPorTodasTurma;
    private readonly RepositoriosElastic _repositoriosElastic;

    public AtualizarContextoRespostasLegadoUseCase(
        IRepositorioRespostaAluno repositorioRespostaAluno,
        IRepositorioSondagem repositorioSondagem,
        IRepositorioElasticTurma repositorioElasticTurma,
        IDadosAlunosService dadosAlunosService,
        IAlunoPapService alunoPapService,
        IRepositorioElasticAluno repositorioElasticAluno,
        IUeComDreEolService ueComDreEolService,
        RepositorioSondagemRelatorioPorTodasTurma repositorioSondagemRelatorioPorTodasTurma,
        IRepositorioRacaCor repositorioRacaCor,
        IRepositorioGeneroSexo repositorioGeneroSexo,
        RepositoriosElastic repositoriosElastic)
    {
        _repositorioRespostaAluno = repositorioRespostaAluno;
        _repositorioSondagem = repositorioSondagem;
        _repositorioElasticTurma = repositorioElasticTurma;
        _dadosAlunosService = dadosAlunosService;
        _alunoPapService = alunoPapService;
        _repositorioElasticAluno = repositorioElasticAluno;
        _ueComDreEolService = ueComDreEolService;
        _repositorioSondagemRelatorioPorTodasTurma = repositorioSondagemRelatorioPorTodasTurma;
        _repositorioRacaCor = repositorioRacaCor;
        _repositorioGeneroSexo = repositorioGeneroSexo;
        _repositoriosElastic = repositoriosElastic;
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

        var listagemRacaCor = await _repositorioRacaCor.ListarAsync(cancellationToken);

        foreach (var grupoAno in lotesAgrupadosAno)
        {
            var anoLetivo = grupoAno.Key;

            var codigoAlunos = respostasLegadas.Select(x => x.AlunoId!).ToList();
            var (dadosAlunos, alunosPap) = await ObterAlunos(codigoAlunos, cancellationToken);

            var turmasCodigosUnicos = dadosAlunos.Select(x => x.CodigoTurma).Where(c => c > 0).Distinct();
            var dadosCompletosTurmas = await _repositoriosElastic.RepositorioElasticTurma.ObterTurmasPorIds(turmasCodigosUnicos, cancellationToken);

            var turmasPorCodigo = dadosCompletosTurmas.Where(t => t.CodigoTurma > 0).GroupBy(t => t.CodigoTurma).ToDictionary(g => g.Key, g => g.First());
            var codigoUes = dadosAlunos.Select(x => x.CodigoEscola!).Where(c => !string.IsNullOrEmpty(c)).Distinct();
            var uesComDre = await _ueComDreEolService.ObterUesComDrePorCodigosUes(codigoUes);
            var uesPorCodigo = uesComDre.Where(u => !string.IsNullOrEmpty(u.CodigoEscola)).ToDictionary(u => u.CodigoEscola!, u => u);

            var turmasIdsList = turmasCodigosUnicos.ToList();
            var dadosRacaGenero = await ObterDadosRacaGeneroAlunos(turmasIdsList, cancellationToken);

            var alunosElasticDict = new System.Collections.Concurrent.ConcurrentDictionary<int, SME.Sondagem.Infra.Dtos.Questionario.AlunoElasticDto>();
            await Task.WhenAll(dadosCompletosTurmas.Select(async turma =>
            {
                var alunosTurma = await _repositoriosElastic.RepositorioElasticAluno.ObterAlunosPorIdTurma(turma.CodigoTurma, turma.AnoLetivo, cancellationToken);
                if (alunosTurma != null)
                {
                    foreach (var aluno in alunosTurma)
                    {
                        alunosElasticDict.TryAdd(aluno.CodigoAluno, aluno);
                    }
                }
            }));

            foreach (var resposta in grupoAno)
            {
                var aluno = dadosAlunos.FirstOrDefault(a => a.CodigoAluno == resposta.AlunoId);
                if (aluno == null) continue;

                var sondagem = sondagens.GetValueOrDefault(resposta.SondagemId);
                if (sondagem == null) continue;

                var dataInicioSondagem = sondagem.PeriodosBimestre?.OrderBy(p => p.DataInicio).FirstOrDefault()?.DataInicio ?? sondagem.DataAplicacao;

                if (aluno.DataMatricula.Date > dataInicioSondagem.Date)
                {
                    // Estudante remanejado, pular
                    continue;
                }

                if (!turmasPorCodigo.TryGetValue((int)aluno.CodigoTurma, out var turmaElastic))
                    continue;

                uesPorCodigo.TryGetValue(turmaElastic.CodigoEscola, out var ueComDre);

                var dadosAluno = dadosRacaGenero.TryGetValue(aluno.CodigoAluno, out var racaGenero) ? racaGenero : (null, null);
                var racaCor = listagemRacaCor.FirstOrDefault(r => r.CodigoEolRacaCor == dadosAluno.Raca);


                atualizacoes.Add(new AtualizarContextoRespostaAlunoDto
                {
                    Id = resposta.Id,
                    TurmaId = turmaElastic.CodigoTurma.ToString(),
                    UeId = turmaElastic.CodigoEscola,
                    DreId = ueComDre?.CodigoDRE ?? string.Empty,
                    AnoLetivo = anoLetivo,
                    AnoTurma = int.TryParse(turmaElastic.AnoTurma, out var aTurma) ? aTurma : null,
                    ModalidadeId = turmaElastic.Modalidade,
                    RacaCorId = racaCor?.Id,
                    GeneroSexoId = dadosAluno.Sexo,
                    Pap = alunosPap.TryGetValue(aluno.CodigoAluno, out var possuiPap) && possuiPap,
                    Aee = false, // Aee não presente no Elastic/EOL atual
                    Deficiente = alunosElasticDict.TryGetValue(aluno.CodigoAluno, out var elDto) && elDto.PossuiDeficiencia == 1,
                });
            }
        }

        return await _repositorioRespostaAluno.AtualizarContextoLoteAsync(atualizacoes, cancellationToken);
    }

    private async Task<(IEnumerable<AlunoEolDto> Alunos, Dictionary<int, bool> AlunosPap)> ObterAlunos(List<int> codigoAlunos, CancellationToken cancellationToken)
    {
        var retorno = new List<AlunoEolDto>();
        var alunosPap = new Dictionary<int, bool>();

        if (codigoAlunos.Count == 0) return (retorno, alunosPap);

        var dados = await _repositorioSondagemRelatorioPorTodasTurma.DadosAlunosService.ObterDadosAlunosPorCodigoUe(codigoAlunos, DateTime.Now.Year, cancellationToken);
        if (dados.Any()) retorno.AddRange(dados);

        var codigosInt = retorno.Select(x => x.CodigoAluno).Distinct().ToList();
        if (codigosInt.Count > 0)
        {
            alunosPap = await _alunoPapService.VerificarAlunosPossuemProgramaPapAsync(codigosInt, DateTime.Now.Year, cancellationToken);
        }

        return (retorno, alunosPap);
    }

    private async Task<Dictionary<long, (int? Raca, int? Sexo)>> ObterDadosRacaGeneroAlunos(IEnumerable<int> turmasIds, CancellationToken cancellationToken)
    {
        var dicionario = new Dictionary<long, (int? Raca, int? Sexo)>();

        foreach (var turmaId in turmasIds)
        {
            var dadosAlunos = await _dadosAlunosService.ObterDadosRacaGeneroAlunos(turmaId, cancellationToken);
            foreach (var aluno in dadosAlunos)
            {
                dicionario.TryAdd(aluno.CodigoAluno, (aluno.RacaId, aluno.SexoId));
            }
        }

        return dicionario;
    }
}
