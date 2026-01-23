using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dados.Teste.Services.Auditoria;
using SME.Sondagem.Dominio.Entidades;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioComponenteCurricularTeste : RepositorioBaseTeste
    {
        [Fact]
        public async Task ObterPorCodigoEolAsync_deve_retornar_componente_quando_existir()
        {
            var context = CriarContexto(nameof(ObterPorCodigoEolAsync_deve_retornar_componente_quando_existir));
            var servicoAuditoria = CriarServicoAuditoria();
            var componente = new ComponenteCurricular(
                nome: "Matemática",
                ano: 5,
                modalidade: "EF",
                codigoEol: 123
            );

            context.ComponentesCurriculares.Add(componente);
            await context.SaveChangesAsync();
            var conextoBase = CriarConextoBase();
            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoria, conextoBase);

            var resultado = await repositorio.ObterPorCodigoEolAsync(123);

            Assert.NotNull(resultado);
            Assert.Equal("Matemática", resultado!.Nome);
            Assert.Equal(5, resultado.Ano);
            Assert.Equal(123, resultado.CodigoEol);
        }

        [Fact]
        public async Task ObterPorCodigoEolAsync_deve_retornar_null_quando_nao_existir()
        {
            var context = CriarContexto(nameof(ObterPorCodigoEolAsync_deve_retornar_null_quando_nao_existir));
            var servicoAuditoria = CriarServicoAuditoria();
            var conextoBase = CriarConextoBase();
            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoria, conextoBase);

            var resultado = await repositorio.ObterPorCodigoEolAsync(999);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObterPorAnoAsync_deve_retornar_somente_componentes_do_ano_informado_ordenados_por_nome()
        {
            var context =
                CriarContexto(
                    nameof(ObterPorAnoAsync_deve_retornar_somente_componentes_do_ano_informado_ordenados_por_nome));

            var servicoAuditoria = CriarServicoAuditoria();
            var conextoBase = CriarConextoBase();
            context.ComponentesCurriculares.AddRange(
                new ComponenteCurricular("Português", 5, "EF", 10),
                new ComponenteCurricular("Matemática", 5, "EF", 20),
                new ComponenteCurricular("História", 4, "EF", 30)
            );

            await context.SaveChangesAsync();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoria, conextoBase);

            var resultado = await repositorio.ObterPorAnoAsync(5);

            var lista = resultado.ToList();

            Assert.Equal(2, lista.Count);
            Assert.Equal("Matemática", lista[0].Nome);
            Assert.Equal("Português", lista[1].Nome);
        }

        [Fact]
        public async Task ExisteComCodigoEolAsync_deve_retornar_true_quando_codigo_existir()
        {
            var context = CriarContexto(nameof(ExisteComCodigoEolAsync_deve_retornar_true_quando_codigo_existir));
            var conextoBase = CriarConextoBase();
            var servicoAuditoria = CriarServicoAuditoria();
            context.ComponentesCurriculares.Add(
                new ComponenteCurricular("Ciências", 6, "EF", 555)
            );

            await context.SaveChangesAsync();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoria, conextoBase);

            var existe = await repositorio.ExisteComCodigoEolAsync(555);

            Assert.True(existe);
        }

        [Fact]
        public async Task ExisteComCodigoEolAsync_deve_retornar_false_quando_codigo_nao_existir()
        {
            var context = CriarContexto(nameof(ExisteComCodigoEolAsync_deve_retornar_false_quando_codigo_nao_existir));
            var conextoBase = CriarConextoBase();
            var servicoAuditoria = CriarServicoAuditoria();
            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoria, conextoBase);

            var existe = await repositorio.ExisteComCodigoEolAsync(999);

            Assert.False(existe);
        }

        [Fact]
        public async Task ExisteComCodigoEolAsync_deve_ignorar_id_informado()
        {
            var context = CriarContexto(nameof(ExisteComCodigoEolAsync_deve_ignorar_id_informado));
            var conextoBase = CriarConextoBase();
            var servicoAuditoria = CriarServicoAuditoria();
            var componente = new ComponenteCurricular("Geografia", 7, "EF", 777);
            context.ComponentesCurriculares.Add(componente);
            await context.SaveChangesAsync();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoria, conextoBase);

            var existe = await repositorio.ExisteComCodigoEolAsync(
                codigoEol: 777,
                idIgnorar: componente.Id
            );

            Assert.False(existe);
        }
        private ContextoFake CriarConextoBase()
        {
            var contexto = new ContextoFake();
            contexto.AdicionarVariaveis(new Dictionary<string, object>
                {
                    { "NomeUsuario", "Usuario Teste" },
                    { "RF", "123456" },
                    { "Administrador", "true" }
                });

            return contexto;
        }
    }
}