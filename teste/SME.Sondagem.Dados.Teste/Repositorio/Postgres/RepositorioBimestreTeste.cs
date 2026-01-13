using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio.Entidades;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioBimestreTeste : RepositorioBaseTeste
    {
        #region Helpers

        private static Bimestre CriarBimestre(
            int id,
            bool excluido = false,
            string descricao = "1º Bimestre")
        {
            var bimestre = new Bimestre(
                codBimestreEnsinoEol: 1,
                descricao: descricao
            );

            typeof(Bimestre).GetProperty("Id")!.SetValue(bimestre, id);
            typeof(Bimestre).GetProperty("Excluido")!.SetValue(bimestre, excluido);

            return bimestre;
        }

        #endregion

        #region Construtor

        [Fact]
        public void Construtor_deve_lancar_excecao_quando_contexto_nulo()
        {
            Assert.Throws<ArgumentNullException>(() => new RepositorioBimestre(null!));
        }

        #endregion

        #region ObterTodosAsync

        [Fact]
        public async Task ObterTodosAsync_deve_retornar_apenas_bimestres_nao_excluidos_ordenados_por_descricao()
        {
            var context = CriarContexto(nameof(ObterTodosAsync_deve_retornar_apenas_bimestres_nao_excluidos_ordenados_por_descricao));

            context.Bimestres.AddRange(
                CriarBimestre(1, descricao: "2º Bimestre"),
                CriarBimestre(2, descricao: "1º Bimestre"),
                CriarBimestre(3, excluido: true)
            );

            await context.SaveChangesAsync();

            var repo = new RepositorioBimestre(context);

            var resultado = (await repo.ObterTodosAsync()).ToList();

            Assert.Equal(2, resultado.Count);
            Assert.Equal("1º Bimestre", resultado[0].Descricao);
            Assert.Equal("2º Bimestre", resultado[1].Descricao);
        }

        #endregion

        #region ObterPorIdAsync

        [Fact]
        public async Task ObterPorIdAsync_deve_retornar_bimestre_quando_existir()
        {
            var context = CriarContexto(nameof(ObterPorIdAsync_deve_retornar_bimestre_quando_existir));

            context.Bimestres.Add(CriarBimestre(1));
            await context.SaveChangesAsync();

            var repo = new RepositorioBimestre(context);

            var resultado = await repo.ObterPorIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado!.Id);
        }

        [Fact]
        public async Task ObterPorIdAsync_deve_retornar_null_quando_excluido()
        {
            var context = CriarContexto(nameof(ObterPorIdAsync_deve_retornar_null_quando_excluido));

            context.Bimestres.Add(CriarBimestre(1, excluido: true));
            await context.SaveChangesAsync();

            var repo = new RepositorioBimestre(context);

            var resultado = await repo.ObterPorIdAsync(1);

            Assert.Null(resultado);
        }

        #endregion

        #region CriarAsync

        [Fact]
        public async Task CriarAsync_deve_persistir_e_retornar_id()
        {
            var context = CriarContexto(nameof(CriarAsync_deve_persistir_e_retornar_id));
            var repo = new RepositorioBimestre(context);

            var bimestre = new Bimestre(1, "1º Bimestre");

            var id = await repo.CriarAsync(bimestre);

            Assert.True(id > 0);
            Assert.Single(context.Bimestres);
        }

        #endregion

        #region AtualizarAsync

        [Fact]
        public async Task AtualizarAsync_deve_retornar_false_quando_nao_existir()
        {
            var context = CriarContexto(nameof(AtualizarAsync_deve_retornar_false_quando_nao_existir));
            var repo = new RepositorioBimestre(context);

            var bimestre = CriarBimestre(99);

            var resultado = await repo.AtualizarAsync(bimestre);

            Assert.False(resultado);
        }

        [Fact]
        public async Task AtualizarAsync_deve_atualizar_campos_de_auditoria_quando_existir()
        {
            var context = CriarContexto(nameof(AtualizarAsync_deve_atualizar_campos_de_auditoria_quando_existir));

            var bimestre = CriarBimestre(1);
            context.Bimestres.Add(bimestre);
            await context.SaveChangesAsync();

            bimestre.AlteradoEm = DateTime.UtcNow;
            bimestre.AlteradoPor = "TESTE";
            bimestre.AlteradoRF = "123";

            var repo = new RepositorioBimestre(context);

            var resultado = await repo.AtualizarAsync(bimestre);

            Assert.True(resultado);
        }

        #endregion

        #region ExcluirAsync

        [Fact]
        public async Task ExcluirAsync_deve_retornar_false_quando_nao_existir()
        {
            var context = CriarContexto(nameof(ExcluirAsync_deve_retornar_false_quando_nao_existir));
            var repo = new RepositorioBimestre(context);

            var resultado = await repo.ExcluirAsync(1);

            Assert.False(resultado);
        }

        [Fact]
        public async Task ExcluirAsync_deve_marcar_bimestre_como_excluido()
        {
            var context = CriarContexto(nameof(ExcluirAsync_deve_marcar_bimestre_como_excluido));

            var bimestre = CriarBimestre(1);
            context.Bimestres.Add(bimestre);
            await context.SaveChangesAsync();

            var repo = new RepositorioBimestre(context);

            var resultado = await repo.ExcluirAsync(1);

            var bimestreExcluido = await context.Bimestres
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(b => b.Id == 1);

            Assert.True(resultado);
            Assert.NotNull(bimestreExcluido);
            Assert.True(bimestreExcluido!.Excluido);
        }

        #endregion
    }
}
