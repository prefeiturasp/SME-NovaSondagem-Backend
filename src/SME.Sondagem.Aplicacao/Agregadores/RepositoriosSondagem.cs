using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.Agregadores;

#pragma warning disable S107 // Methods should not have too many parameters
public class RepositoriosSondagem
{
    public RepositoriosSondagem(
        IRepositorioSondagem repositorioSondagem,
        IRepositorioQuestao repositorioQuestao,
        IRepositorioRespostaAluno repositorioRespostaAluno,
        IRepositorioBimestre repositorioBimestre,
        IRepositorioComponenteCurricular componenteCurricular,
        IRepositorioProficiencia repositorioProficiencia,
        IRepositorioRacaCor repositorioRacaCor,
        IRepositorioGeneroSexo repositorioGeneroSexo)
    {
        RepositorioSondagem = repositorioSondagem ?? throw new ArgumentNullException(nameof(repositorioSondagem));
        RepositorioQuestao = repositorioQuestao ?? throw new ArgumentNullException(nameof(repositorioQuestao));
        RepositorioRespostaAluno = repositorioRespostaAluno ?? throw new ArgumentNullException(nameof(repositorioRespostaAluno));
        RepositorioBimestre = repositorioBimestre ?? throw new ArgumentNullException(nameof(repositorioBimestre));
        RepositorioComponenteCurricular = componenteCurricular ?? throw new ArgumentNullException(nameof(componenteCurricular));
        RepositorioProficiencia = repositorioProficiencia ?? throw new ArgumentNullException(nameof(repositorioProficiencia));
        RepositorioRacaCor = repositorioRacaCor ?? throw new ArgumentNullException(nameof(repositorioRacaCor));
        RepositorioGeneroSexo = repositorioGeneroSexo ?? throw new ArgumentNullException(nameof(repositorioGeneroSexo));
    }
#pragma warning restore S107 // Methods should not have too many parameters

    public IRepositorioSondagem RepositorioSondagem { get; }
    public IRepositorioQuestao RepositorioQuestao { get; }
    public IRepositorioRespostaAluno RepositorioRespostaAluno { get; }
    public IRepositorioBimestre RepositorioBimestre { get; }
    public IRepositorioComponenteCurricular RepositorioComponenteCurricular { get; }
    public IRepositorioProficiencia RepositorioProficiencia { get; }
    public IRepositorioRacaCor RepositorioRacaCor { get; }
    public IRepositorioGeneroSexo RepositorioGeneroSexo { get; }
}