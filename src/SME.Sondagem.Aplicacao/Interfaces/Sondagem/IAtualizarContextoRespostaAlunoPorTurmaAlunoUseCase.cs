namespace SME.Sondagem.Aplicacao.Interfaces.Sondagem;

public interface IAtualizarContextoRespostaAlunoPorTurmaAlunoUseCase
{
    Task<AtualizacaoContextoRespostaPorTurmaAlunoOutput> ExecutarAsync(int codigoTurma, int codigoAluno, int? anoLetivo,
        CancellationToken cancellationToken);
}

public sealed class AtualizacaoContextoRespostaPorTurmaAlunoOutput
{
    public required bool Sucesso { get; init; }
    public required int LinhasAtualizadas { get; init; }
}
