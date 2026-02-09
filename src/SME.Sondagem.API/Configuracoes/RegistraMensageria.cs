using RabbitMQ.Client;
using SME.Sondagem.Infra.EnvironmentVariables;

namespace SME.Sondagem.API.Configuracoes
{
    public static class RegistraMensageria
    {
        public static void Registrar(IServiceCollection services, Infra.EnvironmentVariables.RabbitOptions rabbitOptions)
        {
            services.AddSingleton(_ =>
                CreateRabbitMqConnection(rabbitOptions));
        }

        private static IConnection CreateRabbitMqConnection(RabbitOptions rabbitOptions)
        {
            ValidateRabbitOptions(rabbitOptions);

            var factory = new ConnectionFactory
            {
                HostName = rabbitOptions!.HostName!,
                UserName = rabbitOptions!.UserName!,
                Password = rabbitOptions!.Password!,
                VirtualHost = rabbitOptions!.VirtualHost!
            };

            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        }

        private static void ValidateRabbitOptions(RabbitOptions options)
        {
            if (string.IsNullOrWhiteSpace(options?.HostName) ||
                string.IsNullOrWhiteSpace(options?.UserName) ||
                string.IsNullOrWhiteSpace(options?.Password) ||
                string.IsNullOrWhiteSpace(options?.VirtualHost))
            {
                throw new InvalidOperationException(
                    "Configurações do RabbitMQ estão incompletas. Verifique o appsettings.json");
            }
        }
    }
}
