using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infra.Contexto;

namespace SME.Sondagem.Dados.Services.Auditoria;

public class ServicoAuditoria : IServicoAuditoria
{
    private readonly IRepositorioAuditoria _repositorioAuditoria;
    private readonly ContextoBase _database;

    public ServicoAuditoria(IRepositorioAuditoria repositorioAuditoria, ContextoBase database)
    {
        _repositorioAuditoria = repositorioAuditoria ?? throw new ArgumentNullException(nameof(repositorioAuditoria));
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async Task AuditarAsync(string entidade, long chave, string acao)
    {
        var perfil = _database.PerfilUsuario != string.Empty ? Guid.Parse(_database.PerfilUsuario) : (Guid?)null;

        var auditoria = new Dominio.Entidades.Auditoria()
        {
            Data = DateTimeExtension.HorarioBrasilia(),
            Entidade = entidade.ToLower(),
            Chave = chave,
            Usuario = _database.NomeUsuario,
            RF = _database.UsuarioLogadoRf,
            Perfil = perfil,
            Acao = acao,
            Administrador = _database.Administrador
        };
        await _repositorioAuditoria.Salvar(auditoria);
    }

    public async Task AuditarMultiplosAsync(string entidade, IEnumerable<long> chaves, string acao)
    {
        var perfil = _database.PerfilUsuario != string.Empty ? Guid.Parse(_database.PerfilUsuario) : (Guid?)null;

        var auditorias = chaves.Select(chave => new Dominio.Entidades.Auditoria()
        {
            Data = DateTimeExtension.HorarioBrasilia(),
            Entidade = entidade.ToLower(),
            Chave = chave,
            Usuario = _database.NomeUsuario,
            RF = _database.UsuarioLogadoRf,
            Perfil = perfil,
            Acao = acao,
            Administrador = _database.Administrador
        }).ToList();

        await _repositorioAuditoria.SalvarMultiplos(auditorias);
    }
}