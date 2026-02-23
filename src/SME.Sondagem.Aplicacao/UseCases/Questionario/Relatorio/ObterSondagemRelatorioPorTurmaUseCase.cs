using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Base;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioPorTurmaUseCase : QuestionarioSondagemUseCaseBase, IObterSondagemRelatorioPorTurmaUseCase
{
    private readonly IAlunoTurmaService _alunoTurmaService;

    public ObterSondagemRelatorioPorTurmaUseCase(
        RepositoriosElastic repositoriosElastic,
        RepositoriosSondagem repositoriosSondagem,
        IAlunoPapService alunoPapService,
        IAlunoTurmaService alunoTurmaService,
        IControleAcessoService controleAcessoService)
        : base(repositoriosElastic, repositoriosSondagem, alunoPapService, controleAcessoService)
    {
        _alunoTurmaService = alunoTurmaService ?? throw new ArgumentNullException(nameof(alunoTurmaService));
    }

    public async Task<QuestionarioSondagemRelatorioDto> ObterSondagemRelatorio([FromQuery] FiltroQuestionario filtro, CancellationToken cancellationToken)
    {
        await ValidarAnoRelatorio(filtro, cancellationToken);

        var resultado = await ExecutarProcessamentoQuestionario(filtro, true, cancellationToken);
        return (QuestionarioSondagemRelatorioDto)resultado;
    }

    private async Task ValidarAnoRelatorio(FiltroQuestionario filtro, CancellationToken cancellationToken)
    {
        var turma = await _repositoriosElastic.RepositorioElasticTurma.ObterTurmaPorId(filtro, cancellationToken)
            ?? throw new RegraNegocioException("Turma não localizada", 400);

        if (turma.AnoLetivo < 2025)
            throw new RegraNegocioException("Relatórios só podem ser extraídos para anos letivos a partir de 2025", 400);
    }

    protected override async Task<DadosAlunos> ObterDadosAlunos(
        int turmaId,
        int anoLetivo,
        ContextoProcesamento contexto,
        CancellationToken cancellationToken)
    {
        var alunosComPap = await _alunoPapService.VerificarAlunosPossuemProgramaPapAsync(
            contexto.CodigosAlunos,
            anoLetivo,
            cancellationToken);

        var alunosComLinguaPortuguesaSegundaLingua = await _repositoriosSondagem.RepositorioRespostaAluno
            .VerificarAlunosPossuiLinguaPortuguesaAsync(
                contexto.CodigosAlunos,
                contexto.QuestaoLinguaPortuguesa,
                cancellationToken);

        var dadosRacaGenero = await ObterDadosRacaGeneroAlunos(turmaId, cancellationToken);

        return new DadosAlunos
        {
            AlunosComPap = alunosComPap,
            AlunosComLinguaPortuguesaSegundaLingua = alunosComLinguaPortuguesaSegundaLingua,
            DadosRacaGenero = dadosRacaGenero
        };
    }

    private async Task<Dictionary<long, (string Raca, string Sexo)>> ObterDadosRacaGeneroAlunos(int turmaId, CancellationToken cancellationToken)
    {
        var dadosAlunos = await _alunoTurmaService.InformacoesAlunosPorTurma(turmaId, cancellationToken);

        return dadosAlunos.ToDictionary(
            aluno => aluno.CodigoAluno,
            aluno => (
                Raca: ConverterCodigoRacaParaDescricao(aluno.Raca),
                Sexo: ConverterCodigoGeneroParaDescricao(aluno.Sexo)
            )
        );
    }

    private static string ConverterCodigoGeneroParaDescricao(string codigoGenero)
    {
        return codigoGenero?.ToUpperInvariant() switch
        {
            "M" => "Masculino",
            "F" => "Feminino",
            _ => string.IsNullOrWhiteSpace(codigoGenero) ? string.Empty : codigoGenero
        };
    }

    private static string ConverterCodigoRacaParaDescricao(string codigoRaca)
    {
        if (string.IsNullOrWhiteSpace(codigoRaca))
            return string.Empty;

        var racaUpper = codigoRaca.ToUpperInvariant();
        return racaUpper switch
        {
            "BRANCA" => "Branca",
            "PRETA" => "Preta",
            "PARDA" => "Parda",
            "AMARELA" => "Amarela",
            "INDIGENA" => "Indígena",
            "INDÍGENA" => "Indígena",
            _ => char.ToUpperInvariant(racaUpper[0]) + racaUpper[1..].ToLowerInvariant()
        };
    }
}