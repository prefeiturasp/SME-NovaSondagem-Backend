using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Infrastructure.Interfaces;
using System.Linq.Expressions;

namespace SME.Sondagem.Dados.Contexto;

public class SondagemDbContext : DbContext
{
    private readonly IServicoUsuario _servicoUsuario;

    public SondagemDbContext(
        DbContextOptions<SondagemDbContext> options,
        IServicoUsuario servicoUsuario = null) : base(options)
    {
        _servicoUsuario = servicoUsuario;
    }

    // DbSets
    public DbSet<ComponenteCurricular> ComponentesCurriculares { get; set; }
    public DbSet<Proficiencia> Proficiencias { get; set; }
    public DbSet<Ciclo> Ciclos { get; set; }
    public DbSet<Aluno> Alunos { get; set; }
    public DbSet<Questionario> Questionarios { get; set; }
    public DbSet<GrupoQuestoes> GruposQuestoes { get; set; }
    public DbSet<OpcaoResposta> OpcoesResposta { get; set; }
    public DbSet<Questao> Questoes { get; set; }
    public DbSet<QuestaoOpcaoResposta> QuestoesOpcoesResposta { get; set; }
    public DbSet<Dominio.Entidades.Sondagem.Sondagem> Sondagens { get; set; }
    public DbSet<RespostaAluno> RespostasAluno { get; set; }
    public DbSet<Auditoria> Auditorias { get; set; }
    public DbSet<AuditoriaDetalhe> AuditoriasDetalhes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SondagemDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(EntidadeBase).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(EntidadeBase.Excluido));
                var filter = Expression.Lambda(Expression.Not(property), parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var auditoriasComDetalhes = CapturarAuditoriasPreSave();
        var entidadesInsert = CapturarEntidadesInsert();

        AtualizarAuditoria();

        var resultado = await base.SaveChangesAsync(cancellationToken);
        var auditoriasInsert = CriarAuditoriasInsert(entidadesInsert);

        var todasAuditorias = auditoriasComDetalhes.Concat(auditoriasInsert).ToList();

        if (todasAuditorias.Any())
        {
            await Auditorias.AddRangeAsync(todasAuditorias, cancellationToken);
            await base.SaveChangesAsync(cancellationToken);
        }

        return resultado;
    }

    public override int SaveChanges()
    {
        var auditoriasComDetalhes = CapturarAuditoriasPreSave();
        var entidadesInsert = CapturarEntidadesInsert();
        AtualizarAuditoria();

        var resultado = base.SaveChanges();

        var auditoriasInsert = CriarAuditoriasInsert(entidadesInsert);
        var todasAuditorias = auditoriasComDetalhes.Concat(auditoriasInsert).ToList();

        if (todasAuditorias.Any())
        {
            Auditorias.AddRange(todasAuditorias);
            base.SaveChanges();
        }

        return resultado;
    }

    private void AtualizarAuditoria()
    {
        var entries = ChangeTracker.Entries()
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

    private List<Auditoria> CapturarAuditoriasPreSave()
    {
        var auditorias = new List<Auditoria>();
        var usuarioLogado = _servicoUsuario?.ObterUsuarioLogado() ?? "Sistema";
        var rfUsuarioLogado = _servicoUsuario?.ObterRFUsuarioLogado() ?? "0";

        var entries = ChangeTracker.Entries()
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

            var auditoria = new Auditoria(
                acao: acao,
                chave: entidade.Id,
                data: DateTime.UtcNow,
                entidade: nomeEntidade,
                id: Guid.NewGuid(),
                rF: rfUsuarioLogado,
                usuario: usuarioLogado,
                perfil: null,
                administrador: usuarioLogado
            );

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

    private List<(EntidadeBase Entidade, string NomeEntidade)> CapturarEntidadesInsert()
    {
        var entidades = new List<(EntidadeBase, string)>();

        var entries = ChangeTracker.Entries()
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
            auditorias.Add(new Auditoria(
                acao: "INSERT",
                chave: entidade.Id,
                data: DateTime.UtcNow,
                entidade: nomeEntidade,
                id: Guid.NewGuid(),
                rF: rfUsuarioLogado,
                usuario: usuarioLogado,
                perfil: null,
                administrador: usuarioLogado
            ));
        }

        return auditorias;
    }

    private List<AuditoriaDetalhe> CapturarDetalhesAlteracao(
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry,
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
}