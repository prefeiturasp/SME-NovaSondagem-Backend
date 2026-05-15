using Microsoft.EntityFrameworkCore;
using Xunit;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dominio.Entidades.Sondagem;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class SondagemPeriodoBimestreMapTeste
    {
        [Fact]
        public void Deve_Mapear_Propriedades_E_Relacionamentos_Corretamente()
        {
            var options = new DbContextOptionsBuilder<SondagemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new SondagemDbContext(options);

            var model = context.Model.FindEntityType(typeof(SondagemPeriodoBimestre));
            Assert.NotNull(model);

            // Propriedades principais
            Assert.NotNull(model.FindProperty("Id"));
            Assert.NotNull(model.FindProperty("BimestreId"));
            Assert.NotNull(model.FindProperty("SondagemId"));
            Assert.NotNull(model.FindProperty("DataInicio"));
            Assert.NotNull(model.FindProperty("DataFim"));
            Assert.NotNull(model.FindProperty("Excluido"));

            // Auditoria
            Assert.NotNull(model.FindProperty("CriadoEm"));
            Assert.NotNull(model.FindProperty("CriadoPor"));
            Assert.NotNull(model.FindProperty("AlteradoEm"));
            Assert.NotNull(model.FindProperty("AlteradoPor"));
            Assert.NotNull(model.FindProperty("CriadoRF"));
            Assert.NotNull(model.FindProperty("AlteradoRF"));

            // Relacionamentos
            Assert.Contains(model.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType.Name == "Bimestre");
            Assert.Contains(model.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType.Name == "Sondagem");
        }
    }
}
