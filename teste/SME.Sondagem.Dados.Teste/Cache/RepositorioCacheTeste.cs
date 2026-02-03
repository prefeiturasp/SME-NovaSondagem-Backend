using Moq;
using SME.Sondagem.Dados.Cache;
using SME.Sondagem.Infra.Interfaces;
using StackExchange.Redis;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Cache
{
    public class RepositorioCacheTeste
    {
        private readonly Mock<IServicoLog> _servicoLogMock;
        private readonly Mock<IDatabase> _databaseMock;
        private readonly Mock<IConnectionMultiplexer> _connectionMultiplexerMock;
        private readonly RepositorioCache _repositorioCache;

        public RepositorioCacheTeste()
        {
            _servicoLogMock = new Mock<IServicoLog>();
            _databaseMock = new Mock<IDatabase>();
            _connectionMultiplexerMock = new Mock<IConnectionMultiplexer>();
            _connectionMultiplexerMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_databaseMock.Object);

            _repositorioCache = new RepositorioCache(_servicoLogMock.Object, _connectionMultiplexerMock.Object);
        }

        [Fact]
        public async Task Deve_Salvar_Valor_No_Redis()
        {
            _databaseMock.Setup(x => x.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);

            await _repositorioCache.SalvarRedisAsync("chave", "valor");

            _databaseMock.Verify(x => x.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Remover_Valor_Do_Redis()
        {
            _databaseMock.Setup(x => x.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);

            await _repositorioCache.RemoverRedisAsync("chave");

            _databaseMock.Verify(x => x.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Verificar_Se_Chave_Existe()
        {
            _databaseMock.Setup(x => x.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);

            var existe = await _repositorioCache.ExisteChaveAsync("chave");

            Assert.True(existe);
        }
    }
}
