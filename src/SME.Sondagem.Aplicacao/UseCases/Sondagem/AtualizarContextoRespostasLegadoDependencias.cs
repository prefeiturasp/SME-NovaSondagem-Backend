using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem;

public sealed class AtualizarContextoRespostasLegadoDependencias
{
    public required IRepositorioRespostaAluno RepositorioRespostaAluno { get; init; }
    public required IRepositorioSondagem RepositorioSondagem { get; init; }
    public required IDadosAlunosService DadosAlunosService { get; init; }
    public required IAlunoPapService AlunoPapService { get; init; }
    public required IUeComDreEolService UeComDreEolService { get; init; }
    public required RepositorioSondagemRelatorioPorTodasTurma RepositorioSondagemRelatorioPorTodasTurma { get; init; }
    public required IRepositorioRacaCor RepositorioRacaCor { get; init; }
    public required IRepositorioGeneroSexo RepositorioGeneroSexo { get; init; }
    public required RepositoriosElastic RepositoriosElastic { get; init; }
}
