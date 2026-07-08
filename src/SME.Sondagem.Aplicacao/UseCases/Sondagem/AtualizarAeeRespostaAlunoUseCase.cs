using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem;

public class AtualizarAeeRespostaAlunoUseCase : IAtualizarAeeRespostaAlunoUseCase
{
    private readonly IRepositorioRespostaAluno _repositorioRespostaAluno;
    private readonly IAlunoAeeService _alunoAeeService;

    public AtualizarAeeRespostaAlunoUseCase(IRepositorioRespostaAluno repositorioRespostaAluno, IAlunoAeeService alunoAeeService)
    {
        _repositorioRespostaAluno = repositorioRespostaAluno;
        _alunoAeeService = alunoAeeService;
    }

    public async Task<int> ExecutarAsync(CancellationToken cancellationToken)
    {
        var ultimoId = 0;
        var totalAtualizado = 0;

        try
        {
            while (true)
            {
                var lote = (await _repositorioRespostaAluno.ObterLotePendenteAeeAsync(ultimoId, 1000, cancellationToken)).ToList();
                if (lote.Count == 0)
                    break;

                var alunoIdsParaAtualizar = new HashSet<int>();

                foreach (var grupoTurma in lote.GroupBy(x => x.TurmaId))
                {
                    if (!int.TryParse(grupoTurma.Key, out var codigoTurma))
                        continue;

                    var alunoIds = grupoTurma.Select(x => x.AlunoId).Distinct().ToList();
                    var ueId = grupoTurma.First().UeId;

                    var resultadoAee = await _alunoAeeService.VerificarAlunosPossuemPlanoAeeAsync(alunoIds, codigoTurma, ueId, cancellationToken);

                    await Task.Delay(TimeSpan.FromSeconds(7), cancellationToken);

                    foreach (var alunoId in alunoIds.Where(id => resultadoAee.TryGetValue(id, out var aee) && aee))
                        alunoIdsParaAtualizar.Add(alunoId);
                }

                if (alunoIdsParaAtualizar.Count > 0)
                    totalAtualizado += await _repositorioRespostaAluno.AtualizarAeeLoteAsync(alunoIdsParaAtualizar, cancellationToken);

                ultimoId = lote.Max(x => x.Id);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new RegraNegocioException(
                $"Falha ao atualizar aee. Último id processado com sucesso: {ultimoId}. Total atualizado até a falha: {totalAtualizado}. Erro: {ex.Message}",
                System.Net.HttpStatusCode.InternalServerError);
        }

        return totalAtualizado;
    }
}
