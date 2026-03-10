namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio
{
    public interface IObterSondagemRelatorioPorTodasTurmaUseCase
    {
        public Task<MemoryStream> ObterSondagemRelatorio(CancellationToken cancellationToken);
    }
}
