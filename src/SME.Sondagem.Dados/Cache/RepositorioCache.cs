using MessagePack;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Interfaces;
using StackExchange.Redis;

namespace SME.Sondagem.Dados.Cache
{
    public class RepositorioCache : IRepositorioCache
    {
        private readonly IServicoLog servicoLog;
        private readonly IDatabase database;

        public RepositorioCache(IServicoLog servicoLog, IConnectionMultiplexer connectionMultiplexer)
        {
            this.servicoLog = servicoLog ?? throw new ArgumentNullException(nameof(servicoLog));
            ArgumentNullException.ThrowIfNull(connectionMultiplexer);
            database = connectionMultiplexer.GetDatabase();
        }

        public async Task SalvarRedisAsync(string nomeChave, object valor, int minutosParaExpirar = 720)
        {
            try
            {
                if (valor != null)
                    await database.StringSetAsync(nomeChave, MessagePackSerializer.Serialize(valor), TimeSpan.FromMinutes(minutosParaExpirar));
            }
            catch (Exception ex)
            {
                servicoLog.Registrar(ex);
            }
        }

        public async Task RemoverRedisAsync(string nomeChave)
        {
            try
            {
                await database.KeyDeleteAsync(nomeChave);
            }
            catch (Exception ex)
            {
                servicoLog.Registrar(ex);
            }
        }

        public async Task<T> ObterRedisAsync<T>(string nomeChave, Func<Task<T>> buscarDados, int minutosParaExpirar = 720)
        {
            try
            {
                var byteCache = await database.StringGetAsync(nomeChave);

                if (byteCache.HasValue)
                    return MessagePackSerializer.Deserialize<T>(byteCache);

                var dados = await buscarDados();
                
                if(EqualityComparer<T>.Default.Equals(dados, default(T)))
                    await SalvarRedisAsync(nomeChave, dados!, minutosParaExpirar);

                return dados;
            }
            catch (Exception ex)
            {
                servicoLog.Registrar(ex);
                return await buscarDados();
            }
        }

        public async Task<T> ObterRedisAsync<T>(string nomeChave)
        {
            try
            {
                var byteCache = await database.StringGetAsync(nomeChave);

                if (byteCache.HasValue)
                    return MessagePackSerializer.Deserialize<T>(byteCache);
            }
            catch (Exception ex)
            {
                servicoLog.Registrar(ex);
            }

            return default!;
        }

        public async Task<bool> ExisteChaveAsync(string nomeChave)
        {
            try
            {
                return await database.KeyExistsAsync(nomeChave);
            }
            catch (Exception ex)
            {
                servicoLog.Registrar(ex);
            }

            return false;
        }
        public async Task SalvarRedisToJsonAsync(string nomeChave, string json, int minutosParaExpirar = 720)
        {
            try
            {
                if (!string.IsNullOrEmpty(json))
                {
                    var bytes = MessagePackSerializer.ConvertFromJson(json);                    
                    await database.StringSetAsync(nomeChave, bytes, TimeSpan.FromMinutes(minutosParaExpirar));
                }
            }
            catch (Exception ex)
            {
                servicoLog.Registrar(ex);
            }
        }

        public async Task<string> ObterRedisToJsonAsync(string nomeChave, Func<Task<string>> buscarDados, int minutosParaExpirar = 720)
        {
            try
            {
                var byteCache = await database.StringGetAsync(nomeChave);

                if (byteCache.HasValue)
                    return MessagePackSerializer.ConvertToJson(byteCache);

                var dados = await buscarDados();
                await SalvarRedisToJsonAsync(nomeChave, dados, minutosParaExpirar);

                return dados;
            }
            catch (Exception ex)
            {
                servicoLog.Registrar(ex);
                return await buscarDados();
            }
        }

        public async Task<string> ObterRedisToJsonAsync(string nomeChave)
        {
            try
            {
                var byteCache = await database.StringGetAsync(nomeChave);

                if (byteCache.HasValue)
                    return MessagePackSerializer.ConvertToJson(byteCache);
            }
            catch (Exception ex)
            {
                servicoLog.Registrar(ex);
            }

            return default!;
        }
    }
}
