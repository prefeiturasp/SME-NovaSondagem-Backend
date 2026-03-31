using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Base;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioPorTurmaUseCase : QuestionarioSondagemUseCaseBase, IObterSondagemRelatorioPorTurmaUseCase
{
  
    public ObterSondagemRelatorioPorTurmaUseCase(
        RepositoriosElastic repositoriosElastic,
        RepositoriosSondagem repositoriosSondagem,
        IAlunoPapService alunoPapService,
        IAlunoTurmaService alunoTurmaService,
        IControleAcessoService controleAcessoService,
        IServicoUsuario servicoUsuario)
        : base(repositoriosElastic, repositoriosSondagem, alunoPapService, controleAcessoService, servicoUsuario, alunoTurmaService)
    {
        ArgumentNullException.ThrowIfNull(alunoTurmaService);
    }

    public async Task<QuestionarioSondagemRelatorioDto> ObterSondagemRelatorio([FromQuery] FiltroQuestionario filtro, CancellationToken cancellationToken)
    {
        await ValidarAnoETurmaRelatorio(filtro, cancellationToken);

        var resultado = await ExecutarProcessamentoQuestionario(filtro, true, cancellationToken);
        return (QuestionarioSondagemRelatorioDto)resultado;
    }

    private async Task ValidarAnoETurmaRelatorio(FiltroQuestionario filtro, CancellationToken cancellationToken)
    {
        var turma = await _repositoriosElastic.RepositorioElasticTurma.ObterTurmaPorId(filtro, cancellationToken)
            ?? throw new RegraNegocioException("Turma não localizada", 400);

        if (turma.AnoLetivo < 2025)
            throw new RegraNegocioException("Relatórios só podem ser extraídos para anos letivos a partir de 2025", 400);
    }
}