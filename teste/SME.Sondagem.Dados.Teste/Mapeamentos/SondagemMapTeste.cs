using Microsoft.EntityFrameworkCore;
using Xunit;
using SME.Sondagem.Dados.Contexto;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class SondagemMapTeste
    {
        [Fact]
        public void Deve_Mapear_Propriedades_E_Relacionamentos_Corretamente()
        {
            var options = new DbContextOptionsBuilder<SondagemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new SondagemDbContext(options);

            var model = context.Model.FindEntityType(typeof(Dominio.Entidades.Sondagem.Sondagem));
            Assert.NotNull(model);

            // Propriedades principais
            Assert.NotNull(model.FindProperty("Id"));
            Assert.NotNull(model.FindProperty("Descricao"));
            Assert.NotNull(model.FindProperty("DataAplicacao"));
            Assert.NotNull(model.FindProperty("Excluido"));

            // Auditoria
            Assert.NotNull(model.FindProperty("CriadoEm"));
            Assert.NotNull(model.FindProperty("CriadoPor"));
            Assert.NotNull(model.FindProperty("AlteradoEm"));
            Assert.NotNull(model.FindProperty("AlteradoPor"));
            Assert.NotNull(model.FindProperty("CriadoRF"));
            Assert.NotNull(model.FindProperty("AlteradoRF"));

            // Relacionamentos
            Assert.Contains(model.GetNavigations(), n => n.Name == "Questionarios");
            Assert.Contains(model.GetNavigations(), n => n.Name == "Respostas");
            Assert.Contains(model.GetNavigations(), n => n.Name == "PeriodosBimestre");
        }
    }
}
