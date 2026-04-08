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

    public AtualizarContextosRespostasAlunoUseCase(
        RepositoriosElastic repositoriosElastic,
        RepositoriosSondagem repositoriosSondagem,
        RepositorioSondagemRelatorioPorTodasTurma repositorioSondagemRelatorioPorTodasTurma,
        IRepositorioRespostaAlunoDapper repositorioDapper,
        IDadosAlunosService dadosAlunosService)
    {
        _repositoriosElastic = repositoriosElastic ?? throw new ArgumentNullException(nameof(repositoriosElastic));
        _repositoriosSondagem = repositoriosSondagem ?? throw new ArgumentNullException(nameof(repositoriosSondagem));
        _repositorioSondagemRelatorioPorTodasTurma = repositorioSondagemRelatorioPorTodasTurma ?? throw new ArgumentNullException(nameof(repositorioSondagemRelatorioPorTodasTurma));
        _repositorioDapper = repositorioDapper ?? throw new ArgumentNullException(nameof(repositorioDapper));
        _dadosAlunosService = dadosAlunosService;
    }

    public async Task<ResumoAtualizacaoContextoDto> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        const string NOME_COMPONENTE = "Língua Portuguesa";
        const int modalidadeIdFundamental = (int)Modalidade.Fundamental;

        var componenteLp = await _repositoriosSondagem.RepositorioComponenteCurricular.ObterPorNomeModalidade(NOME_COMPONENTE, modalidadeIdFundamental.ToString(), cancellationToken);
        if (componenteLp == null) return new ResumoAtualizacaoContextoDto();

        var respostas = (await _repositoriosSondagem.RepositorioRespostaAluno.ObterExtracaoDadosRespostasAsync(modalidadeIdFundamental, componenteLp.Id, cancellationToken)).ToList();

        var codigoAlunos = respostas.Select(x => x.CodigoEolEstudante!).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
        var dadosAlunos = await ObterAlunos(codigoAlunos, cancellationToken);
        
        var turmasCodigosUnicos = dadosAlunos.Select(x => x.CodigoTurma).Where(c => c > 0).Distinct();
        var dadosCompletosTurmas = await _repositoriosElastic.RepositorioElasticTurma.ObterTurmasPorIds(turmasCodigosUnicos, cancellationToken);
        
        var codigoUes = dadosAlunos.Select(x => x.CodigoEscola!).Where(c => !string.IsNullOrEmpty(c)).Distinct();
        var uesComDre = await BuscarUesDres(codigoUes);

        var alunosPorCodigo = dadosAlunos.Where(x => x.CodigoAluno > 0).GroupBy(x => x.CodigoAluno.ToString()).ToDictionary(g => g.Key, g => g.ToList());
        var turmasPorCodigo = dadosCompletosTurmas.Where(t => t.CodigoTurma > 0).GroupBy(t => t.CodigoTurma).ToDictionary(g => g.Key, g => g.First());
        var uesPorCodigo = uesComDre.Where(e => !string.IsNullOrEmpty(e.CodigoEscola)).GroupBy(e => e.CodigoEscola!).ToDictionary(g => g.Key, g => g.First());

        int registrosLidos = respostas.Count;
        int registrosAtualizados = 0;

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

            var dadosRacaGenero = await ObterDadosRacaGeneroAlunos(turma.CodigoTurma, cancellationToken);
            var dadosAluno = dadosRacaGenero.TryGetValue(aluno.CodigoAluno, out var racaGenero) ? racaGenero : (null, null);

            await _repositorioDapper.AtualizarCamposAsync(
                resposta.RespostaId,
                turma?.CodigoTurma.ToString(),
                ueDre?.CodigoEscola,
                ueDre?.CodigoDRE,
                anoLetivoInt,
                resposta.ModalidadeId,
                Convert.ToInt32(turma?.AnoTurma),
                dadosAluno.Raca,
                dadosAluno.Sexo
            );

            registrosAtualizados++;
        }

        return new ResumoAtualizacaoContextoDto
        {
            RegistrosLidos = registrosLidos,
            RegistrosAtualizados = registrosAtualizados
        };
    }

    private async Task<IEnumerable<AlunoEolDto>> ObterAlunos(List<string> codigoAlunos, CancellationToken cancellationToken)
    {
        var retorno = new List<AlunoEolDto>();
        if (codigoAlunos.Count == 0) return retorno;

        var dados = await _repositorioSondagemRelatorioPorTodasTurma.DadosAlunosService.ObterDadosAlunosPorCodigoUe(codigoAlunos, cancellationToken);
        if (dados.Any()) retorno.AddRange(dados);

        return retorno;
    }

    private async Task<Dictionary<long, (int? Raca, int? Sexo)>> ObterDadosRacaGeneroAlunos(int turmaId, CancellationToken cancellationToken)
    {
        var dadosAlunos = await _dadosAlunosService.ObterDadosRacaGeneroAlunos(turmaId, cancellationToken);

        return dadosAlunos.ToDictionary(
            aluno => aluno.CodigoAluno,
            aluno => (aluno.RacaId,
                 aluno.SexoId
            )
        );
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
