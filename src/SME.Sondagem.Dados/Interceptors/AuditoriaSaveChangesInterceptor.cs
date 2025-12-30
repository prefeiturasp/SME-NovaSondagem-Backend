using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Infrastructure.Interfaces;

namespace SME.Sondagem.Dados.Interceptors;

public class AuditoriaSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IServicoUsuario? _servicoUsuario;
    private List<Auditoria>? _auditoriasPreSave;
    private List<(EntidadeBase Entidade, string NomeEntidade)>? _entidadesInsert;

    public AuditoriaSaveChangesInterceptor(IServicoUsuario? servicoUsuario)
    {
        _servicoUsuario = servicoUsuario;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            PrepararAuditorias(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            PrepararAuditorias(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        SalvarAuditorias(eventData.Context);
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await SalvarAuditoriasAsync(eventData.Context, cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void PrepararAuditorias(DbContext context)
    {
        _auditoriasPreSave = CapturarAuditoriasPreSave(context.ChangeTracker);
        _entidadesInsert = CapturarEntidadesInsert(context.ChangeTracker);

        AtualizarAuditoriaEntidades(context.ChangeTracker);
    }

    private void SalvarAuditorias(DbContext? context)
    {
        if (context is null)
            return;

        var auditoriasInsert = CriarAuditoriasInsert(_entidadesInsert ?? new List<(EntidadeBase, string)>());

        var todasAuditorias = (_auditoriasPreSave ?? new List<Auditoria>())
            .Concat(auditoriasInsert)
            .ToList();

        if (todasAuditorias.Any())
        {
            context.Set<Auditoria>().AddRange(todasAuditorias);
            context.SaveChanges();
        }

        _auditoriasPreSave = null;
        _entidadesInsert = null;
    }

    private async Task SalvarAuditoriasAsync(DbContext? context, CancellationToken cancellationToken)
    {
        if (context is null)
            return;

        var auditoriasInsert = CriarAuditoriasInsert(_entidadesInsert ?? new List<(EntidadeBase, string)>());

        var todasAuditorias = (_auditoriasPreSave ?? new List<Auditoria>())
            .Concat(auditoriasInsert)
            .ToList();

        if (todasAuditorias.Any())
        {
            await context.Set<Auditoria>().AddRangeAsync(todasAuditorias, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        _auditoriasPreSave = null;
        _entidadesInsert = null;
    }

    #region Métodos de Auditoria

    private void AtualizarAuditoriaEntidades(ChangeTracker changeTracker)
    {
        var entries = changeTracker.Entries()
            .Where(e => e.Entity is EntidadeBase &&
                   (e.State == EntityState.Added || e.State == EntityState.Modified));

        var usuarioLogado = _servicoUsuario?.ObterUsuarioLogado() ?? "Sistema";
        var rfUsuarioLogado = _servicoUsuario?.ObterRFUsuarioLogado() ?? "0";

        foreach (var entry in entries)
        {
            var entidade = (EntidadeBase)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entidade.CriadoPor = usuarioLogado;
                entidade.CriadoRF = rfUsuarioLogado;
            }
            else if (entry.State == EntityState.Modified)
            {
                entidade.AlteradoEm = DateTime.UtcNow;
                entidade.AlteradoPor = usuarioLogado;
                entidade.AlteradoRF = rfUsuarioLogado;
            }
        }
    }

    private List<Auditoria> CapturarAuditoriasPreSave(ChangeTracker changeTracker)
    {
        var auditorias = new List<Auditoria>();
        var usuarioLogado = _servicoUsuario?.ObterUsuarioLogado() ?? "Sistema";
        var rfUsuarioLogado = _servicoUsuario?.ObterRFUsuarioLogado() ?? "0";

        var entries = changeTracker.Entries()
            .Where(e => e.Entity is EntidadeBase &&
                       (e.State == EntityState.Modified || e.State == EntityState.Deleted) &&
                       e.Entity.GetType() != typeof(Auditoria) &&
                       e.Entity.GetType() != typeof(AuditoriaDetalhe))
            .ToList();

        foreach (var entry in entries)
        {
            var entidade = (EntidadeBase)entry.Entity;
            var nomeEntidade = entry.Entity.GetType().Name;
            var acao = entry.State == EntityState.Modified ? "UPDATE" : "DELETE";

            var auditoria = new Auditoria
            {
                Acao = acao,
                Chave = entidade.Id,
                Data = DateTime.UtcNow,
                Entidade = nomeEntidade,
                Id = Guid.NewGuid(),
                RF = rfUsuarioLogado,
                Usuario = usuarioLogado,
                Perfil = null,
                Administrador = usuarioLogado
            };

            if (entry.State == EntityState.Modified)
            {
                var detalhes = CapturarDetalhesAlteracao(entry, auditoria.Id!.Value);
                foreach (var detalhe in detalhes)
                {
                    auditoria.Detalhes.Add(detalhe);
                }
            }

            auditorias.Add(auditoria);
        }

        return auditorias;
    }

    private List<(EntidadeBase Entidade, string NomeEntidade)> CapturarEntidadesInsert(ChangeTracker changeTracker)
    {
        var entidades = new List<(EntidadeBase, string)>();

        var entries = changeTracker.Entries()
            .Where(e => e.Entity is EntidadeBase &&
                       e.State == EntityState.Added &&
                       e.Entity.GetType() != typeof(Auditoria) &&
                       e.Entity.GetType() != typeof(AuditoriaDetalhe))
            .ToList();

        foreach (var entry in entries)
        {
            var entidade = (EntidadeBase)entry.Entity;
            var nomeEntidade = entry.Entity.GetType().Name;
            entidades.Add((entidade, nomeEntidade));
        }

        return entidades;
    }

    private List<Auditoria> CriarAuditoriasInsert(
        List<(EntidadeBase Entidade, string NomeEntidade)> entidades)
    {
        var auditorias = new List<Auditoria>();
        var usuarioLogado = _servicoUsuario?.ObterUsuarioLogado() ?? "Sistema";
        var rfUsuarioLogado = _servicoUsuario?.ObterRFUsuarioLogado() ?? "0";

        foreach (var (entidade, nomeEntidade) in entidades)
        {
            auditorias.Add(new Auditoria
            {
                Acao = "INSERT",
                Chave = entidade.Id,
                Data = DateTime.UtcNow,
                Entidade = nomeEntidade,
                Id = Guid.NewGuid(),
                RF = rfUsuarioLogado,
                Usuario = usuarioLogado,
                Perfil = null,
                Administrador = usuarioLogado
            });
        }

        return auditorias;
    }

    private List<AuditoriaDetalhe> CapturarDetalhesAlteracao(
        EntityEntry entry,
        Guid auditoriaId)
    {
        var detalhes = new List<AuditoriaDetalhe>();

        foreach (var property in entry.Properties)
        {
            // Ignora propriedades de auditoria, navegação e controle
            if (property.Metadata.Name == nameof(EntidadeBase.Id) ||
                property.Metadata.Name == nameof(EntidadeBase.AlteradoEm) ||
                property.Metadata.Name == nameof(EntidadeBase.AlteradoPor) ||
                property.Metadata.Name == nameof(EntidadeBase.AlteradoRF) ||
                property.Metadata.Name == nameof(EntidadeBase.CriadoEm) ||
                property.Metadata.Name == nameof(EntidadeBase.CriadoPor) ||
                property.Metadata.Name == nameof(EntidadeBase.CriadoRF) ||
                property.Metadata.Name == nameof(EntidadeBase.Excluido) ||
                property.Metadata.IsForeignKey())
            {
                continue;
            }

            if (property.IsModified)
            {
                var valorAntigo = property.OriginalValue?.ToString();
                var valorNovo = property.CurrentValue?.ToString();

                if (valorAntigo != valorNovo)
                {
                    detalhes.Add(new AuditoriaDetalhe(
                        auditoriaId: auditoriaId,
                        nomePropriedade: property.Metadata.Name,
                        valorAntigo: valorAntigo,
                        valorNovo: valorNovo
                    ));
                }
            }
        }

        return detalhes;
    }

    #endregion
}