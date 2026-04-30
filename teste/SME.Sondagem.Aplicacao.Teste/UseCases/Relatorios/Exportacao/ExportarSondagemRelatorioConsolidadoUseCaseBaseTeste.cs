using Moq;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio.Exportacao;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Fila;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio.Integracao;
using SME.Sondagem.Infrastructure.Interfaces;
using Xunit;

namespace SME.Sondagem.Testes.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;

public abstract class ExportarSondagemRelatorioConsolidadoUseCaseBaseTeste<TUseCase>
    where TUseCase : IExportarSondagemRelatorioConsolidadoUseCase
{
    protected readonly Mock<ISolicitacaoRelatorioService> MockSolicitacaoRelatorioService;
    protected readonly Mock<IServicoLog> MockServicoLog;
    protected readonly Mock<IServicoMensageria> MockServicoMensageria;
    protected readonly Mock<IServicoUsuario> MockServicoUsuario;
    protected readonly TUseCase UseCase;

    protected ExportarSondagemRelatorioConsolidadoUseCaseBaseTeste()
    {
        MockSolicitacaoRelatorioService = new Mock<ISolicitacaoRelatorioService>();
        MockServicoLog = new Mock<IServicoLog>();
        MockServicoMensageria = new Mock<IServicoMensageria>();
        MockServicoUsuario = new Mock<IServicoUsuario>();

        UseCase = CriarUseCase();
    }

    protected abstract TUseCase CriarUseCase();

    protected abstract TipoRelatorio TipoRelatorioEsperado { get; }

    protected abstract string RotaRabbitEsperada { get; }

    [Fact]
    public async Task Exportar_RelatorioJaExistente_NaoDevePublicarMensagem()
    {
        var filtro = new FiltroRelatorioConsolidado { ExtensaoRelatorio = FormatoRelatorio.Pdf };
        var ct = CancellationToken.None;

        MockSolicitacaoRelatorioService
            .Setup(s => s.ObterSolicitacaoRelatorioAsync(It.IsAny<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(), ct))
            .ReturnsAsync(12345);

        await UseCase.Exportar(filtro, ct);

        MockServicoMensageria.Verify(m => m.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        MockSolicitacaoRelatorioService.Verify(s => s.RegistrarSolicitacaoRelatorioAsync(It.IsAny<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(), ct), Times.Never);
    }

    [Fact]
    public async Task Exportar_NovoRelatorio_DeveRegistrarEPublicarMensagem()
    {
        var filtro = new FiltroRelatorioConsolidado { ExtensaoRelatorio = FormatoRelatorio.Pdf };
        var ct = CancellationToken.None;
        const string rfUsuario = "1234567";
        const long novoId = 999;

        MockServicoUsuario.Setup(u => u.ObterRFUsuarioLogado()).Returns(rfUsuario);

        MockSolicitacaoRelatorioService
            .Setup(s => s.ObterSolicitacaoRelatorioAsync(It.IsAny<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(), ct))
            .ReturnsAsync(0);

        MockSolicitacaoRelatorioService
            .Setup(s => s.RegistrarSolicitacaoRelatorioAsync(It.IsAny<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(), ct))
            .ReturnsAsync(novoId);

        await UseCase.Exportar(filtro, ct);

        MockSolicitacaoRelatorioService.Verify(
            s => s.RegistrarSolicitacaoRelatorioAsync(
                It.Is<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(f => f.TipoRelatorio == TipoRelatorioEsperado),
                ct),
            Times.Once);

        MockServicoMensageria.Verify(m => m.Publicar(
            It.Is<MensagemRabbit>(msg =>
                msg.UsuarioLogadoRF == rfUsuario &&
                ((FiltroSolicitacaoRelatorioIntegracaoRabbitDto)msg.Mensagem).SolicitacaoRelatorioId == novoId &&
                ((FiltroSolicitacaoRelatorioIntegracaoRabbitDto)msg.Mensagem).TipoRelatorio == TipoRelatorioEsperado),
            RotaRabbitEsperada,
            ExchangeRabbit.Sgp),
        Times.Once);
    }

    [Fact]
    public async Task Exportar_ErroNoServico_DeveLogarEPublicarComIdZero()
    {
        var filtro = new FiltroRelatorioConsolidado { ExtensaoRelatorio = FormatoRelatorio.Xlsx };
        var ct = CancellationToken.None;

        MockSolicitacaoRelatorioService
            .Setup(s => s.ObterSolicitacaoRelatorioAsync(It.IsAny<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(), ct))
            .ThrowsAsync(new Exception("Erro de Banco"));

        await UseCase.Exportar(filtro, ct);

        MockServicoLog.Verify(
            l => l.Registrar(It.Is<string>(s => s.Contains("Falha ao controlar duplicidade")), It.IsAny<Exception>()),
            Times.Once);

        MockServicoMensageria.Verify(m => m.Publicar(
            It.Is<MensagemRabbit>(msg => ((FiltroSolicitacaoRelatorioIntegracaoRabbitDto)msg.Mensagem).SolicitacaoRelatorioId == 0),
            RotaRabbitEsperada,
            ExchangeRabbit.Sgp),
        Times.Once);
    }

    [Fact]
    public async Task Exportar_DevePublicarMensagemNaRotaRabbitEsperada()
    {
        var filtro = new FiltroRelatorioConsolidado { ExtensaoRelatorio = FormatoRelatorio.Pdf };
        var ct = CancellationToken.None;

        MockSolicitacaoRelatorioService
            .Setup(s => s.ObterSolicitacaoRelatorioAsync(It.IsAny<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(), ct))
            .ReturnsAsync(0);

        MockSolicitacaoRelatorioService
            .Setup(s => s.RegistrarSolicitacaoRelatorioAsync(It.IsAny<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(), ct))
            .ReturnsAsync(1);

        await UseCase.Exportar(filtro, ct);

        MockServicoMensageria.Verify(
            m => m.Publicar(It.IsAny<MensagemRabbit>(), RotaRabbitEsperada, ExchangeRabbit.Sgp),
            Times.Once);
    }
}
