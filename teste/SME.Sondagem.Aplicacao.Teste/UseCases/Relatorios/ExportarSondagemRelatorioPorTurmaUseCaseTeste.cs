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
    public async Task ExportarSondagemRelatorio_RegistrarRetornaIdJaExistente_DeveRegistrarEPublicarComEsseId()
    {
        // Arrange
        var filtro = new FiltroRelatorio { ExtensaoRelatorio = FormatoRelatorio.Pdf };
        var ct = CancellationToken.None;
        const long idExistente = 12345;
        var rfUsuario = "1234567";

        _mockServicoUsuario.Setup(u => u.ObterRFUsuarioLogado()).Returns(rfUsuario);

        _mockSolicitacaoRelatorioService
            .Setup(s => s.RegistrarSolicitacaoRelatorioAsync(It.IsAny<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(), ct))
            .ReturnsAsync(idExistente);

        // Act
        await _useCase.ExportarSondagemRelatorio(filtro, ct);

        // Assert
        _mockSolicitacaoRelatorioService.Verify(
            s => s.RegistrarSolicitacaoRelatorioAsync(It.IsAny<FiltroSolicitacaoRelatorioIntegracaoSgpDto>(), ct),
            Times.Once);

        _mockServicoMensageria.Verify(m => m.Publicar(
            It.Is<MensagemRabbit>(msg =>
                msg.UsuarioLogadoRF == rfUsuario &&
                ((FiltroSolicitacaoRelatorioIntegracaoRabbitDto)msg.Mensagem).SolicitacaoRelatorioId == idExistente),
            It.IsAny<string>(),
            It.IsAny<string>()),
        Times.Once);
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
}