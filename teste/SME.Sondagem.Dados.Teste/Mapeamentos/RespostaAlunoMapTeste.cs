using Microsoft.EntityFrameworkCore;
using Xunit;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dominio.Entidades.Sondagem;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class RespostaAlunoMapTeste
    {
        [Fact]
        public void Deve_Mapear_Propriedades_E_Relacionamentos_Corretamente()
        {
            var options = new DbContextOptionsBuilder<SondagemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new SondagemDbContext(options);

            var model = context.Model.FindEntityType(typeof(RespostaAluno));
            Assert.NotNull(model);

            // Propriedades principais
            Assert.NotNull(model.FindProperty("Id"));
            Assert.NotNull(model.FindProperty("SondagemId"));
            Assert.NotNull(model.FindProperty("AlunoId"));
            Assert.NotNull(model.FindProperty("QuestaoId"));
            Assert.NotNull(model.FindProperty("OpcaoRespostaId"));
            Assert.NotNull(model.FindProperty("DataResposta"));
            Assert.NotNull(model.FindProperty("Excluido"));
            Assert.NotNull(model.FindProperty("BimestreId"));

            // Auditoria
            Assert.NotNull(model.FindProperty("CriadoEm"));
            Assert.NotNull(model.FindProperty("CriadoPor"));
            Assert.NotNull(model.FindProperty("AlteradoEm"));
            Assert.NotNull(model.FindProperty("AlteradoPor"));
            Assert.NotNull(model.FindProperty("CriadoRF"));
            Assert.NotNull(model.FindProperty("AlteradoRF"));

            // Relacionamentos
            Assert.Contains(model.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType.Name == "Sondagem");
            Assert.Contains(model.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType.Name == "Questao");
            Assert.Contains(model.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType.Name == "OpcaoResposta");
            Assert.Contains(model.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType.Name == "Bimestre");
        }
    }
}
