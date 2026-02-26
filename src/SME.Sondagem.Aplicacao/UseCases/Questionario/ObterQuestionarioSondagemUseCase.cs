using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Base;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario;

public class ObterQuestionarioSondagemUseCase : QuestionarioSondagemUseCaseBase, IObterQuestionarioSondagemUseCase
{
    public ObterQuestionarioSondagemUseCase(
        RepositoriosElastic repositoriosElastic,
        RepositoriosSondagem repositoriosSondagem,
        IAlunoPapService alunoPapService,
        IControleAcessoService controleAcessoService,
        IServicoUsuario servicoUsuario)
        : base(repositoriosElastic, repositoriosSondagem, alunoPapService, controleAcessoService, servicoUsuario)
    {
    }

    public async Task<QuestionarioSondagemDto> ObterQuestionarioSondagem([FromQuery] FiltroQuestionario filtro, CancellationToken cancellationToken)
    {
        return (QuestionarioSondagemDto)await ExecutarProcessamentoQuestionario(filtro, false, cancellationToken);
    }

    protected override async Task<DadosAlunosDto> ObterDadosAlunos(
        int turmaId,
        int anoLetivo,
        ContextoProcessamentoDto contexto,
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

        return new DadosAlunosDto
        {
            AlunosComPap = alunosComPap,
            AlunosComLinguaPortuguesaSegundaLingua = alunosComLinguaPortuguesaSegundaLingua,
            DadosRacaGenero = null
        };
    }
}