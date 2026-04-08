namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioRespostaAlunoDapper
{
    Task AtualizarCamposAsync(int id, string? turmaId, string? ueId, string? dreId, int? anoLetivo, int? modalidadeId, int? anoTurma, int? generoId, int? raca_id);
}
