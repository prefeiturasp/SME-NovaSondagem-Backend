using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Fila;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio.Integracao;
using SME.Sondagem.Infrastructure.Interfaces;
using Xunit;

namespace SME.Sondagem.Testes.Aplicacao.UseCases.Questionario.Relatorio.Exportacao;

public class ExportarSondagemRelatorioPorTurmaUseCaseTeste
{
    private readonly Mock<ISolicitacaoRelatorioService> _mockSolicitacaoRelatorioService;
    private readonly Mock<IServicoLog> _mockServicoLog;
    private readonly Mock<IServicoMensageria> _mockServicoMensageria;
    private readonly Mock<IServicoUsuario> _mockServicoUsuario;
    private readonly ExportarSondagemRelatorioPorTurmaUseCase _useCase;

    public ExportarSondagemRelatorioPorTurmaUseCaseTeste()
    {
        _mockSolicitacaoRelatorioService = new Mock<ISolicitacaoRelatorioService>();
        _mockServicoLog = new Mock<IServicoLog>();
        _mockServicoMensageria = new Mock<IServicoMensageria>();
        _mockServicoUsuario = new Mock<IServicoUsuario>();

        _useCase = new ExportarSondagemRelatorioPorTurmaUseCase(
            _mockSolicitacaoRelatorioService.Object,
            _mockServicoLog.Object,
            _mockServicoMensageria.Object,
            _mockServicoUsuario.Object);
    }

    [Fact]
    public async Task ExportarSondagemRelatorio_RelatorioJaExistente_NaoDevePublicarMensagem()
    {
        // Arrange
        var filtro = new FiltroRelatorio { ExtensaoRelatorio = FormatoRelatorio.Pdf };
        var ct = CancellationToken.None;

        _mockSolicitacaoRelatorioService
            .Setup(s => s.ObterSolicitacaoRelatorioAsync(It.IsAny<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(), ct))
            .ReturnsAsync(12345); // ID diferente de 0 indica que já existe

        // Act
        await _useCase.ExportarSondagemRelatorio(filtro, ct);

        // Assert
        _mockServicoMensageria.Verify(m => m.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockSolicitacaoRelatorioService.Verify(s => s.RegistrarSolicitacaoRelatorioAsync(It.IsAny<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(), ct), Times.Never);
    }

    [Fact]
    public async Task ExportarSondagemRelatorio_NovoRelatorio_DeveRegistrarEPublicarMensagem()
    {
        // Arrange
        var filtro = new FiltroRelatorio { ExtensaoRelatorio = FormatoRelatorio.Pdf };
        var ct = CancellationToken.None;
        var rfUsuario = "1234567";
        var novoId = 999;

        _mockServicoUsuario.Setup(u => u.ObterRFUsuarioLogado()).Returns(rfUsuario);

        _mockSolicitacaoRelatorioService
            .Setup(s => s.ObterSolicitacaoRelatorioAsync(It.IsAny<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(), ct))
            .ReturnsAsync(0); // Não encontrado

        _mockSolicitacaoRelatorioService
            .Setup(s => s.RegistrarSolicitacaoRelatorioAsync(It.IsAny<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(), ct))
            .ReturnsAsync(novoId);

        // Act
        await _useCase.ExportarSondagemRelatorio(filtro, ct);

        // Assert
        _mockSolicitacaoRelatorioService.Verify(s => s.RegistrarSolicitacaoRelatorioAsync(It.IsAny<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(), ct), Times.Once);

        _mockServicoMensageria.Verify(m => m.Publicar(
            It.Is<MensagemRabbit>(msg =>
                msg.UsuarioLogadoRF == rfUsuario &&
                ((FiltroSolicitacaoRelatorioIntegracaoRabbitDto)msg.Mensagem).SolicitacaoRelatorioId == novoId),
            It.IsAny<string>(),
            It.IsAny<string>()),
        Times.Once);
    }

    [Fact]
    public async Task ExportarSondagemRelatorio_ErroNoServico_DeveLogarEPublicarComIdZero()
    {
        // Arrange
        var filtro = new FiltroRelatorio { ExtensaoRelatorio = FormatoRelatorio.Xlsx };
        var ct = CancellationToken.None;

        _mockSolicitacaoRelatorioService
            .Setup(s => s.ObterSolicitacaoRelatorioAsync(It.IsAny<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(), ct))
            .ThrowsAsync(new System.Exception("Erro de Banco"));

        // Act
        await _useCase.ExportarSondagemRelatorio(filtro, ct);

        // Assert
        _mockServicoLog.Verify(l => l.Registrar(It.Is<string>(s => s.Contains("Falha ao controlar duplicidade")), It.IsAny<Exception>()), Times.Once);

        // Como o catch retorna (false, 0), ele deve tentar publicar a mensagem mesmo assim
        _mockServicoMensageria.Verify(m => m.Publicar(
            It.Is<MensagemRabbit>(msg => ((FiltroSolicitacaoRelatorioIntegracaoRabbitDto)msg.Mensagem).SolicitacaoRelatorioId == 0),
            It.IsAny<string>(),
            It.IsAny<string>()),
        Times.Once);
    }
}