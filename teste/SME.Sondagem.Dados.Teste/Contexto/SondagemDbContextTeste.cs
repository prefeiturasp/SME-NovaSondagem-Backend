using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Contexto;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Contexto
{
    public class SondagemDbContextTeste
    {
        private SondagemDbContext CriarContextoInMemory()
        {
            var options = new DbContextOptionsBuilder<SondagemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SondagemDbContext(options);
        }

        [Fact]
        public void Deve_Criar_Contexto_Com_Sucesso()
        {
            using var contexto = CriarContextoInMemory();
            Assert.NotNull(contexto);
        }

        [Fact]
        public void Deve_Acessar_DbSets()
        {
            using var contexto = CriarContextoInMemory();
            Assert.NotNull(contexto.Alunos);
            Assert.NotNull(contexto.ComponentesCurriculares);
            Assert.NotNull(contexto.Proficiencias);
            Assert.NotNull(contexto.Bimestres);
            Assert.NotNull(contexto.Questionarios);
            Assert.NotNull(contexto.GruposQuestoes);
            Assert.NotNull(contexto.OpcoesResposta);
            Assert.NotNull(contexto.Questoes);
            Assert.NotNull(contexto.QuestoesOpcoesResposta);
            Assert.NotNull(contexto.Sondagens);
            Assert.NotNull(contexto.SondagemPeriodosBimestre);
            Assert.NotNull(contexto.RespostasAluno);
            Assert.NotNull(contexto.Auditorias);
            Assert.NotNull(contexto.AuditoriasDetalhes);
            Assert.NotNull(contexto.QuestionariosBimestres);
        }
    }
}
