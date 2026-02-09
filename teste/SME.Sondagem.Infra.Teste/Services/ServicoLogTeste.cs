using Microsoft.Extensions.Logging;
using Moq;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.EnvironmentVariables;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infra.Services;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Services
{
    public class ServicoLogTeste
    {
        private readonly Mock<IServicoTelemetria> telemetriaMock;
        private readonly Mock<ILogger<ServicoLog>> loggerMock;
        private readonly RabbitLogOptions rabbitOptions;

        private readonly ServicoLog servico;
        public ServicoLogTeste()
        {
            telemetriaMock = new Mock<IServicoTelemetria>();
            loggerMock = new Mock<ILogger<ServicoLog>>();

            rabbitOptions = new RabbitLogOptions
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };

            servico = new ServicoLog(
                telemetriaMock.Object,
                rabbitOptions,
                loggerMock.Object
            );
        }

        [Fact]
        public void Registrar_exception_deve_chamar_telemetria()
        {
            Exception ex = new InvalidOperationException("erro");

            servico.Registrar(ex);

            telemetriaMock.Verify(t =>
                t.Registrar(
                    It.IsAny<Action>(),
                    "RabbitMQ",
                    "Salvar Log Via Rabbit",
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public void Registrar_com_nivel_deve_chamar_telemetria()
        {
            servico.Registrar(
                LogNivel.Critico,
                "Erro qualquer",
                "Observação",
                "StackTrace");

            telemetriaMock.Verify(t =>
                t.Registrar(
                    It.IsAny<Action>(),
                    "RabbitMQ",
                    "Salvar Log Via Rabbit",
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public void Registrar_com_mensagem_e_exception_deve_chamar_telemetria()
        {
            var ex = new Exception("falha");

            servico.Registrar("Mensagem customizada", ex);

            telemetriaMock.Verify(t =>
                t.Registrar(
                    It.IsAny<Action>(),
                    "RabbitMQ",
                    "Salvar Log Via Rabbit",
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public void Registrar_deve_executar_delegate_sem_lancar_excecao()
        {
            Action? funcExecutada = null;

            telemetriaMock
                .Setup(t => t.Registrar(
                    It.IsAny<Action>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Callback<Action, string, string, string>((f, _, _, _) =>
                {
                    funcExecutada = f;
                });

            servico.Registrar(new Exception("erro"));

            Assert.NotNull(funcExecutada);
        }
    }
}
