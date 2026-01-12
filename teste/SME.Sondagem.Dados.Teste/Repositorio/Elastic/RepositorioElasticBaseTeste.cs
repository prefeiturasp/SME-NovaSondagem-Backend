using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using FluentAssertions;
using Moq;
using SME.Sondagem.Dados.Repositorio.Elastic;
using SME.Sondagem.Infra.Interfaces;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Elastic;

public class RepositorioElasticBaseTeste
{
    private readonly Mock<IServicoTelemetria> servicoTelemetriaMock;
    private readonly Mock<ElasticsearchClient> elasticClientMock;

    public RepositorioElasticBaseTeste()
    {
        servicoTelemetriaMock = new Mock<IServicoTelemetria>();
        elasticClientMock = new Mock<ElasticsearchClient>();
    }

    #region Construtor

    [Fact]
    public void Construtor_ComServicoTelemetriaNulo_DeveLancarArgumentNullException()
    {
        var act = () => new RepositorioElasticBaseFake(null, elasticClientMock.Object);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("servicoTelemetria");
    }

    [Fact]
    public void Construtor_ComElasticClientNulo_DeveLancarArgumentNullException()
    {
        var act = () => new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, null);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("elasticClient");
    }

    [Fact]
    public void Construtor_ComDependenciasValidas_DeveCriarInstancia()
    {
        var act = () => new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);

        act.Should().NotThrow();
    }

    #endregion

    #region ObterAsync

    [Fact]
    public async Task ObterAsync_ComDocumentoEncontrado_DeveRetornarEntidade()
    {
        var entidadeEsperada = new EntidadeTeste { Id = "123", Nome = "Teste" };

        ConfigurarServicoTelemetriaParaExecutarFuncao(servicoTelemetriaMock, entidadeEsperada, true);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var resultado = await repositorio.ObterAsync<EntidadeTeste>("indice-teste", "123", "consulta-teste");

        resultado.Should().NotBeNull();
        resultado.Id.Should().Be("123");
        resultado.Nome.Should().Be("Teste");
    }

    [Fact]
    public async Task ObterAsync_ComDocumentoNaoEncontrado_DeveRetornarNull()
    {
        ConfigurarServicoTelemetriaParaExecutarFuncao<EntidadeTeste>(servicoTelemetriaMock, null, false);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var resultado = await repositorio.ObterAsync<EntidadeTeste>("indice-teste", "999", "consulta-teste");

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterAsync_ComParametro_DeveRegistrarTelemetriaCorretamente()
    {
        var entidade = new EntidadeTeste { Id = "123", Nome = "Teste" };
        var parametro = new { Filtro = "teste" };

        ConfigurarServicoTelemetriaParaExecutarFuncao(servicoTelemetriaMock, entidade, true);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        await repositorio.ObterAsync<EntidadeTeste>("indice-teste", "123", "consulta-teste", parametro);

        servicoTelemetriaMock.Verify(x => x.RegistrarComRetornoAsync<It.IsAnyType>(
            It.IsAny<Func<Task<object>>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()), Times.Once);
    }

    #endregion

    #region ObterListaAsync

    [Fact]
    public async Task ObterListaAsync_ComDocumentosEncontrados_DeveRetornarLista()
    {
        var documentos = new List<EntidadeTeste>
        {
            new() { Id = "1", Nome = "Teste 1" },
            new() { Id = "2", Nome = "Teste 2" }
        };

        ConfigurarServicoTelemetriaParaExecutarFuncaoComLista(servicoTelemetriaMock, documentos);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var resultado = await repositorio.ObterListaAsync<EntidadeTeste>(
            "indice-teste",
            q => q.MatchAll(),
            "consulta-teste");

        resultado.Should().HaveCount(2);
        resultado.Should().Contain(x => x.Id == "1");
        resultado.Should().Contain(x => x.Id == "2");
    }

    [Fact]
    public async Task ObterListaAsync_ComRespostaInvalida_DeveLancarException()
    {
        ConfigurarServicoTelemetriaParaRespostaInvalida<EntidadeTeste>(servicoTelemetriaMock);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var act = async () => await repositorio.ObterListaAsync<EntidadeTeste>(
            "indice-teste",
            q => q.MatchAll(),
            "consulta-teste");

        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task ObterListaAsync_SemDocumentos_DeveRetornarListaVazia()
    {
        ConfigurarServicoTelemetriaParaExecutarFuncaoComLista<EntidadeTeste>(servicoTelemetriaMock, []);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var resultado = await repositorio.ObterListaAsync<EntidadeTeste>(
            "indice-teste",
            q => q.MatchAll(),
            "consulta-teste");

        resultado.Should().BeEmpty();
    }

    #endregion

    #region ObterTodosAsync

    [Fact]
    public async Task ObterTodosAsync_ComDocumentos_DeveRetornarTodos()
    {
        var documentos = new List<EntidadeTeste>
        {
            new() { Id = "1", Nome = "Teste 1" },
            new() { Id = "2", Nome = "Teste 2" },
            new() { Id = "3", Nome = "Teste 3" }
        };

        ConfigurarServicoTelemetriaParaExecutarFuncaoComLista(servicoTelemetriaMock, documentos);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var resultado = await repositorio.ObterTodosAsync<EntidadeTeste>("indice-teste", "consulta-teste");

        resultado.Should().HaveCount(3);
        resultado.Should().Contain(x => x.Id == "1");
        resultado.Should().Contain(x => x.Id == "2");
        resultado.Should().Contain(x => x.Id == "3");
    }

    [Fact]
    public async Task ObterTodosAsync_ComRespostaInvalida_DeveLancarException()
    {
        ConfigurarServicoTelemetriaParaRespostaInvalida<EntidadeTeste>(servicoTelemetriaMock);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var act = async () => await repositorio.ObterTodosAsync<EntidadeTeste>("indice-teste", "consulta-teste");

        await act.Should().ThrowAsync<Exception>();
    }

    #endregion

    #region ObterTotalDeRegistroAsync

    [Fact]
    public async Task ObterTotalDeRegistroAsync_ComDocumentos_DeveRetornarTotal()
    {
        ConfigurarServicoTelemetriaParaExecutarFuncaoComTotal(servicoTelemetriaMock, 150);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var resultado = await repositorio.ObterTotalDeRegistroAsync<EntidadeTeste>("indice-teste", "consulta-teste");

        resultado.Should().Be(150);
    }

    [Fact]
    public async Task ObterTotalDeRegistroAsync_SemDocumentos_DeveRetornarZero()
    {
        ConfigurarServicoTelemetriaParaExecutarFuncaoComTotal(servicoTelemetriaMock, 0);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var resultado = await repositorio.ObterTotalDeRegistroAsync<EntidadeTeste>("indice-teste", "consulta-teste");

        resultado.Should().Be(0);
    }

    #endregion

    #region ObterTotalDeRegistroAPartirDeUmaCondicaoAsync

    [Fact]
    public async Task ObterTotalDeRegistroAPartirDeUmaCondicaoAsync_ComCondicao_DeveRetornarTotal()
    {
        ConfigurarServicoTelemetriaParaExecutarFuncaoComTotal(servicoTelemetriaMock, 42);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var resultado = await repositorio.ObterTotalDeRegistroAPartirDeUmaCondicaoAsync<EntidadeTeste>(
            "indice-teste",
            "consulta-teste",
            q => q.Match(m => m.Field("nome").Query("teste")));

        resultado.Should().Be(42);
    }

    [Fact]
    public async Task ObterTotalDeRegistroAPartirDeUmaCondicaoAsync_ComErro_DeveRetornarZero()
    {
        servicoTelemetriaMock
            .Setup(x => x.RegistrarComRetornoAsync<It.IsAnyType>(
                It.IsAny<Func<Task<object>>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ThrowsAsync(new Exception("Erro de conexão"));

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var resultado = await repositorio.ObterTotalDeRegistroAPartirDeUmaCondicaoAsync<EntidadeTeste>(
            "indice-teste",
            "consulta-teste",
            q => q.Match(m => m.Field("nome").Query("teste")));

        resultado.Should().Be(0);
    }

    #endregion

    #region ExisteAsync

    [Fact]
    public async Task ExisteAsync_ComDocumentoExistente_DeveRetornarTrue()
    {
        ConfigurarServicoTelemetriaParaExecutarFuncao(servicoTelemetriaMock, new EntidadeTeste(), true);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var resultado = await repositorio.ExisteAsync<EntidadeTeste>("indice-teste", "123", "consulta-teste");

        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task ExisteAsync_ComDocumentoInexistente_DeveRetornarFalse()
    {
        ConfigurarServicoTelemetriaParaExecutarFuncao<EntidadeTeste>(servicoTelemetriaMock, null, false);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var resultado = await repositorio.ExisteAsync<EntidadeTeste>("indice-teste", "999", "consulta-teste");

        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task ExisteAsync_ComRespostaInvalida_DeveLancarException()
    {
        ConfigurarServicoTelemetriaParaRespostaInvalidaGet<EntidadeTeste>(servicoTelemetriaMock);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var act = async () => await repositorio.ExisteAsync<EntidadeTeste>("indice-teste", "123", "consulta-teste");

        await act.Should().ThrowAsync<Exception>();
    }

    #endregion

    #region InserirAsync

    [Fact]
    public async Task InserirAsync_ComEntidadeValida_DeveRetornarTrue()
    {
        var entidade = new EntidadeTeste { Id = "123", Nome = "Teste" };

        ConfigurarServicoTelemetriaParaInserir(servicoTelemetriaMock, true);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var resultado = await repositorio.InserirAsync(entidade, "indice-teste");

        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task InserirAsync_ComRespostaInvalida_DeveLancarException()
    {
        var entidade = new EntidadeTeste { Id = "123", Nome = "Teste" };

        ConfigurarServicoTelemetriaParaInserir(servicoTelemetriaMock, false);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var act = async () => await repositorio.InserirAsync(entidade, "indice-teste");

        await act.Should().ThrowAsync<Exception>();
    }

    #endregion

    #region ExcluirTodos

    [Fact]
    public async Task ExcluirTodos_ComIndiceValido_DeveExecutarSemErro()
    {
        ConfigurarServicoTelemetriaParaExcluir(servicoTelemetriaMock);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var act = async () => await repositorio.ExcluirTodos<EntidadeTeste>("indice-teste", "excluir-teste");

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ExcluirTodos_ComErroNaExclusao_DeveLancarException()
    {
        ConfigurarServicoTelemetriaParaExcluir(servicoTelemetriaMock, false);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var act = async () => await repositorio.ExcluirTodos<EntidadeTeste>("indice-teste", "excluir-teste");

        await act.Should().ThrowAsync<Exception>();
    }

    #endregion

    #region InserirBulk

    [Fact]
    public async Task InserirBulk_ComListaDeDocumentos_DeveExecutarSemErro()
    {
        var documentos = new List<EntidadeTeste>
        {
            new() { Id = "1", Nome = "Teste 1" },
            new() { Id = "2", Nome = "Teste 2" }
        };

        ConfigurarServicoTelemetriaParaBulk(servicoTelemetriaMock, true);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var act = async () => await repositorio.InserirBulk(documentos, "indice-teste");

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task InserirBulk_ComErro_DeveLancarException()
    {
        var documentos = new List<EntidadeTeste>
        {
            new() { Id = "1", Nome = "Teste 1" }
        };

        ConfigurarServicoTelemetriaParaBulk(servicoTelemetriaMock, false);

        var repositorio = new RepositorioElasticBaseFake(servicoTelemetriaMock.Object, elasticClientMock.Object);
        var act = async () => await repositorio.InserirBulk(documentos, "indice-teste");

        await act.Should().ThrowAsync<Exception>();
    }

    #endregion

    #region Métodos auxiliares para configuração de mocks

    private static void ConfigurarServicoTelemetriaParaExecutarFuncao<T>(
        Mock<IServicoTelemetria> servicoTelemetriaMock,
        T entidade,
        bool found) where T : class
    {
        servicoTelemetriaMock
            .Setup(x => x.RegistrarComRetornoAsync<It.IsAnyType>(
                It.IsAny<Func<Task<object>>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync((Func<Task<object>> func, string a, string b, string c, string d) =>
            {
                var response = new MockGetResponse<T>
                {
                    Found = found,
                    Source = entidade,
                    IsValidResponse = true
                };
                return response;
            });
    }

    private static void ConfigurarServicoTelemetriaParaRespostaInvalidaGet<T>(
        Mock<IServicoTelemetria> servicoTelemetriaMock) where T : class
    {
        servicoTelemetriaMock
            .Setup(x => x.RegistrarComRetornoAsync<It.IsAnyType>(
                It.IsAny<Func<Task<object>>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync((Func<Task<object>> func, string a, string b, string c, string d) =>
            {
                var response = new MockGetResponse<T>
                {
                    Found = false,
                    Source = null,
                    IsValidResponse = false
                };
                return response;
            });
    }

    private static void ConfigurarServicoTelemetriaParaExecutarFuncaoComLista<T>(
        Mock<IServicoTelemetria> servicoTelemetriaMock,
        List<T> documentos) where T : class
    {
        servicoTelemetriaMock
            .Setup(x => x.RegistrarComRetornoAsync<It.IsAnyType>(
                It.IsAny<Func<Task<object>>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync((Func<Task<object>> func, string a, string b, string c, string d) =>
            {
                var response = new MockSearchResponse<T>
                {
                    Documents = documentos,
                    IsValidResponse = true,
                    Total = documentos.Count,
                    ScrollId = null
                };
                return response;
            });
    }

    private static void ConfigurarServicoTelemetriaParaRespostaInvalida<T>(
        Mock<IServicoTelemetria> servicoTelemetriaMock) where T : class
    {
        servicoTelemetriaMock
            .Setup(x => x.RegistrarComRetornoAsync<It.IsAnyType>(
                It.IsAny<Func<Task<object>>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync((Func<Task<object>> func, string a, string b, string c, string d) =>
            {
                var response = new MockSearchResponse<T>
                {
                    Documents = [],
                    IsValidResponse = false,
                    Total = 0,
                    ScrollId = null
                };
                return response;
            });
    }

    private static void ConfigurarServicoTelemetriaParaExecutarFuncaoComTotal(
        Mock<IServicoTelemetria> servicoTelemetriaMock,
        long total)
    {
        servicoTelemetriaMock
            .Setup(x => x.RegistrarComRetornoAsync<It.IsAnyType>(
                It.IsAny<Func<Task<object>>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync((Func<Task<object>> func, string a, string b, string c, string d) =>
            {
                var response = new MockSearchResponse<EntidadeTeste>
                {
                    Documents = [],
                    IsValidResponse = true,
                    Total = total,
                    ScrollId = null
                };
                return response;
            });
    }

    private static void ConfigurarServicoTelemetriaParaInserir(
        Mock<IServicoTelemetria> servicoTelemetriaMock,
        bool sucesso)
    {
        servicoTelemetriaMock
            .Setup(x => x.RegistrarComRetornoAsync<It.IsAnyType>(
                It.IsAny<Func<Task<object>>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync((Func<Task<object>> func, string a, string b, string c, string d) =>
            {
                var response = new MockIndexResponse
                {
                    IsValidResponse = sucesso
                };
                return response;
            });
    }

    private static void ConfigurarServicoTelemetriaParaExcluir(
        Mock<IServicoTelemetria> servicoTelemetriaMock,
        bool sucesso = true)
    {
        servicoTelemetriaMock
            .Setup(x => x.RegistrarComRetornoAsync<It.IsAnyType>(
                It.IsAny<Func<Task<object>>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync((Func<Task<object>> func, string a, string b, string c, string d) =>
            {
                var response = new MockDeleteByQueryResponse
                {
                    IsValidResponse = sucesso
                };
                return response;
            });
    }

    private static void ConfigurarServicoTelemetriaParaBulk(
        Mock<IServicoTelemetria> servicoTelemetriaMock,
        bool sucesso)
    {
        servicoTelemetriaMock
            .Setup(x => x.RegistrarComRetornoAsync<It.IsAnyType>(
                It.IsAny<Func<Task<object>>>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync((Func<Task<object>> func, string a, string b, string c, string d) =>
            {
                var response = new MockBulkResponse
                {
                    IsValidResponse = sucesso,
                    Errors = !sucesso
                };
                return response;
            });
    }

    #endregion
}

#region Classes Mock para respostas do Elasticsearch

public class MockGetResponse<T> where T : class
{
    public bool Found { get; set; }
    public T Source { get; set; }
    public bool IsValidResponse { get; set; }
}

public class MockSearchResponse<T> where T : class
{
    public List<T> Documents { get; set; } = [];
    public bool IsValidResponse { get; set; }
    public long Total { get; set; }
    public string ScrollId { get; set; }
}

public class MockIndexResponse
{
    public bool IsValidResponse { get; set; }
}

public class MockDeleteByQueryResponse
{
    public bool IsValidResponse { get; set; }
}

public class MockBulkResponse
{
    public bool IsValidResponse { get; set; }
    public bool Errors { get; set; }
}

#endregion

public class RepositorioElasticBaseFake : RepositorioElasticBase<EntidadeTeste>
{
    private readonly IServicoTelemetria servicoTelemetria;

    public RepositorioElasticBaseFake(IServicoTelemetria servicoTelemetria, ElasticsearchClient elasticClient)
        : base(servicoTelemetria, elasticClient)
    {
        this.servicoTelemetria = servicoTelemetria;
    }

    public new async Task<T> ObterAsync<T>(string indice, string id, string nomeConsulta, object parametro = null) where T : class
    {
        var mockResponse = await ObterRespostaMock<MockGetResponse<T>>();

        if (mockResponse != null && mockResponse.Found && mockResponse.Source is T entidade)
            return entidade;

        return null;
    }

    public new async Task<IEnumerable<T>> ObterListaAsync<T>(
        string indice,
        Func<QueryDescriptor<T>, Query> query,
        string nomeConsulta,
        object parametro = null) where T : class
    {
        var mockResponse = await ObterRespostaMock<MockSearchResponse<T>>();

        if (mockResponse == null || !mockResponse.IsValidResponse)
            throw new Exception("Erro simulado no Elasticsearch");

        return mockResponse.Documents ?? [];
    }

    public new async Task<IEnumerable<T>> ObterTodosAsync<T>(
        string indice,
        string nomeConsulta,
        object parametro = null) where T : class
    {
        var mockResponse = await ObterRespostaMock<MockSearchResponse<T>>();

        if (mockResponse == null || !mockResponse.IsValidResponse)
            throw new Exception("Erro simulado no Elasticsearch");

        return mockResponse.Documents ?? [];
    }

    public new async Task<long> ObterTotalDeRegistroAsync<T>(
        string indice,
        string nomeConsulta,
        object parametro = null) where T : class
    {
        var mockResponse = await ObterRespostaMock<MockSearchResponse<T>>();

        if (mockResponse == null || !mockResponse.IsValidResponse)
            throw new Exception("Erro simulado no Elasticsearch");

        return mockResponse.Total;
    }

    public new async Task<long> ObterTotalDeRegistroAPartirDeUmaCondicaoAsync<T>(
        string indice,
        string nomeConsulta,
        Func<QueryDescriptor<T>, Query> query,
        object parametro = null) where T : class
    {
        try
        {
            var mockResponse = await ObterRespostaMock<MockSearchResponse<T>>();

            if (mockResponse == null || !mockResponse.IsValidResponse)
                throw new Exception("Erro simulado no Elasticsearch");

            return mockResponse.Total;
        }
        catch
        {
            return 0;
        }
    }

    public new async Task<bool> ExisteAsync<T>(string indice, string id, string nomeConsulta, object parametro = null) where T : class
    {
        var mockResponse = await ObterRespostaMock<MockGetResponse<T>>();

        if (mockResponse == null || !mockResponse.IsValidResponse)
            throw new Exception("Erro simulado no Elasticsearch");

        return mockResponse.Found;
    }

    public new async Task<bool> InserirAsync<T>(T entidade, string indice) where T : class
    {
        var mockResponse = await ObterRespostaMock<MockIndexResponse>();
            
        if (mockResponse == null || !mockResponse.IsValidResponse)
            throw new Exception("Erro simulado no Elasticsearch");

        return true;
    }

    public new async Task ExcluirTodos<T>(string indice, string nomeConsulta) where T : class
    {
        var mockResponse = await ObterRespostaMock<MockDeleteByQueryResponse>();

        if (mockResponse == null || !mockResponse.IsValidResponse)
            throw new Exception("Erro simulado no Elasticsearch");
    }

    public new async Task InserirBulk<T>(IEnumerable<T> listaDeDocumentos, string indice) where T : class
    {
        var mockResponse = await ObterRespostaMock<MockBulkResponse>();

        if (mockResponse == null || !mockResponse.IsValidResponse || mockResponse.Errors)
            throw new Exception("Erro simulado no Elasticsearch");
    }

    private async Task<T> ObterRespostaMock<T>() where T : class
    {
        try
        {
            var resultado = await servicoTelemetria.RegistrarComRetornoAsync<T>(
                async () => await Task.FromResult<object>(Activator.CreateInstance(typeof(T))),
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);

            return resultado as T;
        }
        catch
        {
            return null;
        }
    }
}

public class EntidadeTeste
{
    public string Id { get; set; }
    public string Nome { get; set; }
}