using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.Sondagem.Dominio.Entidades.Elastic;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infra.EnvironmentVariables;

namespace SME.Sondagem.IoC.Extensions
{
    internal static class ElasticSearchExtension
    {
        internal static readonly Func<Uri, ElasticsearchClientSettings> CriarSettingsPadraoFunc =
            CriarSettingsPadrao;

        internal static readonly Func<ElasticsearchClientSettings, ElasticsearchClient> CriarClientPadraoFunc =
            settings => new ElasticsearchClient(settings);

        internal static readonly Func<ElasticsearchClient, Task> MapearIndicesPadraoFunc =
            MapearIndicesAsync;

        internal static Func<Uri, ElasticsearchClientSettings> CriarSettings { get; private set; }
            = CriarSettingsPadraoFunc;

        internal static Func<ElasticsearchClientSettings, ElasticsearchClient> CriarClient { get; private set; }
            = CriarClientPadraoFunc;

        internal static Func<ElasticsearchClient, Task> MapearIndices { get; private set; }
            = MapearIndicesPadraoFunc;

        internal static void OverrideForTests(
            Func<Uri, ElasticsearchClientSettings>? criarSettings = null,
            Func<ElasticsearchClientSettings, ElasticsearchClient>? criarClient = null,
            Func<ElasticsearchClient, Task>? mapearIndices = null)
        {
            CriarSettings = criarSettings ?? CriarSettingsPadraoFunc;
            CriarClient = criarClient ?? CriarClientPadraoFunc;
            MapearIndices = mapearIndices ?? MapearIndicesPadraoFunc;
        }

        internal static void AdicionarElasticSearch(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var elasticOptions = new ElasticOptions();
            configuration
                .GetSection(ElasticOptions.Secao)
                .Bind(elasticOptions, c => c.BindNonPublicProperties = true);

            services.AddSingleton(elasticOptions);

            if (string.IsNullOrWhiteSpace(elasticOptions.Urls))
                return;

            var uri = new Uri(elasticOptions.Urls.Split(',')[0].Trim());

            var settings = CriarSettings(uri);

            if (!string.IsNullOrEmpty(elasticOptions.DefaultIndex))
                settings.DefaultIndex(elasticOptions.DefaultIndex);

            if (!string.IsNullOrEmpty(elasticOptions.Username) &&
                !string.IsNullOrEmpty(elasticOptions.Password))
            {
                settings = settings.Authentication(
                    new BasicAuthentication(elasticOptions.Username, elasticOptions.Password));
            }

            var client = CriarClient(settings);

            MapearIndices(client).GetAwaiter().GetResult();

            services.AddSingleton(client);
        }

        internal static ElasticsearchClientSettings CriarSettingsPadrao(Uri uri)
        {
            return new ElasticsearchClientSettings(uri)
                .DefaultFieldNameInferrer(f => f.ToLowerInvariant())
                .ServerCertificateValidationCallback((_, _, _, _) => true);
        }

        internal static async Task MapearIndicesAsync(ElasticsearchClient elasticClient)
        {
            ArgumentNullException.ThrowIfNull(elasticClient);

            const string indiceAlunoMatriculaTurmaDre = IndicesElastic.INDICE_ALUNO_MATRICULA_TURMA_DRE;
            const string indiceTurma = IndicesElastic.INDICE_TURMA;

            try
            {
                var existsResponse =
                    await elasticClient.Indices.ExistsAsync(indiceAlunoMatriculaTurmaDre);

                if (!existsResponse.IsValidResponse)
                {
                    Console.WriteLine(
                        $"Não foi possível verificar a existência do índice: {indiceAlunoMatriculaTurmaDre}");
                    return;
                }

                if (existsResponse.Exists)
                {
                    Console.WriteLine(
                        $"Índice {indiceAlunoMatriculaTurmaDre} já existe.");
                    return;
                }

                var createResponse = await elasticClient.Indices.CreateAsync(
                    indiceTurma,
                    c => c.Mappings(m => m
                        .Properties<QuestionarioDto>(p => p
                            .Keyword(k => k.CodigoTurma)
                            .IntegerNumber(k => k.CodigoTurma)
                            .Keyword(k => k.NomeTurma)
                            .Text(k => k.NomeTurma)
                        )
                    )
                );

                if (!createResponse.IsValidResponse)
                {
                    Console.WriteLine(
                        $"Erro ao criar índice {indiceTurma}: " +
                        $"{createResponse.ElasticsearchServerError?.Error?.Reason}");
                }
                else
                {
                    Console.WriteLine(
                        $"Índice {indiceTurma} criado com sucesso!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao mapear índices: {ex.Message}");
                throw;
            }
        }
    }
}
