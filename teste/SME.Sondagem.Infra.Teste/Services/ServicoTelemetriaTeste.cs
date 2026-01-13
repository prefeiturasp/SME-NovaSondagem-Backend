using FluentAssertions;
using SME.Sondagem.Infra.EnvironmentVariables;
using SME.Sondagem.Infra.Services;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Services
{
    public class ServicoTelemetriaTeste
    {
        #region Helpers

        private static TelemetriaOptions CriarOptions(
            bool apm = false,
            bool applicationInsights = false)
        {
            return new TelemetriaOptions
            {
                Apm = apm,
                ApplicationInsights = applicationInsights
            };
        }

        #endregion

        #region IniciarTransacao

        [Fact]
        public void IniciarTransacao_deve_criar_transacao_com_nome_e_sucesso_true()
        {
            var options = CriarOptions();
            var servico = new ServicoTelemetria(options);

            var transacao = servico.IniciarTransacao("rota-teste");

            transacao.Should().NotBeNull();
            transacao.Nome.Should().Be("rota-teste");
            transacao.Sucesso.Should().BeTrue();
        }

        #endregion

        #region FinalizarTransacao

        [Fact]
        public void FinalizarTransacao_nao_deve_lancar_excecao_quando_apm_desativado()
        {
            var options = CriarOptions(apm: false);
            var servico = new ServicoTelemetria(options);
            var transacao = servico.IniciarTransacao("rota");

            servico.Invoking(s => s.FinalizarTransacao(transacao))
                .Should().NotThrow();
        }

        #endregion

        #region RegistrarAsync

        [Fact]
        public async Task RegistrarAsync_deve_executar_acao_sem_apm()
        {
            var options = CriarOptions(apm: false);
            var servico = new ServicoTelemetria(options);
            var executou = false;

            await servico.RegistrarAsync(() =>
            {
                executou = true;
                return Task.CompletedTask;
            }, "acao", "telemetria", "valor");

            executou.Should().BeTrue();
        }

        [Fact]
        public async Task RegistrarAsync_deve_executar_acao_com_apm_ativo()
        {
            var options = CriarOptions(apm: true);
            var servico = new ServicoTelemetria(options);
            var transacao = servico.IniciarTransacao("rota-teste");
            var executou = false;

            await servico.RegistrarAsync(() =>
            {
                executou = true;
                return Task.CompletedTask;
            }, "acao", "telemetria", "valor");

            servico.FinalizarTransacao(transacao);

            executou.Should().BeTrue();
        }

        [Fact]
        public async Task RegistrarAsync_com_application_insights_ativo_nao_deve_lancar_excecao()
        {
            var options = CriarOptions(applicationInsights: true);
            var servico = new ServicoTelemetria(options);

            var act = async () => await servico.RegistrarAsync(
                () => Task.CompletedTask,
                "acao",
                "telemetria",
                "valor");

            await act.Should().NotThrowAsync();
        }

        #endregion

        #region RegistrarComRetornoAsync

        [Fact]
        public async Task RegistrarComRetornoAsync_deve_retornar_resultado()
        {
            var options = CriarOptions();
            var servico = new ServicoTelemetria(options);

            dynamic resultado = await servico.RegistrarComRetornoAsync<object>(
                () => Task.FromResult<object>("ok"),
                "acao",
                "telemetria",
                "valor");

            string resultadoString = resultado;
            resultadoString.Should().Be("ok");
        }

        [Fact]
        public async Task RegistrarComRetornoAsync_com_apm_ativo_deve_retornar_resultado()
        {
            var options = CriarOptions(apm: true);
            var servico = new ServicoTelemetria(options);
            var transacao = servico.IniciarTransacao("rota-teste");

            dynamic resultado = await servico.RegistrarComRetornoAsync<object>(
                () => Task.FromResult<object>(123),
                "acao",
                "telemetria",
                "valor");

            servico.FinalizarTransacao(transacao);

            int resultadoInt = resultado;
            resultadoInt.Should().Be(123);
        }

        #endregion

        #region Registrar (sync)

        [Fact]
        public void Registrar_deve_executar_acao()
        {
            var options = CriarOptions();
            var servico = new ServicoTelemetria(options);
            var executou = false;

            servico.Registrar(() =>
            {
                executou = true;
            }, "acao", "telemetria", "valor");

            executou.Should().BeTrue();
        }

        [Fact]
        public void Registrar_com_apm_ativo_nao_deve_lancar_excecao()
        {
            var options = CriarOptions(apm: true);
            var servico = new ServicoTelemetria(options);
            var transacao = servico.IniciarTransacao("rota-teste");

            servico.Invoking(s => s.Registrar(
                () => { },
                "acao",
                "telemetria",
                "valor"))
                .Should().NotThrow();

            servico.FinalizarTransacao(transacao);
        }

        #endregion

        #region RegistrarComRetorno (sync)

        [Fact]
        public void RegistrarComRetorno_deve_retornar_valor()
        {
            var options = CriarOptions();
            var servico = new ServicoTelemetria(options);

            dynamic resultado = servico.RegistrarComRetorno<object>(
                () => "resultado",
                "acao",
                "telemetria",
                "valor");

            string resultadoString = resultado;
            resultadoString.Should().Be("resultado");
        }

        #endregion
    }
}
