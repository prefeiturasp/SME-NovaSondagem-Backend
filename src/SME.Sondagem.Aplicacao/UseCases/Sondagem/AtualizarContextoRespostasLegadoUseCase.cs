using System.Collections.Concurrent;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem;

public class AtualizarContextoRespostasLegadoUseCase : IAtualizarContextoRespostasLegadoUseCase
{
    private readonly IRepositorioRespostaAluno _repositorioRespostaAluno;
    private readonly IRepositorioSondagem _repositorioSondagem;
    private readonly IDadosAlunosService _dadosAlunosService;
    private readonly IAlunoPapService _alunoPapService;
    private readonly IUeComDreEolService _ueComDreEolService;
    protected readonly IRepositorioGeneroSexo _repositorioGeneroSexo;
    protected readonly IRepositorioRacaCor _repositorioRacaCor;
    private readonly RepositorioSondagemRelatorioPorTodasTurma _repositorioSondagemRelatorioPorTodasTurma;
    private readonly RepositoriosElastic _repositoriosElastic;

    public AtualizarContextoRespostasLegadoUseCase(AtualizarContextoRespostasLegadoDependencias dependencias)
    {
        _repositorioRespostaAluno = dependencias.RepositorioRespostaAluno;
        _repositorioSondagem = dependencias.RepositorioSondagem;
        _dadosAlunosService = dependencias.DadosAlunosService;
        _alunoPapService = dependencias.AlunoPapService;
        _ueComDreEolService = dependencias.UeComDreEolService;
        _repositorioSondagemRelatorioPorTodasTurma = dependencias.RepositorioSondagemRelatorioPorTodasTurma;
        _repositorioRacaCor = dependencias.RepositorioRacaCor;
        _repositorioGeneroSexo = dependencias.RepositorioGeneroSexo;
        _repositoriosElastic = dependencias.RepositoriosElastic;
    }

    public async Task<int> ExecutarAsync(int respostaIdInicial, int pagina, int tamanhoLote, CancellationToken cancellationToken)
    {
        var respostasLegadas = await _repositorioRespostaAluno.ObterRespostasSemContextoPaginadoAsync(respostaIdInicial, pagina, tamanhoLote, cancellationToken);
        var respostasLista = respostasLegadas.ToList();
        if (respostasLista.Count == 0)
            return 0;

        var sondagensIds = respostasLista.Select(r => r.SondagemId).Distinct().ToList();
        var sondagens = await CarregarSondagensPorIdsAsync(sondagensIds);

        var listagemRacaCor = (await _repositorioRacaCor.ListarAsync(cancellationToken)).ToList();
        var lotesAgrupadosAno = respostasLista.GroupBy(r => r.AnoLetivo).ToList();
        var atualizacoes = new List<AtualizarContextoRespostaAlunoDto>();

        foreach (var grupoAno in lotesAgrupadosAno)
        {
            atualizacoes.AddRange(await ProcessarGrupoAnoLetivoAsync(
                grupoAno,
                respostasLista,
                sondagens,
                listagemRacaCor,
                cancellationToken));
        }

        if (atualizacoes.Count > 0)
            return await _repositorioRespostaAluno.AtualizarContextoLoteAsync(atualizacoes, cancellationToken);

        return atualizacoes.Count;
    }

    private async Task<Dictionary<int, Dominio.Entidades.Sondagem.Sondagem>> CarregarSondagensPorIdsAsync(List<int> sondagensIds)
    {
        var sondagens = new Dictionary<int, Dominio.Entidades.Sondagem.Sondagem>();
        foreach (var sId in sondagensIds)
        {
            var sondagem = await _repositorioSondagem.ObterPorIdAsync(sId);
            if (sondagem != null)
                sondagens[sId] = sondagem;
        }

        return sondagens;
    }

    private async Task<ConcurrentDictionary<int, AlunoElasticDto>> CarregarAlunosElasticPorTurmasAsync(
        IEnumerable<TurmaElasticDto> dadosCompletosTurmas,
        CancellationToken cancellationToken)
    {
        var alunosElasticDict = new ConcurrentDictionary<int, AlunoElasticDto>();
        await Task.WhenAll(dadosCompletosTurmas.Select(async turma =>
        {
            var alunosTurma = await _repositoriosElastic.RepositorioElasticAluno.ObterAlunosPorIdTurma(
                turma.CodigoTurma, turma.AnoLetivo, cancellationToken);
            if (alunosTurma == null)
                return;

            foreach (var aluno in alunosTurma)
                alunosElasticDict.TryAdd(aluno.CodigoAluno, aluno);
        }));

        return alunosElasticDict;
    }

    private async Task<List<AtualizarContextoRespostaAlunoDto>> ProcessarGrupoAnoLetivoAsync(
        IGrouping<int, RespostaAlunoLegadoDto> grupoAno,
        IReadOnlyList<RespostaAlunoLegadoDto> respostasLegadas,
        Dictionary<int, Dominio.Entidades.Sondagem.Sondagem> sondagens,
        IReadOnlyList<Dominio.Entidades.RacaCor> listagemRacaCor,
        CancellationToken cancellationToken)
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
        var alunosElasticDict = await CarregarAlunosElasticPorTurmasAsync(dadosCompletosTurmas, cancellationToken);

        var contexto = new ContextoMontagemAtualizacaoLegado
        {
            AnoLetivo = anoLetivo,
            DadosAlunos = dadosAlunos,
            Sondagens = sondagens,
            ListagemRacaCor = listagemRacaCor,
            TurmasPorCodigo = turmasPorCodigo,
            UesPorCodigo = uesPorCodigo,
            DadosRacaGenero = dadosRacaGenero,
            AlunosPap = alunosPap,
            AlunosElasticDict = alunosElasticDict,
        };

        var atualizacoes = new List<AtualizarContextoRespostaAlunoDto>();
        foreach (var resposta in grupoAno)
        {
            var atualizacao = MontarAtualizacao(resposta, contexto);
            if (atualizacao != null)
                atualizacoes.Add(atualizacao);
        }

        return atualizacoes;
    }

    private static AtualizarContextoRespostaAlunoDto? MontarAtualizacao(
        RespostaAlunoLegadoDto resposta,
        ContextoMontagemAtualizacaoLegado contexto)
    {
        var aluno = contexto.DadosAlunos.FirstOrDefault(a => a.CodigoAluno == resposta.AlunoId);
        if (aluno == null)
            return null;

        if (!contexto.Sondagens.TryGetValue(resposta.SondagemId, out var sondagem))
            return null;

        var dataInicioSondagem = sondagem.PeriodosBimestre?.OrderBy(p => p.DataInicio).FirstOrDefault()?.DataInicio ?? sondagem.DataAplicacao;
        if (aluno.DataMatricula.Date > dataInicioSondagem.Date)
            return null; // Estudante remanejado, pular

        if (!contexto.TurmasPorCodigo.TryGetValue(aluno.CodigoTurma, out var turmaElastic))
            return null;

        contexto.UesPorCodigo.TryGetValue(turmaElastic.CodigoEscola, out var ueComDre);

        var dadosAluno = contexto.DadosRacaGenero.TryGetValue(aluno.CodigoAluno, out var racaGenero) ? racaGenero : (null, null);
        var racaCor = contexto.ListagemRacaCor.FirstOrDefault(r => r.CodigoEolRacaCor == dadosAluno.Raca);

        return new AtualizarContextoRespostaAlunoDto
        {
            Id = resposta.Id,
            TurmaId = turmaElastic.CodigoTurma.ToString(),
            UeId = turmaElastic.CodigoEscola,
            DreId = ueComDre?.CodigoDRE ?? string.Empty,
            AnoLetivo = contexto.AnoLetivo,
            AnoTurma = int.TryParse(turmaElastic.AnoTurma, out var aTurma) ? aTurma : null,
            ModalidadeId = turmaElastic.Modalidade,
            RacaCorId = racaCor?.Id,
            GeneroSexoId = dadosAluno.Sexo,
            Pap = contexto.AlunosPap.TryGetValue(aluno.CodigoAluno, out var possuiPap) && possuiPap,
            Aee = false,
            Deficiente = contexto.AlunosElasticDict.TryGetValue(aluno.CodigoAluno, out var elDto) && elDto.PossuiDeficiencia == 1,
        };
    }

    private sealed class ContextoMontagemAtualizacaoLegado
    {
        public required int AnoLetivo { get; init; }
        public required IEnumerable<AlunoEolDto> DadosAlunos { get; init; }
        public required Dictionary<int, Dominio.Entidades.Sondagem.Sondagem> Sondagens { get; init; }
        public required IReadOnlyList<Dominio.Entidades.RacaCor> ListagemRacaCor { get; init; }
        public required Dictionary<int, TurmaElasticDto> TurmasPorCodigo { get; init; }
        public required Dictionary<string, UeComDreEolDto> UesPorCodigo { get; init; }
        public required Dictionary<long, (int? Raca, int? Sexo)> DadosRacaGenero { get; init; }
        public required Dictionary<int, bool> AlunosPap { get; init; }
        public required ConcurrentDictionary<int, AlunoElasticDto> AlunosElasticDict { get; init; }
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
