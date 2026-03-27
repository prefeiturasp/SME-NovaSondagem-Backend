using Microsoft.EntityFrameworkCore;
using Moq;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Infra.Contexto;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Postgres;

public class RepositorioParametroSondagemQuestionarioTeste : IDisposable
{
    private readonly SondagemDbContext _context;
    private readonly Mock<IServicoAuditoria> _servicoAuditoriaMock;
    private readonly Mock<ContextoBase> _contextoBaseMock;
    private readonly RepositorioParametroSondagemQuestionario _repositorio;

    public RepositorioParametroSondagemQuestionarioTeste()
    {
        var options = new DbContextOptionsBuilder<SondagemDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new SondagemDbContext(options);
        _servicoAuditoriaMock = new Mock<IServicoAuditoria>();
        _contextoBaseMock = new Mock<ContextoBase>();
        
        _repositorio = new RepositorioParametroSondagemQuestionario(
            _context,
            _servicoAuditoriaMock.Object,
            _contextoBaseMock.Object);
    }

    [Fact]
    public void Deve_instanciar_repositorio_com_parametros_validos()
    {
        // Arrange & Act
        var repositorio = new RepositorioParametroSondagemQuestionario(
            _context,
            _servicoAuditoriaMock.Object,
            _contextoBaseMock.Object);

        // Assert
        Assert.NotNull(repositorio);
    }

    [Fact]
    public async Task ObterPorIdQuestionarioAsync_Deve_retornar_parametros_quando_existirem()
    {
        // Arrange
        const int idQuestionario = 1;
        
        var parametroSondagem = new ParametroSondagem
        {
            Id = 1,
            Nome = "Parâmetro Teste",
            Tipo = Dominio.Enums.TipoParametroSondagem.ExibirTituloTabelaSondagem,
            Ativo = true,
            Excluido = false
        };

        var parametro1 = new ParametroSondagemQuestionario
        {
            Id = 1,
            IdQuestionario = idQuestionario,
            IdParametroSondagem = 1,
            Valor = "Valor 1",
            Excluido = false,
            ParametroSondagem = parametroSondagem
        };

        var parametro2 = new ParametroSondagemQuestionario
        {
            Id = 2,
            IdQuestionario = idQuestionario,
            IdParametroSondagem = 1,
            Valor = "Valor 2",
            Excluido = false,
            ParametroSondagem = parametroSondagem
        };

        await _context.ParametrosSondagem.AddAsync(parametroSondagem);
        await _context.ParametrosSondagemQuestionario.AddRangeAsync(parametro1, parametro2);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repositorio.ObterPorIdQuestionarioAsync(idQuestionario);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
        
        var parametros = resultado.ToList();
        Assert.Contains(parametros, p => p.Id == 1 && p.Valor == "Valor 1");
        Assert.Contains(parametros, p => p.Id == 2 && p.Valor == "Valor 2");
        Assert.All(parametros, p => 
        {
            Assert.Equal(idQuestionario, p.IdQuestionario);
            Assert.NotNull(p.ParametroSondagem);
        });
    }

    [Fact]
    public async Task ObterPorIdQuestionarioAsync_Deve_retornar_lista_vazia_quando_nao_existirem_parametros()
    {
        // Arrange
        const long idQuestionarioInexistente = 999;

        // Act
        var resultado = await _repositorio.ObterPorIdQuestionarioAsync(idQuestionarioInexistente);

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ObterPorIdQuestionarioAsync_Deve_filtrar_parametros_excluidos()
    {
        // Arrange
        const int idQuestionario = 2;
        
        var parametroSondagem = new ParametroSondagem
        {
            Id = 2,
            Nome = "Parâmetro Descrição",
            Tipo = Dominio.Enums.TipoParametroSondagem.ExibirDescricaoOpcaoResposta,
            Ativo = true,
            Excluido = false
        };

        var parametroAtivo = new ParametroSondagemQuestionario
        {
            Id = 3,
            IdQuestionario = idQuestionario,
            IdParametroSondagem = 2,
            Valor = "Valor Ativo",
            Excluido = false,
            ParametroSondagem = parametroSondagem
        };

        var parametroExcluido = new ParametroSondagemQuestionario
        {
            Id = 4,
            IdQuestionario = idQuestionario,
            IdParametroSondagem = 2,
            Valor = "Valor Excluído",
            Excluido = true,
            ParametroSondagem = parametroSondagem
        };

        await _context.ParametrosSondagem.AddAsync(parametroSondagem);
        await _context.ParametrosSondagemQuestionario.AddRangeAsync(parametroAtivo, parametroExcluido);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repositorio.ObterPorIdQuestionarioAsync(idQuestionario);

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado);
        
        var parametro = resultado.First();
        Assert.Equal(3, parametro.Id);
        Assert.Equal("Valor Ativo", parametro.Valor);
        Assert.False(parametro.Excluido);
    }

    [Fact]
    public async Task ObterPorIdQuestionarioAsync_Deve_filtrar_parametros_com_sondagem_excluida()
    {
        // Arrange
        const int idQuestionario = 3;
        
        var parametroSondagemAtivo = new ParametroSondagem
        {
            Id = 3,
            Nome = "Parâmetro Título",
            Tipo = Dominio.Enums.TipoParametroSondagem.ExibirTituloTabelaSondagem,
            Ativo = true,
            Excluido = false
        };

        var parametroSondagemExcluido = new ParametroSondagem
        {
            Id = 4,
            Nome = "Parâmetro Excluído",
            Tipo = Dominio.Enums.TipoParametroSondagem.ExibirDescricaoOpcaoResposta,
            Ativo = true,
            Excluido = true
        };

        var parametroComSondagemAtiva = new ParametroSondagemQuestionario
        {
            Id = 5,
            IdQuestionario = idQuestionario,
            IdParametroSondagem = 3,
            Valor = "Sondagem Ativa",
            Excluido = false,
            ParametroSondagem = parametroSondagemAtivo
        };

        var parametroComSondagemExcluida = new ParametroSondagemQuestionario
        {
            Id = 6,
            IdQuestionario = idQuestionario,
            IdParametroSondagem = 4,
            Valor = "Sondagem Excluída",
            Excluido = false,
            ParametroSondagem = parametroSondagemExcluido
        };

        await _context.ParametrosSondagem.AddRangeAsync(parametroSondagemAtivo, parametroSondagemExcluido);
        await _context.ParametrosSondagemQuestionario.AddRangeAsync(parametroComSondagemAtiva, parametroComSondagemExcluida);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repositorio.ObterPorIdQuestionarioAsync(idQuestionario);

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado);
        
        var parametro = resultado.First();
        Assert.Equal(5, parametro.Id);
        Assert.Equal("Sondagem Ativa", parametro.Valor);
        Assert.NotNull(parametro.ParametroSondagem);
        Assert.False(parametro.ParametroSondagem.Excluido);
    }

    [Fact]
    public async Task ObterPorIdQuestionarioAsync_Deve_incluir_propriedade_navigation_ParametroSondagem()
    {
        // Arrange
        const int idQuestionario = 4;
        
        var parametroSondagem = new ParametroSondagem
        {
            Id = 5,
            Nome = "Parâmetro Navigation",
            Tipo = Dominio.Enums.TipoParametroSondagem.ExibirTituloTabelaSondagem,
            Ativo = true,
            Excluido = false
        };

        var parametro = new ParametroSondagemQuestionario
        {
            Id = 7,
            IdQuestionario = idQuestionario,
            IdParametroSondagem = 5,
            Valor = "Teste Navigation",
            Excluido = false,
            ParametroSondagem = parametroSondagem
        };

        await _context.ParametrosSondagem.AddAsync(parametroSondagem);
        await _context.ParametrosSondagemQuestionario.AddAsync(parametro);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repositorio.ObterPorIdQuestionarioAsync(idQuestionario);

        // Assert
        Assert.NotNull(resultado);
        Assert.Single(resultado);
        
        var parametroResultado = resultado.First();
        Assert.NotNull(parametroResultado.ParametroSondagem);
        Assert.Equal(Dominio.Enums.TipoParametroSondagem.ExibirTituloTabelaSondagem, parametroResultado.ParametroSondagem.Tipo);
    }

    [Fact]
    public async Task ObterPorIdQuestionarioAsync_Deve_respeitar_cancellation_token()
    {
        // Arrange
        const long idQuestionario = 5;
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repositorio.ObterPorIdQuestionarioAsync(idQuestionario, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task ObterPorIdQuestionarioAsync_Deve_retornar_multiplos_parametros_com_diferentes_tipos()
    {
        // Arrange
        const int idQuestionario = 6;
        
        var parametroSondagem1 = new ParametroSondagem
        {
            Id = 6,
            Nome = "Parâmetro Título Tabela",
            Tipo = Dominio.Enums.TipoParametroSondagem.ExibirTituloTabelaSondagem,
            Ativo = true,
            Excluido = false
        };

        var parametroSondagem2 = new ParametroSondagem
        {
            Id = 7,
            Nome = "Parâmetro Descrição Opção",
            Tipo = Dominio.Enums.TipoParametroSondagem.ExibirDescricaoOpcaoResposta,
            Ativo = true,
            Excluido = false
        };

        var parametro1 = new ParametroSondagemQuestionario
        {
            Id = 8,
            IdQuestionario = idQuestionario,
            IdParametroSondagem = 6,
            Valor = "Título",
            Excluido = false,
            ParametroSondagem = parametroSondagem1
        };

        var parametro2 = new ParametroSondagemQuestionario
        {
            Id = 9,
            IdQuestionario = idQuestionario,
            IdParametroSondagem = 7,
            Valor = "Descrição",
            Excluido = false,
            ParametroSondagem = parametroSondagem2
        };

        await _context.ParametrosSondagem.AddRangeAsync(parametroSondagem1, parametroSondagem2);
        await _context.ParametrosSondagemQuestionario.AddRangeAsync(parametro1, parametro2);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repositorio.ObterPorIdQuestionarioAsync(idQuestionario);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
        
        var parametros = resultado.ToList();
        Assert.Contains(parametros, p => p.Valor == "Título" && 
                                        p.ParametroSondagem.Tipo == Dominio.Enums.TipoParametroSondagem.ExibirTituloTabelaSondagem);
        Assert.Contains(parametros, p => p.Valor == "Descrição" && 
                                        p.ParametroSondagem.Tipo == Dominio.Enums.TipoParametroSondagem.ExibirDescricaoOpcaoResposta);
    }

    public void Dispose()
    {
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }
}