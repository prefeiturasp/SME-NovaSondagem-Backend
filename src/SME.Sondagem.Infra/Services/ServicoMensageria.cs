using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SME.Sondagem.Infra.EnvironmentVariables;
using SME.Sondagem.Infra.Extensions;
using SME.Sondagem.Infra.Fila;
using SME.Sondagem.Infra.Interfaces;
using System.Text;

namespace SME.Sondagem.Infra.Services;

public class ServicoMensageria : IServicoMensageria
{
    private readonly RabbitOptions rabbitOptions;
    private readonly ILogger<ServicoMensageria> logger;

    public ServicoMensageria(RabbitOptions rabbitOptions,
        ILogger<ServicoMensageria> logger)
    {
        this.rabbitOptions = rabbitOptions ?? throw new ArgumentNullException(nameof(rabbitOptions));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Publicar(MensagemRabbit mensagemRabbit, string rota, string exchange)
    {
        var body = Encoding.UTF8.GetBytes(mensagemRabbit.ConverterObjectParaJson());
        var teste = mensagemRabbit.ConverterObjectParaJson();

        await PublicarMensagem(rota, body, exchange);

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
