using Microsoft.Extensions.Logging;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem;

public class AtualizarAeeRespostaAlunoUseCase : IAtualizarAeeRespostaAlunoUseCase
{
    private readonly IRepositorioRespostaAluno _repositorioRespostaAluno;
    private readonly IAlunoAeeService _alunoAeeService;
    private readonly ILogger<AtualizarAeeRespostaAlunoUseCase> _logger;

    public AtualizarAeeRespostaAlunoUseCase(
        IRepositorioRespostaAluno repositorioRespostaAluno,
        IAlunoAeeService alunoAeeService,
        ILogger<AtualizarAeeRespostaAlunoUseCase> logger)
    {
        _repositorioRespostaAluno = repositorioRespostaAluno;
        _alunoAeeService = alunoAeeService;
        _logger = logger;
    }

    public async Task<(int UltimoId, int TotalAtualizado)> ExecutarAsync(int ultimoIdInicial, CancellationToken cancellationToken)
    {
        var ultimoId = ultimoIdInicial;
        var totalAtualizado = 0;

        while (true)
        {
            var turmas = (await _repositorioRespostaAluno.ObterTurmasPendentesAeeAsync(ultimoId, 100, cancellationToken)).ToList();
            if (turmas.Count == 0)
            {
                _logger.LogInformation("Atualização de aee finalizada. UltimoId: {UltimoId}. TotalAtualizado: {TotalAtualizado}", ultimoId, totalAtualizado);
                break;
            }

            foreach (var turma in turmas)
            {
                try
                {
                    if (int.TryParse(turma.TurmaId, out var codigoTurma))
                    {
                        var codigosComAee = await _alunoAeeService.ObterAlunosComPlanoAeeAsync(codigoTurma, turma.UeId, cancellationToken);

                        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

                        if (codigosComAee.Count > 0)
                            totalAtualizado += await _repositorioRespostaAluno.AtualizarAeeLoteAsync(codigosComAee, cancellationToken);
                    }

                    ultimoId = turma.UltimoId;
                    _logger.LogInformation("Turma processada. UltimoId: {UltimoId}. TotalAtualizado até agora: {TotalAtualizado}", ultimoId, totalAtualizado);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    throw new RegraNegocioException(
                        $"Falha ao atualizar aee. Último id processado com sucesso: {ultimoId}. Total atualizado até a falha: {totalAtualizado}. Erro: {ex.Message}",
                        System.Net.HttpStatusCode.InternalServerError);
                }
            }
        }

        return (ultimoId, totalAtualizado);
    }
}
