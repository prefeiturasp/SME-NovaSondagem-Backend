using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.Agregadores;

public class RepositoriosSondagem
{
    public RepositoriosSondagem(
        IRepositorioSondagem repositorioSondagem,
        IRepositorioQuestao repositorioQuestao,
        IRepositorioRespostaAluno repositorioRespostaAluno,
        IRepositorioBimestre repositorioBimestre)
    {
        RepositorioSondagem = repositorioSondagem ?? throw new ArgumentNullException(nameof(repositorioSondagem));
        RepositorioQuestao = repositorioQuestao ?? throw new ArgumentNullException(nameof(repositorioQuestao));
        RepositorioRespostaAluno = repositorioRespostaAluno ?? throw new ArgumentNullException(nameof(repositorioRespostaAluno));
        RepositorioBimestre = repositorioBimestre ?? throw new ArgumentNullException(nameof(repositorioBimestre));
    }

    public IRepositorioSondagem RepositorioSondagem { get; }
    public IRepositorioQuestao RepositorioQuestao { get; }
    public IRepositorioRespostaAluno RepositorioRespostaAluno { get; }
    public IRepositorioBimestre RepositorioBimestre { get; }
}