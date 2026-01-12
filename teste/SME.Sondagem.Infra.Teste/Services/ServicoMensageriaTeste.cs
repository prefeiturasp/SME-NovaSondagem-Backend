using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using Polly.Registry;
using SME.Sondagem.Infra.EnvironmentVariables;
using SME.Sondagem.Infra.Fila;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infra.Policies;
using SME.Sondagem.Infra.Services;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Services
{
    public class ServicoMensageriaTeste
    {
        private readonly Mock<IServicoTelemetria> telemetriaMock;
        private readonly Mock<IAsyncPolicy> policyMock;
        private readonly Mock<IReadOnlyPolicyRegistry<string>> registryMock;
        private readonly Mock<ILogger<ServicoMensageria>> loggerMock;

        private readonly RabbitOptions rabbitOptions;
        private readonly ServicoMensageria servico;

        public ServicoMensageriaTeste()
        {
            telemetriaMock = new Mock<IServicoTelemetria>();
            policyMock = new Mock<IAsyncPolicy>();
            registryMock = new Mock<IReadOnlyPolicyRegistry<string>>();
            loggerMock = new Mock<ILogger<ServicoMensageria>>();

            rabbitOptions = new RabbitOptions
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };

            registryMock
                .Setup(r => r.Get<IAsyncPolicy>(PoliticaPolly.PublicaFila))
                .Returns(policyMock.Object);

            policyMock
                .Setup(p => p.ExecuteAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(async action =>
                {
                    await action();
                });

            servico = new ServicoMensageria(
                rabbitOptions,
                telemetriaMock.Object,
                registryMock.Object,
                loggerMock.Object
            );
        }

        [Fact]
        public async Task Publicar_deve_retornar_true()
        {
            var mensagem = new MensagemRabbit("teste", Guid.NewGuid());

            var resultado = await servico.Publicar(
                mensagem,
                "rota.teste",
                "exchange.teste",
                "Publicar Mensagem");

            Assert.True(resultado);
        }

        [Fact]
        public async Task Publicar_deve_chamar_telemetria()
        {
            var mensagem = new MensagemRabbit("teste", Guid.NewGuid());

            await servico.Publicar(
                mensagem,
                "rota",
                "exchange",
                "acao");

            telemetriaMock.Verify(t =>
                t.RegistrarAsync(
                    It.IsAny<Func<Task>>(),
                    "acao",
                    "rota",
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task Publicar_deve_executar_delegate_passado_para_telemetria()
        {
            Func<Task>? delegateExecutado = null;

            telemetriaMock
                .Setup(t => t.RegistrarAsync(
                    It.IsAny<Func<Task>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Callback<Func<Task>, string, string, string>((func, _, _, _) =>
                {
                    delegateExecutado = func;
                });

            var mensagem = new MensagemRabbit("teste", Guid.NewGuid());

            await servico.Publicar(
                mensagem,
                "rota",
                "exchange",
                "acao");

            Assert.NotNull(delegateExecutado);

            var ex = await Record.ExceptionAsync(() => delegateExecutado!());
            Assert.Null(ex);
        }


        [Fact]
        public async Task Publicar_nao_deve_lancar_excecao_quando_publicacao_falhar()
        {
            policyMock
                .Setup(p => p.ExecuteAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(action =>
                {
                    throw new Exception("Falha simulada");
                });

            var mensagem = new MensagemRabbit("teste", Guid.NewGuid());

            var ex = await Record.ExceptionAsync(() =>
                servico.Publicar(
                    mensagem,
                    "rota",
                    "exchange",
                    "acao"));

            Assert.Null(ex);
        }
    }
}
