using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Infra.Exceptions;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem;

public class SondagemSalvarRespostasUseCase : ISondagemSalvarRespostasUseCase
{
    private readonly IRepositorioSondagem _repositorioSondagem;
    private readonly IRepositorioRespostaAluno _repositorioSondagemResposta;

    public SondagemSalvarRespostasUseCase(IRepositorioSondagem repositorioSondagem,
        IRepositorioRespostaAluno repositorioSondagemResposta)
    {
        _repositorioSondagem = repositorioSondagem;
        _repositorioSondagemResposta = repositorioSondagemResposta;
    }

    public async Task<bool> SalvarOuAtualizarSondagemAsync(SondagemSalvarDto dto)
    {
        var sondagemAtiva = await ObterEValidarSondagemAtiva(dto.SondagemId);
        var periodosBimestresAtivos = sondagemAtiva.PeriodosBimestre.Where(x => !x.Excluido);

        var alunosIds = dto.Alunos.Select(a => a.AlunoId);
        var questoresIds = dto.Alunos.SelectMany(a => a.Respostas.Select(r => r.QuestaoId));
        var repostasAlunos =
            await _repositorioSondagemResposta.ObterRespostasPorSondagemEAlunosAsync(dto.SondagemId, alunosIds,
                questoresIds);

        var respostas = ProcessarRespostasAlunos(dto, periodosBimestresAtivos, repostasAlunos);

        return await _repositorioSondagemResposta.SalvarAsync(respostas);
    }

    private async Task<Dominio.Entidades.Sondagem.Sondagem> ObterEValidarSondagemAtiva(int sondagemId)
    {
        var sondagemAtiva = await _repositorioSondagem.ObterSondagemAtiva();

        if (sondagemAtiva == null)
            throw new NegocioException(MensagemNegocioComuns.NENHUM_SONDAGEM_ATIVA_ENCONRADA);

        return sondagemAtiva.Id != sondagemId
            ? throw new NegocioException(MensagemNegocioComuns.SALVAR_SOMENTE_PARA_SONDAGEM_ATIVA)
            : sondagemAtiva;
    }

    private static List<RespostaAluno> ProcessarRespostasAlunos(
        SondagemSalvarDto dto,
        IEnumerable<SondagemPeriodoBimestre> periodosBimestresAtivos,
        IEnumerable<RespostaAluno> repostasAlunos)
    {
        var respostas = new List<RespostaAluno>();

        foreach (var aluno in dto.Alunos)
        foreach (var respostaDto in aluno.Respostas)
        {
            var resposta = ProcessarRespostaIndividual(
                dto.SondagemId,
                aluno.AlunoId,
                respostaDto,
                periodosBimestresAtivos,
                repostasAlunos);

            if (resposta != null)
                respostas.Add(resposta);
        }

        return respostas;
    }

    private static RespostaAluno? ProcessarRespostaIndividual(
        int sondagemId,
        int alunoId,
        RespostaSondagemDto respostaDto,
        IEnumerable<SondagemPeriodoBimestre> periodosBimestresAtivos,
        IEnumerable<RespostaAluno> repostasAlunos)
    {
        var periodoBimestreAtivo = periodosBimestresAtivos
            .FirstOrDefault(pb => pb.BimestreId == respostaDto.BimestreId);

        if (!ValidarPeriodoBimestre(periodoBimestreAtivo))
            return null;

        var respostaExistente = repostasAlunos?.FirstOrDefault(r =>
            r.AlunoId == alunoId && r.QuestaoId == respostaDto.QuestaoId);

        return CriarOuAtualizarResposta(
            sondagemId,
            alunoId,
            respostaDto,
            respostaExistente);
    }

    private static bool ValidarPeriodoBimestre(SondagemPeriodoBimestre? periodoBimestreAtivo)
    {
        if (periodoBimestreAtivo == null)
            return false;

        return periodoBimestreAtivo.DataInicio <= DateTime.Now &&
               periodoBimestreAtivo.DataFim >= DateTime.Now;
    }

    private static RespostaAluno CriarOuAtualizarResposta(
        int sondagemId,
        int alunoId,
        RespostaSondagemDto respostaDto,
        RespostaAluno? respostaExistente)
    {
        if (respostaExistente == null)
            return new RespostaAluno(
                sondagemId,
                alunoId,
                respostaDto.QuestaoId,
                respostaDto.OpcaoRepostaId,
                DateTime.UtcNow,
                respostaDto.BimestreId);
        respostaExistente.AtualizarResposta(respostaDto.OpcaoRepostaId, DateTime.UtcNow);
        return respostaExistente;
    }
}