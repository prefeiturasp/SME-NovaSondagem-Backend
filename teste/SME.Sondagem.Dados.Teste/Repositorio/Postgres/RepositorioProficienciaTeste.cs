using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio.Entidades;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioProficienciaTeste : RepositorioBaseTeste
    {
        [Fact]
        public async Task ObterTodosAsync_deve_retornar_somente_proficiencias_nao_excluidas_ordenadas_por_nome()
        {
            var context = CriarContexto(nameof(ObterTodosAsync_deve_retornar_somente_proficiencias_nao_excluidas_ordenadas_por_nome));

            context.Proficiencias.AddRange(
                new Proficiencia("Avançado", 1),
                new Proficiencia("Básico", 1),
                new Proficiencia("Intermediário", 1)
                {
                    Excluido = true
                }
            );

            await context.SaveChangesAsync();

            var repositorio = new RepositorioProficiencia(context);

            var resultado = await repositorio.ObterTodosAsync();

            var lista = resultado.ToList();

            Assert.Equal(2, lista.Count);
            Assert.Equal("Avançado", lista[0].Nome);
            Assert.Equal("Básico", lista[1].Nome);
        }

        [Fact]
        public async Task ObterPorIdAsync_deve_retornar_proficiencia_quando_existir_e_nao_estiver_excluida()
        {
            var context = CriarContexto(nameof(ObterPorIdAsync_deve_retornar_proficiencia_quando_existir_e_nao_estiver_excluida));

            var proficiencia = new Proficiencia("Básico", 2);
            context.Proficiencias.Add(proficiencia);
            await context.SaveChangesAsync();

            var repositorio = new RepositorioProficiencia(context);

            var resultado = await repositorio.ObterPorIdAsync(proficiencia.Id);

            Assert.NotNull(resultado);
            Assert.Equal("Básico", resultado!.Nome);
        }

        [Fact]
        public async Task ObterPorIdAsync_deve_retornar_null_quando_nao_existir()
        {
            var context = CriarContexto(nameof(ObterPorIdAsync_deve_retornar_null_quando_nao_existir));
            var repositorio = new RepositorioProficiencia(context);

            var resultado = await repositorio.ObterPorIdAsync(999);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObterPorIdAsync_deve_retornar_null_quando_estiver_excluida()
        {
            var context = CriarContexto(nameof(ObterPorIdAsync_deve_retornar_null_quando_estiver_excluida));

            var proficiencia = new Proficiencia("Intermediário", 3)
            {
                Excluido = true
            };

            context.Proficiencias.Add(proficiencia);
            await context.SaveChangesAsync();

            var repositorio = new RepositorioProficiencia(context);

            var resultado = await repositorio.ObterPorIdAsync(proficiencia.Id);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task CriarAsync_deve_persistir_proficiencia_e_retornar_id()
        {
            var context = CriarContexto(nameof(CriarAsync_deve_persistir_proficiencia_e_retornar_id));

            var proficiencia = new Proficiencia("Avançado", 4);
            var repositorio = new RepositorioProficiencia(context);

            var id = await repositorio.CriarAsync(proficiencia);

            Assert.True(id > 0);
            Assert.Equal(1, await context.Proficiencias.CountAsync());
        }

        [Fact]
        public async Task AtualizarAsync_deve_atualizar_dados_quando_proficiencia_existir()
        {
            var context = CriarContexto(nameof(AtualizarAsync_deve_atualizar_dados_quando_proficiencia_existir));

            var proficiencia = new Proficiencia("Básico", 5);
            context.Proficiencias.Add(proficiencia);
            await context.SaveChangesAsync();

            proficiencia.AlteradoEm = DateTime.UtcNow;
            proficiencia.AlteradoPor = "TESTE";
            proficiencia.AlteradoRF = "123456";

            var repositorio = new RepositorioProficiencia(context);

            var atualizado = await repositorio.AtualizarAsync(proficiencia);

            Assert.True(atualizado);

            var proficienciaAtualizada = await context.Proficiencias.FindAsync(proficiencia.Id);
            Assert.Equal("TESTE", proficienciaAtualizada!.AlteradoPor);
        }

        [Fact]
        public async Task AtualizarAsync_deve_retornar_false_quando_proficiencia_nao_existir()
        {
            var context = CriarContexto(nameof(AtualizarAsync_deve_retornar_false_quando_proficiencia_nao_existir));
            var repositorio = new RepositorioProficiencia(context);

            var proficiencia = new Proficiencia("Inexistente", 6)
            {
                Id = 999
            };

            var resultado = await repositorio.AtualizarAsync(proficiencia);

            Assert.False(resultado);
        }

        [Fact]
        public async Task ExcluirAsync_deve_marcar_proficiencia_como_excluida()
        {
            var context = CriarContexto(nameof(ExcluirAsync_deve_marcar_proficiencia_como_excluida));

            var proficiencia = new Proficiencia("Excluir", 7);
            context.Proficiencias.Add(proficiencia);
            await context.SaveChangesAsync();

            var repositorio = new RepositorioProficiencia(context);

            var excluido = await repositorio.ExcluirAsync(proficiencia.Id);

            Assert.True(excluido);

            var proficienciaExcluida = await context.Proficiencias.FindAsync(proficiencia.Id);
            Assert.True(proficienciaExcluida!.Excluido);
        }

        [Fact]
        public async Task ExcluirAsync_deve_retornar_false_quando_proficiencia_nao_existir()
        {
            var context = CriarContexto(nameof(ExcluirAsync_deve_retornar_false_quando_proficiencia_nao_existir));
            var repositorio = new RepositorioProficiencia(context);

            var resultado = await repositorio.ExcluirAsync(999);

            Assert.False(resultado);
        }

        [Fact]
        public async Task ObterProeficienciaPorComponenteCurricular_DeveRetornarApenasProficienciasDoComponenteInformado()
        {
            var contexto = CriarContexto(nameof(ObterProeficienciaPorComponenteCurricular_DeveRetornarApenasProficienciasDoComponenteInformado));

            var prof1 = new Proficiencia("Escrita", 1);
            var prof2 = new Proficiencia("Números", 1);
            var prof3 = new Proficiencia("Mapeamento dos saberes", 2);
            var prof4 = new Proficiencia("Leitura", 1);
            prof4.Excluido = true;

            contexto.Proficiencias.AddRange(prof1, prof2, prof3, prof4);
            await contexto.SaveChangesAsync();

            var repositorio = new RepositorioProficiencia(contexto);

            var resultado = await repositorio.ObterProeficienciaPorComponenteCurricular(1);

            var lista = resultado.ToList();

            Assert.Equal(2, lista.Count);

            Assert.All(lista, p =>
            {
                Assert.Equal(1, p.ComponenteCurricularId);
            });

            var nomesEsperados = new[] { "Escrita", "Números" };
            Assert.Equal(
                nomesEsperados,
                lista.Select(p => p.Nome).ToArray()
            );
        }
    }
}
