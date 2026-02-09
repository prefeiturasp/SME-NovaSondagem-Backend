using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio.Enums;
using Xunit;
using Proficiencia = SME.Sondagem.Dominio.Entidades.Proficiencia;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioProficienciaTeste : RepositorioBaseTeste
    {
        private static readonly string[] NomesEsperadosComponente1 = ["Escrita", "Números"];
        private const int ModalidadeId = (int)Modalidade.Fundamental;

        [Fact]
        public async Task ObterTodosAsync_deve_retornar_somente_proficiencias_nao_excluidas_ordenadas_por_nome()
        {
            var context = CriarContexto(nameof(ObterTodosAsync_deve_retornar_somente_proficiencias_nao_excluidas_ordenadas_por_nome));

            context.Proficiencias.AddRange(
                new Proficiencia("Avançado", 1,ModalidadeId),
                new Proficiencia("Básico", 1,ModalidadeId),
                new Proficiencia("Intermediário", 1,ModalidadeId)
                {
                    Excluido = true
                }
            );

            await context.SaveChangesAsync();

            var servicoAuditoria = CriarServicoAuditoria();
            var contextoBase = CriarConextoBase();
            var repositorio = new RepositorioProficiencia(context, servicoAuditoria, contextoBase);

            var resultado = await repositorio.ListarAsync();

            var lista = resultado.ToList();

            Assert.Equal(2, lista.Count);
            Assert.Equal("Avançado", lista[0].Nome);
            Assert.Equal("Básico", lista[1].Nome);
        }

        [Fact]
        public async Task ObterPorIdAsync_deve_retornar_proficiencia_quando_existir_e_nao_estiver_excluida()
        {
            var context = CriarContexto(nameof(ObterPorIdAsync_deve_retornar_proficiencia_quando_existir_e_nao_estiver_excluida));

            var proficiencia = new Proficiencia("Básico", 2,ModalidadeId);
            context.Proficiencias.Add(proficiencia);
            await context.SaveChangesAsync();

            var servicoAuditoria = CriarServicoAuditoria();
            var contextoBase = CriarConextoBase();
            var repositorio = new RepositorioProficiencia(context, servicoAuditoria, contextoBase);

            var resultado = await repositorio.ObterPorIdAsync(proficiencia.Id);

            Assert.NotNull(resultado);
            Assert.Equal("Básico", resultado!.Nome);
        }

        [Fact]
        public async Task ObterPorIdAsync_deve_retornar_null_quando_nao_existir()
        {
            var context = CriarContexto(nameof(ObterPorIdAsync_deve_retornar_null_quando_nao_existir));
            var servicoAuditoria = CriarServicoAuditoria();
            var contextoBase = CriarConextoBase();
            var repositorio = new RepositorioProficiencia(context, servicoAuditoria, contextoBase);

            var resultado = await repositorio.ObterPorIdAsync(999);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObterPorIdAsync_deve_retornar_null_quando_estiver_excluida()
        {
            var context = CriarContexto(nameof(ObterPorIdAsync_deve_retornar_null_quando_estiver_excluida));

            var proficiencia = new Proficiencia("Intermediário", 3,ModalidadeId)
            {
                Excluido = true
            };

            context.Proficiencias.Add(proficiencia);
            await context.SaveChangesAsync();

            var servicoAuditoria = CriarServicoAuditoria();
            var contextoBase = CriarConextoBase();
            var repositorio = new RepositorioProficiencia(context, servicoAuditoria, contextoBase);

            var resultado = await repositorio.ObterPorIdAsync(proficiencia.Id);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task CriarAsync_deve_persistir_proficiencia_e_retornar_id()
        {
            var context = CriarContexto(nameof(CriarAsync_deve_persistir_proficiencia_e_retornar_id));

            var proficiencia = new Proficiencia("Avançado", 4,ModalidadeId);
            var servicoAuditoria = CriarServicoAuditoria();
            var contextoBase = CriarConextoBase();
            var repositorio = new RepositorioProficiencia(context, servicoAuditoria, contextoBase);

            var id = await repositorio.SalvarAsync(proficiencia);

            Assert.True(id > 0);
            Assert.Equal(1, await context.Proficiencias.CountAsync());
        }

        [Fact]
        public async Task AtualizarAsync_deve_atualizar_dados_quando_proficiencia_existir()
        {
            var context = CriarContexto(nameof(AtualizarAsync_deve_atualizar_dados_quando_proficiencia_existir));

            var proficiencia = new Proficiencia("Básico", 5,ModalidadeId);
            context.Proficiencias.Add(proficiencia);
            await context.SaveChangesAsync();

            proficiencia.AlteradoEm = DateTime.UtcNow;
            proficiencia.AlteradoPor = "Sistema";
            proficiencia.AlteradoRF = "123456";

            var servicoAuditoria = CriarServicoAuditoria();
            var contextoBase = CriarConextoBase();
            var repositorio = new RepositorioProficiencia(context, servicoAuditoria, contextoBase);

            var atualizado = await repositorio.SalvarAsync(proficiencia) > 0;

            Assert.True(atualizado);

            var proficienciaAtualizada = await context.Proficiencias.FindAsync(proficiencia.Id);
            Assert.Equal("Sistema", proficienciaAtualizada!.AlteradoPor);
        }

        [Fact]
        public async Task AtualizarAsync_deve_retornar_false_quando_proficiencia_nao_existir()
        {
            var context = CriarContexto(nameof(AtualizarAsync_deve_retornar_false_quando_proficiencia_nao_existir));
            var servicoAuditoria = CriarServicoAuditoria();
            var contextoBase = CriarConextoBase();
            var repositorio = new RepositorioProficiencia(context, servicoAuditoria, contextoBase);

            var proficiencia = new Proficiencia("Inexistente", 6,ModalidadeId)
            {
                Id = 999
            };

            var resultado = await repositorio.SalvarAsync(proficiencia) > 0;

            Assert.False(resultado);
        }

        [Fact]
        public async Task ExcluirAsync_deve_marcar_proficiencia_como_excluida()
        {
            var context = CriarContexto(nameof(ExcluirAsync_deve_marcar_proficiencia_como_excluida));

            var proficiencia = new Proficiencia("Excluir", 7,ModalidadeId);
            context.Proficiencias.Add(proficiencia);
            await context.SaveChangesAsync();

            var servicoAuditoria = CriarServicoAuditoria();
            var contextoBase = CriarConextoBase();
            var repositorio = new RepositorioProficiencia(context, servicoAuditoria, contextoBase);

            var excluido = await repositorio.RemoverLogico(proficiencia.Id)>0;

            Assert.True(excluido);

            var proficienciaExcluida = await context.Proficiencias.FindAsync(proficiencia.Id);
            Assert.True(proficienciaExcluida!.Excluido);
        }

        [Fact]
        public async Task ExcluirAsync_deve_retornar_false_quando_proficiencia_nao_existir()
        {
            var context = CriarContexto(nameof(ExcluirAsync_deve_retornar_false_quando_proficiencia_nao_existir));
            var servicoAuditoria = CriarServicoAuditoria();
            var contextoBase = CriarConextoBase();
            var repositorio = new RepositorioProficiencia(context, servicoAuditoria, contextoBase);

            var resultado = await repositorio.RemoverLogico(999) > 0;

            Assert.False(resultado);
        }

        [Fact]
        public async Task ObterProeficienciaPorComponenteCurricular_DeveRetornarApenasProficienciasDoComponenteInformado()
        {
            var contexto = CriarContexto(nameof(ObterProeficienciaPorComponenteCurricular_DeveRetornarApenasProficienciasDoComponenteInformado));

            var prof1 = new Proficiencia("Escrita", 1,ModalidadeId);
            var prof2 = new Proficiencia("Números", 1,ModalidadeId);
            var prof3 = new Proficiencia("Mapeamento dos saberes", 2,ModalidadeId);
            var prof4 = new Proficiencia("Leitura", 1,ModalidadeId);
            prof4.Excluido = true;

            contexto.Proficiencias.AddRange(prof1, prof2, prof3, prof4);
            await contexto.SaveChangesAsync();

            var servicoAuditoria = CriarServicoAuditoria();
            var contextoBase = CriarConextoBase();
            var repositorio = new RepositorioProficiencia(contexto, servicoAuditoria, contextoBase);

            var resultado = await repositorio.ObterProeficienciaPorComponenteCurricular(1,ModalidadeId);

            var lista = resultado.ToList();

            Assert.Equal(2, lista.Count);

            Assert.All(lista, p =>
            {
                Assert.Equal(1, p.ComponenteCurricularId);
            });

            Assert.Equal(
                NomesEsperadosComponente1,
                lista.Select(p => p.Nome).ToArray()
            );
        }
    }
}
