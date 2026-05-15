using Microsoft.EntityFrameworkCore;
using Xunit;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class QuestionarioMapTeste
    {
        [Fact]
        public void Deve_Mapear_Propriedades_E_Relacionamentos_Corretamente()
        {
            var options = new DbContextOptionsBuilder<SondagemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new SondagemDbContext(options);

            var model = context.Model.FindEntityType(typeof(Questionario));
            Assert.NotNull(model);

            // Propriedades principais
            Assert.NotNull(model.FindProperty("Id"));
            Assert.NotNull(model.FindProperty("Nome"));
            Assert.NotNull(model.FindProperty("AnoLetivo"));
            Assert.NotNull(model.FindProperty("ComponenteCurricularId"));
            Assert.NotNull(model.FindProperty("ProficienciaId"));
            Assert.NotNull(model.FindProperty("SondagemId"));
            Assert.NotNull(model.FindProperty("Tipo"));
            Assert.NotNull(model.FindProperty("Excluido"));

            // Auditoria
            Assert.NotNull(model.FindProperty("CriadoEm"));
            Assert.NotNull(model.FindProperty("CriadoPor"));
            Assert.NotNull(model.FindProperty("AlteradoEm"));
            Assert.NotNull(model.FindProperty("AlteradoPor"));
            Assert.NotNull(model.FindProperty("CriadoRF"));
            Assert.NotNull(model.FindProperty("AlteradoRF"));

            // Relacionamentos
            Assert.Contains(model.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType.Name == "ComponenteCurricular");
            Assert.Contains(model.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType.Name == "Proficiencia");
            Assert.Contains(model.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType.Name == "Sondagem");
        }
    }
}
