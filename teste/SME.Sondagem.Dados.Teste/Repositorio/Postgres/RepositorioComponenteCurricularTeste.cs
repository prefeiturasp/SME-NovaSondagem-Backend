using Microsoft.EntityFrameworkCore;
using Moq;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Infra.Contexto;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres
{
    public class RepositorioComponenteCurricularTeste : RepositorioBaseTeste
    {

        private static SondagemDbContext CriarContextoEmMemoria()
        {
            var options = new DbContextOptionsBuilder<SondagemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SondagemDbContext(options);
        }

        [Fact]
        public async Task DeveObterComponentePorCodigoEolComSucesso()
        {
            using var context = CriarContextoEmMemoria();
            var componente = new ComponenteCurricular("Matemática", 2024, "Fundamental", 123);

            context.Set<ComponenteCurricular>().Add(componente);
            await context.SaveChangesAsync();

            var servicoAuditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBaseMock = new Mock<ContextoBase>();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoriaMock.Object, contextoBaseMock.Object);
            var resultado = await repositorio.ObterPorCodigoEolAsync(123);

            Assert.NotNull(resultado);
            Assert.Equal("Matemática", resultado.Nome);
            Assert.Equal(123, resultado.CodigoEol);
        }

        [Fact]
        public async Task DeveRetornarNuloQuandoComponenteNaoEncontrado()
        {
            using var context = CriarContextoEmMemoria();
            var servicoAuditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBaseMock = new Mock<ContextoBase>();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoriaMock.Object, contextoBaseMock.Object);
            var resultado = await repositorio.ObterPorCodigoEolAsync(999);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task DeveObterComponentePorNomeEModalidade()
        {
            using var context = CriarContextoEmMemoria();
            var componente = new ComponenteCurricular("Português", 2024, "EJA", 456);

            context.Set<ComponenteCurricular>().Add(componente);
            await context.SaveChangesAsync();

            var servicoAuditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBaseMock = new Mock<ContextoBase>();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoriaMock.Object, contextoBaseMock.Object);
            var resultado = await repositorio.ObterPorNomeModalidade("Português", "EJA");

            Assert.NotNull(resultado);
            Assert.Equal("EJA", resultado.Modalidade);
            Assert.Equal("Português", resultado.Nome);
        }

        [Fact]
        public async Task DeveObterComponentePorNomeQuandoModalidadeNula()
        {
            using var context = CriarContextoEmMemoria();
            var componente = new ComponenteCurricular("Português", 2024, string.Empty, 789);

            context.Set<ComponenteCurricular>().Add(componente);
            await context.SaveChangesAsync();

            var servicoAuditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBaseMock = new Mock<ContextoBase>();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoriaMock.Object, contextoBaseMock.Object);
            var resultado = await repositorio.ObterPorNomeModalidade("Português", null);

            Assert.NotNull(resultado);
            Assert.Equal("Português", resultado.Nome);
        }

        [Fact]
        public async Task DeveListarComponentesPorModalidade()
        {
            using var context = CriarContextoEmMemoria();
            var componentes = new List<ComponenteCurricular>
        {
            new("Matemática", 2024, "Regular", 101),
            new("Português", 2024, "Regular", 102),
            new("História", 2024, "EJA", 103)
        };

            context.Set<ComponenteCurricular>().AddRange(componentes);
            await context.SaveChangesAsync();

            var servicoAuditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBaseMock = new Mock<ContextoBase>();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoriaMock.Object, contextoBaseMock.Object);
            var resultado = await repositorio.ObterPorModalidadeAsync("Regular");

            Assert.NotEmpty(resultado);
            Assert.Equal(2, resultado.Count());
            Assert.All(resultado, c => Assert.Equal("Regular", c.Modalidade));
        }

        [Fact]
        public async Task DeveListarComponentesPorAno()
        {
            using var context = CriarContextoEmMemoria();
            var componentes = new List<ComponenteCurricular>
            {
                new("Matemática", 2024, "Fundamental", 201),
                new("Português", 2024, "Fundamental", 202),
                new("História", 2023, "Fundamental", 203)
            };

            context.Set<ComponenteCurricular>().AddRange(componentes);
            await context.SaveChangesAsync();

            var servicoAuditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBaseMock = new Mock<ContextoBase>();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoriaMock.Object, contextoBaseMock.Object);
            var resultado = await repositorio.ObterPorAnoAsync(2024);

            Assert.NotEmpty(resultado);
            Assert.Equal(2, resultado.Count());
            Assert.All(resultado, c => Assert.Equal(2024, c.Ano));
        }

        [Fact]
        public async Task DeveVerificarSeExisteComponenteComCodigoEol()
        {
            using var context = CriarContextoEmMemoria();
            var componente = new ComponenteCurricular("Matemática", 2024, "Fundamental", 301);

            context.Set<ComponenteCurricular>().Add(componente);
            await context.SaveChangesAsync();

            var servicoAuditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBaseMock = new Mock<ContextoBase>();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoriaMock.Object, contextoBaseMock.Object);
            var existe = await repositorio.ExisteComCodigoEolAsync(301);

            Assert.True(existe);
        }

        [Fact]
        public async Task DeveRetornarFalsoQuandoComponenteNaoExiste()
        {
            using var context = CriarContextoEmMemoria();
            var servicoAuditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBaseMock = new Mock<ContextoBase>();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoriaMock.Object, contextoBaseMock.Object);
            var existe = await repositorio.ExisteComCodigoEolAsync(999);

            Assert.False(existe);
        }

        [Fact]
        public async Task DeveIgnorarIdAoVerificarSeExisteComponente()
        {
            using var context = CriarContextoEmMemoria();
            var componente = new ComponenteCurricular("Matemática", 2024, "Fundamental", 401);

            context.Set<ComponenteCurricular>().Add(componente);
            await context.SaveChangesAsync();

            var servicoAuditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBaseMock = new Mock<ContextoBase>();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoriaMock.Object, contextoBaseMock.Object);

            // Obtém o ID do componente inserido
            var componenteInserido = context.Set<ComponenteCurricular>().First();
            var existe = await repositorio.ExisteComCodigoEolAsync(401, idIgnorar: componenteInserido.Id);

            Assert.False(existe);
        }

        [Fact]
        public async Task DeveListarTodosComponentesOrdenados()
        {
            using var context = CriarContextoEmMemoria();
            var componentes = new List<ComponenteCurricular>
            {
                new("Português", 2024, "Fundamental", 501),
                new("Matemática", 2024, "Fundamental", 502),
                new("História", 2024, "Fundamental", 503)
            };

            context.Set<ComponenteCurricular>().AddRange(componentes);
            await context.SaveChangesAsync();

            var servicoAuditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBaseMock = new Mock<ContextoBase>();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoriaMock.Object, contextoBaseMock.Object);
            var resultado = await repositorio.ListarAsync();

            Assert.NotEmpty(resultado);
            var lista = resultado.ToList();

            // Verifica se está ordenado por nome
            Assert.Equal("História", lista[0].Nome);
            Assert.Equal("Matemática", lista[1].Nome);
            Assert.Equal("Português", lista[2].Nome);
        }

        [Fact]
        public async Task DeveCarregarModalidadeComponenteCurricularAoListar()
        {
            using var context = CriarContextoEmMemoria();
            var componente = new ComponenteCurricular("Matemática", 2024, "Fundamental", 601);

            context.Set<ComponenteCurricular>().Add(componente);
            await context.SaveChangesAsync();

            var servicoAuditoriaMock = new Mock<IServicoAuditoria>();
            var contextoBaseMock = new Mock<ContextoBase>();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoriaMock.Object, contextoBaseMock.Object);
            var resultado = await repositorio.ListarAsync();

            Assert.NotEmpty(resultado);
            var componentes = resultado.ToList();
            Assert.NotNull(componentes[0].ModalidadeComponenteCurricular);
        }

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
        public async Task ObterPorModalidadeAsync_deve_retornar_somente_componentes_da_modalidade_informada()
        {
            var context = CriarContexto(nameof(ObterPorModalidadeAsync_deve_retornar_somente_componentes_da_modalidade_informada));
            var servicoAuditoria = CriarServicoAuditoria();
            var conextoBase = CriarConextoBase();

            context.ComponentesCurriculares.AddRange(
                new ComponenteCurricular("Língua Portuguesa", 1, "EF", 10),
                new ComponenteCurricular("Língua Portuguesa", 1, "EJA", 10),
                new ComponenteCurricular("Matemática", 1, "EF", 20)
            );

            await context.SaveChangesAsync();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoria, conextoBase);

            var resultado = await repositorio.ObterPorModalidadeAsync("EJA");

            var lista = resultado.ToList();
            Assert.Single(lista);
            Assert.Equal("EJA", lista[0].Modalidade);
            Assert.Equal("Língua Portuguesa", lista[0].Nome);
        }

        [Fact]
        public async Task ObterPorModalidadeAsync_deve_retornar_lista_vazia_quando_nao_existir()
        {
            var context = CriarContexto(nameof(ObterPorModalidadeAsync_deve_retornar_lista_vazia_quando_nao_existir));
            var servicoAuditoria = CriarServicoAuditoria();
            var conextoBase = CriarConextoBase();

            context.ComponentesCurriculares.AddRange(
                new ComponenteCurricular("Língua Portuguesa", 1, "EF", 10),
                new ComponenteCurricular("Matemática", 1, "EF", 20)
            );

            await context.SaveChangesAsync();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoria, conextoBase);

            var resultado = await repositorio.ObterPorModalidadeAsync("EJA");

            Assert.Empty(resultado);
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

        [Fact]
        public async Task ListarAsync_deve_retornar_componentes_com_modalidades_ordenados_por_nome()
        {
            var context = CriarContexto(nameof(ListarAsync_deve_retornar_componentes_com_modalidades_ordenados_por_nome));
            var servicoAuditoria = CriarServicoAuditoria();
            var conextoBase = CriarConextoBase();

            var portugues = new ComponenteCurricular("Português", 5, "EF", 10);
            var matematica = new ComponenteCurricular("Matemática", 5, "EF", 20);

            context.ComponentesCurriculares.AddRange(portugues, matematica);
            await context.SaveChangesAsync();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoria, conextoBase);

            var resultado = await repositorio.ListarAsync();

            var lista = resultado.ToList();

            Assert.Equal(2, lista.Count);
            Assert.Equal("Matemática", lista[0].Nome);
            Assert.Equal("Português", lista[1].Nome);
            Assert.NotNull(lista[0].ModalidadeComponenteCurricular);
            Assert.NotNull(lista[1].ModalidadeComponenteCurricular);
        }

        [Fact]
        public async Task ListarAsync_deve_retornar_lista_vazia_quando_nao_houver_componentes()
        {
            var context = CriarContexto(nameof(ListarAsync_deve_retornar_lista_vazia_quando_nao_houver_componentes));
            var servicoAuditoria = CriarServicoAuditoria();
            var conextoBase = CriarConextoBase();

            var repositorio = new RepositorioComponenteCurricular(context, servicoAuditoria, conextoBase);

            var resultado = await repositorio.ListarAsync();

            Assert.Empty(resultado);
        }
    }
}