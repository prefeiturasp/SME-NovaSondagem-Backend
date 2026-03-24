using SME.Sondagem.Aplicacao.Interfaces.Services;

namespace SME.Sondagem.Aplicacao.Agregadores
{
    public class RepositorioSondagemRelatorioPorTodasTurma
    {
        public RepositorioSondagemRelatorioPorTodasTurma(IDadosAlunosService dadosAlunosService, IUeComDreEolService ueComDreEolService)
        {
            DadosAlunosService = dadosAlunosService ?? throw new ArgumentNullException(nameof(dadosAlunosService));
            UeComDreEolService = ueComDreEolService ?? throw new ArgumentNullException(nameof(ueComDreEolService));
        }

        public IDadosAlunosService DadosAlunosService { get; }
        public IUeComDreEolService UeComDreEolService { get; }
    }
}
