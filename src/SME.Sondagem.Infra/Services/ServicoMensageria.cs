using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using RabbitMQ.Client;
using SME.Sondagem.Infra.EnvironmentVariables;
using SME.Sondagem.Infra.Extensions;
using SME.Sondagem.Infra.Fila;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infra.Policies;
using System.Text;

namespace SME.Sondagem.Infra.Services;

public class ServicoMensageria : IServicoMensageria
{
    private readonly RabbitOptions rabbitOptions;
    private readonly IServicoTelemetria servicoTelemetria;
    private readonly IAsyncPolicy policy;
    private readonly ILogger<ServicoLog> logger;

    public ServicoMensageria(RabbitOptions rabbitOptions,
        IServicoTelemetria servicoTelemetria,
        IReadOnlyPolicyRegistry<string> registry,
        ILogger<ServicoLog> logger)
    {
        this.rabbitOptions = rabbitOptions ?? throw new ArgumentNullException(nameof(rabbitOptions));
        this.servicoTelemetria = servicoTelemetria ?? throw new ArgumentNullException(nameof(servicoTelemetria));
        policy = registry.Get<IAsyncPolicy>(PoliticaPolly.PublicaFila);
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Publicar(MensagemRabbit mensagemRabbit, string rota, string exchange, string nomeAcao)
    {
        var body = Encoding.UTF8.GetBytes(mensagemRabbit.ConverterObjectParaJson());

        await servicoTelemetria.RegistrarAsync(
            async () => await policy.ExecuteAsync(async () =>
                await PublicarMensagem(rota, body, exchange)), nomeAcao, rota, string.Empty);

        return true;
    }

    private async Task PublicarMensagem(string rota, byte[] body, string? exchange = null)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitOptions?.HostName ?? string.Empty,
                UserName = rabbitOptions?.UserName ?? string.Empty,
                Password = rabbitOptions?.Password ?? string.Empty,
                VirtualHost = rabbitOptions?.VirtualHost ?? string.Empty
            };

            using var conexaoRabbit = await factory.CreateConnectionAsync();
            using var channel = await conexaoRabbit.CreateChannelAsync();
            var props = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange ?? string.Empty,
                rota,
                true,
                props,
                body
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao publicar mensagem no RabbitMQ");
        }
    }
}