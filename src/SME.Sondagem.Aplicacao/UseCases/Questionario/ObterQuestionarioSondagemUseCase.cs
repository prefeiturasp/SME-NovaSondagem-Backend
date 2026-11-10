using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Services.EOL;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Base;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario;

public class ObterQuestionarioSondagemUseCase : QuestionarioSondagemUseCaseBase, IObterQuestionarioSondagemUseCase
{
    private readonly IAbrangenciaService _abrangenciaService;

    public ObterQuestionarioSondagemUseCase(
        RepositoriosElastic repositoriosElastic,
        RepositoriosSondagem repositoriosSondagem,
        IAlunoPapService alunoPapService,
        IAlunoAeeService alunoAeeService,
        IControleAcessoService controleAcessoService,
        IServicoUsuario servicoUsuario,
        IDadosAlunosService _alunoService,
        IAbrangenciaService abrangenciaService
        )
        : base(repositoriosElastic, repositoriosSondagem, alunoPapService, alunoAeeService, controleAcessoService,
            servicoUsuario, _alunoService)
    {
        _abrangenciaService = abrangenciaService ?? throw new ArgumentNullException(nameof(abrangenciaService));
    }

    public async Task<QuestionarioSondagemDto> ObterQuestionarioSondagem([FromQuery] FiltroQuestionario filtro, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        return (QuestionarioSondagemDto)await ExecutarProcessamentoQuestionario(filtro, false, cancellationToken);
    }

    protected override async Task ValidarAcessoTurmaAsync(TurmaElasticDto turma, CancellationToken cancellationToken)
    {
        if (await _abrangenciaService.DeveIgnorarAbrangenciaAsync(cancellationToken: cancellationToken))
            return;

        var codigoTurma = turma.CodigoTurma.ToString();
        var semestre = turma.Modalidade == (int)Modalidade.EJA ? turma.Semestre : 0;

        var (_, _, turmas) = await _abrangenciaService.ObterAbrangenciaCompletaAsync(
            new AbrangenciaFiltroQuery(
                turma.AnoLetivo,
                turma.Modalidade,
                CodigoDre: null,
                CodigoUe: turma.CodigoEscola,
                Semestre: semestre,
                CodigoTurma: codigoTurma),
            cancellationToken);

        if (!turmas.Contains(codigoTurma))
            throw new RegraNegocioException(MensagemNegocioComuns.SEM_ABRANGENCIA_TURMA_SONDAGEM, 403);
    }
}
