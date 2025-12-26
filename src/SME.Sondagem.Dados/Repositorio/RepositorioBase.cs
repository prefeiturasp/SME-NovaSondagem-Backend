using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Repositorio;

public class RepositorioBase<T> : IRepositorioBase<T> where T : EntidadeBase
{
    private readonly SondagemDbContext _context;

    public RepositorioBase(SondagemDbContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<T>> ListarAsync()
    {
        throw new NotImplementedException();
    }

    public Task<T> ObterPorIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task RemoverAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task RemoverAsync(T entidade)
    {
        throw new NotImplementedException();
    }

    public Task<long> RemoverLogico(long id, string coluna = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoverLogico(long[] id, string coluna = null)
    {
        throw new NotImplementedException();
    }

    public async Task<long> SalvarAsync(T entidade)
    {
        if (entidade.Id > 0)
        {
            entidade.AlteradoEm = DateTimeExtension.HorarioBrasilia();
            entidade.AlteradoPor = database.UsuarioLogadoNomeCompleto;
            entidade.AlteradoRF = database.UsuarioLogadoRF;
            await _context.Database.UpdateAsync(entidade);
        }
        else
        {
            entidade.CriadoPor = database.UsuarioLogadoNomeCompleto;
            entidade.CriadoRF = database.UsuarioLogadoRF;
            entidade.Id = (long)(await database.Conexao.InsertAsync(entidade));
        }

        return entidade.Id;
    }
}