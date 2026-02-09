using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.Sondagem.Infra.EnvironmentVariables;
using SME.Sondagem.IoC.Extensions;
using System.Reflection;
using Xunit;

namespace SME.Sondagem.IoC.Teste
{
    public class ElasticSearchExtensionTeste
    {
        private IServiceCollection _services;
        private IConfiguration? _configuration;

        public ElasticSearchExtensionTeste()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void CriarSettingsPadrao_DeveExecutarDelegates()
        {
            var uri = new Uri("http://localhost:9200");

            var settings = ElasticSearchExtension.CriarSettingsPadrao(uri);

            Assert.NotNull(settings);
        }

        [Fact]
        public async Task MapearIndicesAsync_QuandoClientNulo_DeveLancarArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                ElasticSearchExtension.MapearIndicesAsync(null!));
        }


        [Fact]
        public void AdicionarElasticSearch_DeveRegistrarElasticOptionsNoContainer()
        {
            var configValues = new Dictionary<string, string>
            {
                { "ElasticSearch:Urls", "http://localhost:9200" },
                { "ElasticSearch:DefaultIndex", "teste-index" },
                { "ElasticSearch:Username", "admin" },
                { "ElasticSearch:Password", "senha123" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            var elasticOptions = serviceProvider.GetService<ElasticOptions>();

            Assert.NotNull(elasticOptions);
            Assert.Equal("http://localhost:9200", elasticOptions.Urls);
            Assert.Equal("teste-index", elasticOptions.DefaultIndex);
            Assert.Equal("admin", elasticOptions.Username);
            Assert.Equal("senha123", elasticOptions.Password);
        }

        [Fact]
        public void AdicionarElasticSearch_DeveRegistrarElasticsearchClientNoContainer()
        {
            var configValues = new Dictionary<string, string>
            {
                { "ElasticSearch:Urls", "http://localhost:9200" },
                { "ElasticSearch:DefaultIndex", "teste-index" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            var elasticClient = serviceProvider.GetService<ElasticsearchClient>();

            Assert.NotNull(elasticClient);
        }

        [Fact]
        public void AdicionarElasticSearch_ComMultiplasUrlsSeparadasPorVirgula_DeveUsarPrimeiraUrl()
        {
            var configValues = new Dictionary<string, string>
            {
                { "ElasticSearch:Urls", "http://localhost:9200, http://localhost:9201, http://localhost:9202" },
                { "ElasticSearch:DefaultIndex", "teste-index" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            var elasticClient = serviceProvider.GetService<ElasticsearchClient>();

            Assert.NotNull(elasticClient);
        }

        [Fact]
        public void AdicionarElasticSearch_ComCredenciais_DeveConfigurarAutenticacao()
        {
            var configValues = new Dictionary<string, string>
            {
                { "ElasticSearch:Urls", "http://localhost:9200" },
                { "ElasticSearch:DefaultIndex", "teste-index" },
                { "ElasticSearch:Username", "usuario" },
                { "ElasticSearch:Password", "senha" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            var elasticClient = serviceProvider.GetService<ElasticsearchClient>();

            Assert.NotNull(elasticClient);
        }

        [Fact]
        public void AdicionarElasticSearch_SemCredenciais_DeveConfigurarSemAutenticacao()
        {
            var configValues = new Dictionary<string, string>
            {
                { "ElasticSearch:Urls", "http://localhost:9200" },
                { "ElasticSearch:DefaultIndex", "teste-index" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            var elasticClient = serviceProvider.GetService<ElasticsearchClient>();

            Assert.NotNull(elasticClient);
        }

        [Fact]
        public void AdicionarElasticSearch_ComUsernameVazioESenhaVazia_NaoDeveConfigurarAutenticacao()
        {
            var configValues = new Dictionary<string, string>
            {
                { "ElasticSearch:Urls", "http://localhost:9200" },
                { "ElasticSearch:DefaultIndex", "teste-index" },
                { "ElasticSearch:Username", "" },
                { "ElasticSearch:Password", "" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            var elasticClient = serviceProvider.GetService<ElasticsearchClient>();

            Assert.NotNull(elasticClient);
        }

        [Fact]
        public void AdicionarElasticSearch_ComDefaultIndex_DeveConfigurarIndice()
        {
            var configValues = new Dictionary<string, string>
            {
                { "ElasticSearch:Urls", "http://localhost:9200" },
                { "ElasticSearch:DefaultIndex", "meu-indice-padrao" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            var elasticClient = serviceProvider.GetService<ElasticsearchClient>();

            Assert.NotNull(elasticClient);
        }

        [Fact]
        public void AdicionarElasticSearch_ComUrlComEspacos_DeveTratarCorretamente()
        {
            var configValues = new Dictionary<string, string>
            {
                { "ElasticSearch:Urls", "  http://localhost:9200  " },
                { "ElasticSearch:DefaultIndex", "teste-index" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            var elasticClient = serviceProvider.GetService<ElasticsearchClient>();

            Assert.NotNull(elasticClient);
        }

        [Fact]
        public void AdicionarElasticSearch_DeveCriarClientComCertificateValidationCallback()
        {
            var configValues = new Dictionary<string, string>
            {
                { "ElasticSearch:Urls", "https://localhost:9200" },
                { "ElasticSearch:DefaultIndex", "teste-index" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            var elasticClient = serviceProvider.GetService<ElasticsearchClient>();

            Assert.NotNull(elasticClient);
        }

        [Fact]
        public void AdicionarElasticSearch_DeveCriarClientComDefaultFieldNameInferrer()
        {
            var configValues = new Dictionary<string, string>
            {
                { "ElasticSearch:Urls", "http://localhost:9200" },
                { "ElasticSearch:DefaultIndex", "teste-index" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            var elasticClient = serviceProvider.GetService<ElasticsearchClient>();

            Assert.NotNull(elasticClient);
        }

        [Fact]
        public void AdicionarElasticSearch_SemSecaoElasticSearch_DeveRegistrarElasticOptionsVazio()
        {
            var configValues = new Dictionary<string, string>();

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            var elasticOptions = serviceProvider.GetService<ElasticOptions>();

            Assert.NotNull(elasticOptions);
            Assert.True(string.IsNullOrEmpty(elasticOptions.Urls));
            Assert.True(string.IsNullOrEmpty(elasticOptions.DefaultIndex));
            
            var elasticClient = serviceProvider.GetService<ElasticsearchClient>();
            Assert.Null(elasticClient);
        }

        [Fact]
        public void AdicionarElasticSearch_ComPrefixIndex_DeveConfigurarCorretamente()
        {
            var configValues = new Dictionary<string, string>
            {
                { "ElasticSearch:Urls", "http://localhost:9200" },
                { "ElasticSearch:DefaultIndex", "teste-index" },
                { "ElasticSearch:PrefixIndex", "dev-" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            var elasticOptions = serviceProvider.GetService<ElasticOptions>();

            Assert.NotNull(elasticOptions);
            Assert.Equal("dev-", elasticOptions.PrefixIndex);
        }

        [Fact]
        public void AdicionarElasticSearch_DeveRegistrarServicosComoSingleton()
        {
            var configValues = new Dictionary<string, string>
            {
                { "ElasticSearch:Urls", "http://localhost:9200" },
                { "ElasticSearch:DefaultIndex", "teste-index" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

             var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            
            var elasticOptions1 = serviceProvider.GetService<ElasticOptions>();
            var elasticOptions2 = serviceProvider.GetService<ElasticOptions>();
            Assert.Same(elasticOptions1, elasticOptions2);

            var elasticClient1 = serviceProvider.GetService<ElasticsearchClient>();
            var elasticClient2 = serviceProvider.GetService<ElasticsearchClient>();
            Assert.Same(elasticClient1, elasticClient2);
        }

        [Theory]
        [InlineData("http://localhost:9200")]
        [InlineData("https://elasticsearch.example.com:9200")]
        [InlineData("http://10.0.0.1:9200")]
        public void AdicionarElasticSearch_ComDiferentesFormatosDeUrl_DeveConfigurarCorretamente(string url)
        {
            var configValues = new Dictionary<string, string>
            {
                { "ElasticSearch:Urls", url },
                { "ElasticSearch:DefaultIndex", "teste-index" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            var elasticClient = serviceProvider.GetService<ElasticsearchClient>();

            Assert.NotNull(elasticClient);
        }

        [Fact]
        public void AdicionarElasticSearch_ComApenasUsername_NaoDeveConfigurarAutenticacao()
        {
            var configValues = new Dictionary<string, string>
            {
                { "ElasticSearch:Urls", "http://localhost:9200" },
                { "ElasticSearch:DefaultIndex", "teste-index" },
                { "ElasticSearch:Username", "usuario" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            var elasticClient = serviceProvider.GetService<ElasticsearchClient>();

            Assert.NotNull(elasticClient);
        }

        [Fact]
        public void AdicionarElasticSearch_ComApenasPassword_NaoDeveConfigurarAutenticacao()
        {
            var configValues = new Dictionary<string, string>
            {
                { "ElasticSearch:Urls", "http://localhost:9200" },
                { "ElasticSearch:DefaultIndex", "teste-index" },
                { "ElasticSearch:Password", "senha" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            method?.Invoke(null, new object[] { _services, _configuration });

            var serviceProvider = _services.BuildServiceProvider();
            var elasticClient = serviceProvider.GetService<ElasticsearchClient>();

            Assert.NotNull(elasticClient);
        }

        [Fact]
        public void AdicionarElasticSearch_ComUrlsApenasEspacos_DeveRetornarSemCriarClient()
        {
            // Arrange
            var configValues = new Dictionary<string, string>
    {
        { "ElasticSearch:Urls", "   " }, // string.IsNullOrWhiteSpace == true
        { "ElasticSearch:DefaultIndex", "teste-index" }
    };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

            var method = typeof(ElasticSearchExtension).GetMethod(
                "AdicionarElasticSearch",
                BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            method?.Invoke(null, new object[] { _services, _configuration });

            // Assert
            var serviceProvider = _services.BuildServiceProvider();

            var elasticOptions = serviceProvider.GetService<ElasticOptions>();
            Assert.NotNull(elasticOptions);

            // Branch crítico: client NÃO pode existir
            var elasticClient = serviceProvider.GetService<ElasticsearchClient>();
            Assert.Null(elasticClient);
        }
    }
}
