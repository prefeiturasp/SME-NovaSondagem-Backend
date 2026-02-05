using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using System.Linq.Expressions;

namespace SME.Sondagem.Dados.Contexto;

public class SondagemDbContext : DbContext
{
    public SondagemDbContext(DbContextOptions<SondagemDbContext> options) : base(options)
    {
    }

    public DbSet<Auditoria> Auditorias { get; set; }
    public DbSet<AuditoriaDetalhe> AuditoriasDetalhes { get; set; }
    public DbSet<Aluno> Alunos { get; set; }
    public DbSet<Bimestre> Bimestres { get; set; }
    public DbSet<ComponenteCurricular> ComponentesCurriculares { get; set; }
    public DbSet<GrupoQuestoes> GruposQuestoes { get; set; }
    public DbSet<Proficiencia> Proficiencias { get; set; }
    public DbSet<Questionario> Questionarios { get; set; }
    public DbSet<OpcaoResposta> OpcoesResposta { get; set; }
    public DbSet<ParametroQuestionario> ParametrosQuestionario { get; set; }
    public DbSet<ParametroSondagem> ParametrosSondagem { get; set; }
    public DbSet<Questao> Questoes { get; set; }
    public DbSet<QuestaoOpcaoResposta> QuestoesOpcoesResposta { get; set; }
    public DbSet<QuestionarioBimestre> QuestionariosBimestres { get; set; }
    public DbSet<RespostaAluno> RespostasAluno { get; set; }
    public DbSet<Dominio.Entidades.Sondagem.Sondagem> Sondagens { get; set; }
    public DbSet<SondagemPeriodoBimestre> SondagemPeriodosBimestre { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SondagemDbContext).Assembly);

        modelBuilder.Model.GetEntityTypes()
            .Where(entityType => typeof(EntidadeBase).IsAssignableFrom(entityType.ClrType))
            .ToList()
            .ForEach(entityType =>
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(EntidadeBase.Excluido));
                var filter = Expression.Lambda(Expression.Not(property), parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetColumnType("timestamp with time zone");
                }
            }
        }
    }
}