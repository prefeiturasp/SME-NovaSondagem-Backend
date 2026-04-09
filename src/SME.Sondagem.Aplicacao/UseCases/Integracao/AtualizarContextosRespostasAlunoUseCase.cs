using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Integracao;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Extensions;
using SME.Sondagem.Infrastructure.Dtos;
using SME.Sondagem.Infrastructure.Dtos.Integracao;

namespace SME.Sondagem.Aplicacao.UseCases.Integracao;

public class AtualizarContextosRespostasAlunoUseCase : IAtualizarContextosRespostasAlunoUseCase
{
    private readonly RepositoriosElastic _repositoriosElastic;
    private readonly RepositoriosSondagem _repositoriosSondagem;
    private readonly RepositorioSondagemRelatorioPorTodasTurma _repositorioSondagemRelatorioPorTodasTurma;
    private readonly IRepositorioRespostaAlunoDapper _repositorioDapper;
    protected readonly IDadosAlunosService _dadosAlunosService;
    protected readonly IRepositorioGeneroSexo _repositorioGeneroSexo;
    protected readonly IRepositorioRacaCor _repositorioRacaCor;
    private readonly IAlunoPapService _alunoPapService;

    public AtualizarContextosRespostasAlunoUseCase(
        RepositoriosElastic repositoriosElastic,
        RepositoriosSondagem repositoriosSondagem,
        RepositorioSondagemRelatorioPorTodasTurma repositorioSondagemRelatorioPorTodasTurma,
        IRepositorioRespostaAlunoDapper repositorioDapper,
        IDadosAlunosService dadosAlunosService,
        IRepositorioRacaCor repositorioRacaCor,
        IAlunoPapService alunoPapService)
    {
        _repositoriosElastic = repositoriosElastic ?? throw new ArgumentNullException(nameof(repositoriosElastic));
        _repositoriosSondagem = repositoriosSondagem ?? throw new ArgumentNullException(nameof(repositoriosSondagem));
        _repositorioSondagemRelatorioPorTodasTurma = repositorioSondagemRelatorioPorTodasTurma ?? throw new ArgumentNullException(nameof(repositorioSondagemRelatorioPorTodasTurma));
        _repositorioDapper = repositorioDapper ?? throw new ArgumentNullException(nameof(repositorioDapper));
        _dadosAlunosService = dadosAlunosService;
        _repositorioRacaCor = repositorioRacaCor;
        _alunoPapService = alunoPapService ?? throw new ArgumentNullException(nameof(alunoPapService));
    }

    public async Task<ResumoAtualizacaoContextoDto> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        const string NOME_COMPONENTE = "Língua Portuguesa";
        const int modalidadeIdFundamental = (int)Modalidade.Fundamental;

        var componenteLp = await _repositoriosSondagem.RepositorioComponenteCurricular.ObterPorNomeModalidade(NOME_COMPONENTE, modalidadeIdFundamental.ToString(), cancellationToken);
        if (componenteLp == null) return new ResumoAtualizacaoContextoDto();

        var respostas = (await _repositoriosSondagem.RepositorioRespostaAluno.ObterExtracaoDadosRespostasAsync(modalidadeIdFundamental, componenteLp.Id, cancellationToken)).ToList();

        var codigoAlunos = respostas.Select(x => x.CodigoEolEstudante!).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
        var (dadosAlunos, alunosPap) = await ObterAlunos(codigoAlunos, cancellationToken);

        var turmasCodigosUnicos = dadosAlunos.Select(x => x.CodigoTurma).Where(c => c > 0).Distinct();
        var dadosCompletosTurmas = await _repositoriosElastic.RepositorioElasticTurma.ObterTurmasPorIds(turmasCodigosUnicos, cancellationToken);

        var codigoUes = dadosAlunos.Select(x => x.CodigoEscola!).Where(c => !string.IsNullOrEmpty(c)).Distinct();
        var uesComDre = await BuscarUesDres(codigoUes);

        var alunosPorCodigo = dadosAlunos.Where(x => x.CodigoAluno > 0).GroupBy(x => x.CodigoAluno.ToString()).ToDictionary(g => g.Key, g => g.ToList());
        var turmasPorCodigo = dadosCompletosTurmas.Where(t => t.CodigoTurma > 0).GroupBy(t => t.CodigoTurma).ToDictionary(g => g.Key, g => g.First());
        var uesPorCodigo = uesComDre.Where(e => !string.IsNullOrEmpty(e.CodigoEscola)).GroupBy(e => e.CodigoEscola!).ToDictionary(g => g.Key, g => g.First());

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

        int registrosLidos = respostas.Count;
        int registrosAtualizados = 0;
        var listagemRacaCor = await _repositorioRacaCor.ListarAsync(cancellationToken);
        
        var turmasIdsList = turmasCodigosUnicos.ToList();
        var dadosRacaGenero = await ObterDadosRacaGeneroAlunos(turmasIdsList, cancellationToken);

        foreach (var resposta in respostas)
        {
            if (!alunosPorCodigo.TryGetValue(resposta.CodigoEolEstudante ?? string.Empty, out var listaAlunos))
                continue;

            var aluno = listaAlunos.FirstOrDefault();

            if (resposta.DataResposta.HasValue && listaAlunos.Count > 1)
            {
                var alunoMatriculaAtiva = listaAlunos
                    .Where(a => a.DataMatricula.Date <= resposta.DataResposta.Value.Date)
                    .OrderByDescending(a => a.DataMatricula)
                    .FirstOrDefault();

                if (alunoMatriculaAtiva != null)
                    aluno = alunoMatriculaAtiva;
            }

            if (aluno == null)
                continue;

            turmasPorCodigo.TryGetValue(aluno.CodigoTurma, out var turma);
            uesPorCodigo.TryGetValue(aluno.CodigoEscola!, out var ueDre);

            int? anoLetivoInt = null;
            if (int.TryParse(turma?.AnoTurma, out var anoResult))
                anoLetivoInt = anoResult;

            var dadosAluno = dadosRacaGenero.TryGetValue(aluno.CodigoAluno, out var racaGenero) ? racaGenero : (null, null);
            var racaCor = listagemRacaCor.FirstOrDefault(r => r.CodigoEolRacaCor == dadosAluno.Raca);

            var contextoDto = new ContextoRespostaAlunoDto
            {
                RespostaId = resposta.RespostaId,
                TurmaId = turma?.CodigoTurma.ToString(),
                UeId = ueDre?.CodigoEscola,
                DreId = ueDre?.CodigoDRE,
                AnoLetivo = anoLetivoInt,
                ModalidadeId = resposta.ModalidadeId,
                AnoTurma = Convert.ToInt32(turma?.AnoTurma),
                GeneroId = dadosAluno.Sexo,
                RacaId = racaCor?.Id,
                Pap = alunosPap.TryGetValue(aluno.CodigoAluno, out var possuiPap) && possuiPap,
                Deficiente = alunosElasticDict.TryGetValue(aluno.CodigoAluno, out var elDto) && elDto.PossuiDeficiencia == 1,
                Aee = false // TODO: Avaliar de onde obter o Aee
            };

            await _repositorioDapper.AtualizarCamposAsync(contextoDto);

            registrosAtualizados++;
            Console.WriteLine($"[Sondagem] AtualizarContextosRespostasAlunoUseCase - Registros Lidos: {registrosLidos} | Registros Atualizados: {registrosAtualizados}");
            System.Diagnostics.Debug.WriteLine($"[Sondagem] AtualizarContextosRespostasAlunoUseCase - Registros Lidos: {registrosLidos} | Registros Atualizados: {registrosAtualizados}");

        }

        var resumo = new ResumoAtualizacaoContextoDto
        {
            RegistrosLidos = registrosLidos,
            RegistrosAtualizados = registrosAtualizados
        };

        
        return resumo;
    }

    private async Task<(IEnumerable<AlunoEolDto> Alunos, Dictionary<int, bool> AlunosPap)> ObterAlunos(List<string> codigoAlunos, CancellationToken cancellationToken)
    {
        var retorno = new List<AlunoEolDto>();
        var alunosPap = new Dictionary<int, bool>();

        if (codigoAlunos.Count == 0) return (retorno, alunosPap);

        var dados = await _repositorioSondagemRelatorioPorTodasTurma.DadosAlunosService.ObterDadosAlunosPorCodigoUe(codigoAlunos, cancellationToken);
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

    private async Task<IEnumerable<UeComDreEolDto>> BuscarUesDres(IEnumerable<string> codigosUes)
    {
        var retorno = new List<UeComDreEolDto>();
        if (!codigosUes.Any()) return retorno;

        var busca = await _repositorioSondagemRelatorioPorTodasTurma.UeComDreEolService.ObterUesComDrePorCodigosUes(codigosUes);
        if (busca.Any()) retorno.AddRange(busca);

        return retorno;
    }
}
