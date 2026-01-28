using SME.Sondagem.Dados.Interfaces.Elastic;

namespace SME.Sondagem.Aplicacao.Agregadores;

public class RepositoriosElastic
{
    public RepositoriosElastic(
        IRepositorioElasticTurma repositorioElasticTurma,
        IRepositorioElasticAluno repositorioElasticAluno)
    {
        RepositorioElasticTurma = repositorioElasticTurma ?? throw new ArgumentNullException(nameof(repositorioElasticTurma));
        RepositorioElasticAluno = repositorioElasticAluno ?? throw new ArgumentNullException(nameof(repositorioElasticAluno));
    }

    public IRepositorioElasticTurma RepositorioElasticTurma { get; }
    public IRepositorioElasticAluno RepositorioElasticAluno { get; }
}