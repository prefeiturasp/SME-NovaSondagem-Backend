using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Questionario;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.Sondagem.Dados.Contexto
{
    public class SondagemDbContext : DbContext
    {
        public SondagemDbContext(DbContextOptions<SondagemDbContext> options) : base(options)
        {
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
        public DbSet<Dominio.Entidades.Questionario.Sondagem> Sondagens { get; set; }
        public DbSet<RespostaAluno> RespostasAluno { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar todas as configurações automaticamente
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SondagemDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AtualizarAuditoria();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            AtualizarAuditoria();
            return base.SaveChanges();
        }

        private void AtualizarAuditoria()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is EntidadeBase &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entidade = (EntidadeBase)entry.Entity;

                if (entry.State == EntityState.Modified)
                {
                    entidade.AlteradoEm = DateTime.UtcNow;
                    // AlteradoPor e AlteradoRF devem ser setados pela aplicação
                }
            }
        }
    }
}
