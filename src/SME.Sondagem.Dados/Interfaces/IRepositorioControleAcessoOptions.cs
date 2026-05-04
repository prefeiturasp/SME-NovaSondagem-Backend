using SME.Sondagem.Dominio.Entidades.Configuration;

namespace SME.Sondagem.Dados.Interfaces
{
    public interface IRepositorioControleAcessoOptions : IRepositorioBase<ControleAcessoOptions>
    {
        Task<IEnumerable<ControleAcessoOptions>> ObterTodosComPerfis();
    }
}
