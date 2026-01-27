using MessagePack;
using Moq;
using SME.Sondagem.Dados.Cache;
using SME.Sondagem.Infra.Interfaces;
using StackExchange.Redis;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio
{
    public class RepositorioCacheTeste
    {
        private readonly Mock<IServicoLog> servicoLogMock;
        private readonly Mock<IConnectionMultiplexer> connectionMock;
        private readonly Mock<IDatabase> databaseMock;
        private readonly RepositorioCache repositorio;

        public RepositorioCacheTeste()
        {
            servicoLogMock = new Mock<IServicoLog>();
            connectionMock = new Mock<IConnectionMultiplexer>();
            databaseMock = new Mock<IDatabase>();

            connectionMock
                .Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(databaseMock.Object);

            repositorio = new RepositorioCache(servicoLogMock.Object, connectionMock.Object);
        }


        [Fact]
        public async Task SalvarRedisAsync_ComValor_DeveSalvar()
        {
            var obj = new { Nome = "Teste" };

            await repositorio.SalvarRedisAsync("chave", obj);

            databaseMock.Verify(d =>
                d.StringSetAsync(
                    "chave",
                    It.IsAny<RedisValue>(),
                    It.IsAny<Expiration>(),
                    It.IsAny<ValueCondition>(),
                    CommandFlags.None),
                Times.Once);
        }

        [Fact]
        public async Task SalvarRedisAsync_ValorNulo_NaoDeveSalvar()
        {
            await repositorio.SalvarRedisAsync("chave", null);

            databaseMock.Verify(d =>
                d.StringSetAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<RedisValue>(),
                    It.IsAny<TimeSpan>(),
                    false,
                    When.Always,
                    CommandFlags.None),
                Times.Never);
        }

        [Fact]
        public async Task SalvarRedisAsync_Exception_DeveLogar()
        {
            databaseMock
                .Setup(d => d.StringSetAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<RedisValue>(),
                    It.IsAny<Expiration>(),
                    It.IsAny<ValueCondition>(),
                    CommandFlags.None))
                .ThrowsAsync(new Exception("Erro"));

            await repositorio.SalvarRedisAsync("chave", new { });

            servicoLogMock.Verify(
                s => s.Registrar(It.IsAny<Exception>()),
                Times.Once);
        }


        [Fact]
        public async Task RemoverRedisAsync_DeveRemover()
        {
            await repositorio.RemoverRedisAsync("chave");

            databaseMock.Verify(d => d.KeyDeleteAsync("chave", CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task RemoverRedisAsync_Exception_DeveLogar()
        {
            databaseMock
                .Setup(d => d.KeyDeleteAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                .ThrowsAsync(new Exception());

            await repositorio.RemoverRedisAsync("chave");

            servicoLogMock.Verify(s => s.Registrar(It.IsAny<Exception>()), Times.Once);
        }

        [Fact]
        public async Task ObterRedisAsync_ComCache_DeveRetornarDoCache()
        {
            var esperado = 123;
            var bytes = MessagePackSerializer.Serialize(esperado);

            databaseMock
                .Setup(d => d.StringGetAsync("chave", CommandFlags.None))
                .ReturnsAsync(bytes);

            var resultado = await repositorio.ObterRedisAsync<int>("chave", () => Task.FromResult(0));

            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public async Task ObterRedisAsync_SemCache_DeveBuscarDados()
        {
            databaseMock
                .Setup(d => d.StringGetAsync("chave", CommandFlags.None))
                .ReturnsAsync(RedisValue.Null);

            var resultado = await repositorio.ObterRedisAsync("chave", () => Task.FromResult(10));

            Assert.Equal(10, resultado);
        }

        [Fact]
        public async Task ObterRedisAsync_Exception_DeveLogarEBuscarDados()
        {
            databaseMock
                .Setup(d => d.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                .ThrowsAsync(new Exception());

            var resultado = await repositorio.ObterRedisAsync("chave", () => Task.FromResult(5));

            Assert.Equal(5, resultado);
            servicoLogMock.Verify(s => s.Registrar(It.IsAny<Exception>()), Times.Once);
        }

        [Fact]
        public async Task ObterRedisAsync_SemFallback_ComCache()
        {
            var esperado = "teste";
            var bytes = MessagePackSerializer.Serialize(esperado);

            databaseMock
                .Setup(d => d.StringGetAsync("chave", CommandFlags.None))
                .ReturnsAsync(bytes);

            var resultado = await repositorio.ObterRedisAsync<string>("chave");

            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public async Task ObterRedisAsync_SemFallback_SemCache()
        {
            databaseMock
                .Setup(d => d.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                .ReturnsAsync(RedisValue.Null);

            var resultado = await repositorio.ObterRedisAsync<string>("chave");

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ExisteChaveAsync_True()
        {
            databaseMock
                .Setup(d => d.KeyExistsAsync("chave", CommandFlags.None))
                .ReturnsAsync(true);

            var existe = await repositorio.ExisteChaveAsync("chave");

            Assert.True(existe);
        }

        [Fact]
        public async Task ExisteChaveAsync_Exception_DeveLogar()
        {
            databaseMock
                .Setup(d => d.KeyExistsAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                .ThrowsAsync(new Exception());

            var existe = await repositorio.ExisteChaveAsync("chave");

            Assert.False(existe);
            servicoLogMock.Verify(s => s.Registrar(It.IsAny<Exception>()), Times.Once);
        }

        [Fact]
        public async Task SalvarRedisToJsonAsync_DeveSalvar()
        {
            var json = "{\"nome\":\"teste\"}";

            await repositorio.SalvarRedisToJsonAsync("chave", json);

            databaseMock.Verify(d =>
                d.StringSetAsync(
                    "chave",
                    It.IsAny<RedisValue>(),
                    It.IsAny<Expiration>(),
                    It.IsAny<ValueCondition>(),
                    CommandFlags.None),
                Times.Once);
        }

        [Fact]
        public async Task ObterRedisToJsonAsync_ComCache()
        {
            var json = "{\"nome\":\"teste\"}";
            var bytes = MessagePackSerializer.ConvertFromJson(json);

            databaseMock
                .Setup(d => d.StringGetAsync("chave", CommandFlags.None))
                .ReturnsAsync(bytes);

            var resultado = await repositorio.ObterRedisToJsonAsync("chave");

            Assert.Contains("teste", resultado);
        }

        [Fact]
        public async Task ObterRedisToJsonAsync_SemCache_DeveBuscar()
        {
            databaseMock
                .Setup(d => d.StringGetAsync("chave", CommandFlags.None))
                .ReturnsAsync(RedisValue.Null);

            var resultado = await repositorio.ObterRedisToJsonAsync(
                "chave",
                () => Task.FromResult("{\"ok\":true}")
            );

            Assert.Contains("ok", resultado);
        }

        [Fact]
        public async Task ObterRedisToJsonAsync_Exception_DeveLogarEBuscar()
        {
            databaseMock
                .Setup(d => d.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                .ThrowsAsync(new Exception());

            var resultado = await repositorio.ObterRedisToJsonAsync(
                "chave",
                () => Task.FromResult("{\"fallback\":true}")
            );

            Assert.Contains("fallback", resultado);
            servicoLogMock.Verify(s => s.Registrar(It.IsAny<Exception>()), Times.Once);
        }
    }
}
