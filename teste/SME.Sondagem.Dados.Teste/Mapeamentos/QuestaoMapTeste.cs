using Microsoft.EntityFrameworkCore;
using Xunit;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Teste.Mapeamentos
{
    public class QuestaoMapTeste
    {
        [Fact]
        public void Deve_Mapear_Propriedades_E_Relacionamentos_Corretamente()
        {
            var options = new DbContextOptionsBuilder<SondagemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new SondagemDbContext(options);

            var model = context.Model.FindEntityType(typeof(Questao));
            Assert.NotNull(model);

            // Propriedades principais
            Assert.NotNull(model.FindProperty("Id"));
            Assert.NotNull(model.FindProperty("Nome"));
            Assert.NotNull(model.FindProperty("Observacao"));
            Assert.NotNull(model.FindProperty("Obrigatorio"));
            Assert.NotNull(model.FindProperty("Tipo"));
            Assert.NotNull(model.FindProperty("Opcionais"));
            Assert.NotNull(model.FindProperty("SomenteLeitura"));
            Assert.NotNull(model.FindProperty("Dimensao"));
            Assert.NotNull(model.FindProperty("Tamanho"));
            Assert.NotNull(model.FindProperty("Mascara"));
            Assert.NotNull(model.FindProperty("PlaceHolder"));
            Assert.NotNull(model.FindProperty("NomeComponente"));
            Assert.NotNull(model.FindProperty("Excluido"));

            // Relacionamentos
            Assert.Contains(model.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType == typeof(Questionario));
            Assert.Contains(model.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType.Name == "GrupoQuestoes" || fk.PrincipalEntityType.ClrType.Name == "Questao");
        }
    }
}
