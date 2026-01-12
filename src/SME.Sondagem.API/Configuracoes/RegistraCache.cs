using SME.Sondagem.Infra.EnvironmentVariables;
using StackExchange.Redis;

namespace SME.Sondagem.API.Configuracoes
{
    public static class RegistraCache
    {
        public static void Registrar(IServiceCollection services, RedisOptions redisOptions)
        {
            ValidarConfiguracao(redisOptions);

            var redisConfigurationOptions = new ConfigurationOptions()
            {
                Proxy = redisOptions.Proxy,
                SyncTimeout = redisOptions.SyncTimeout,
                EndPoints = { redisOptions!.Endpoint! }
            };

            var muxer = ConnectionMultiplexer.ConnectAsync(redisConfigurationOptions).Result;
            services.AddSingleton<IConnectionMultiplexer>(muxer);
        }

        private static void ValidarConfiguracao(RedisOptions redisOptions)
        {
            if (string.IsNullOrWhiteSpace(redisOptions?.Endpoint))
                throw new InvalidOperationException("Configuração do Redis está incompleta. O Endpoint não pode ser nulo ou vazio. Verifique o appsettings.json");
        }
    }
}
