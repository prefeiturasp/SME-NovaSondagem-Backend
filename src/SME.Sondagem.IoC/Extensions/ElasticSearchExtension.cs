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
        internal static void AdicionarElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            var elasticOptions = new ElasticOptions();
            configuration.GetSection(ElasticOptions.Secao).Bind(elasticOptions, c => c.BindNonPublicProperties = true);
            services.AddSingleton(elasticOptions);

            if (string.IsNullOrWhiteSpace(elasticOptions.Urls)) return;

            var uri = new Uri(elasticOptions.Urls.Split(',')[0].Trim());

            var settings = new ElasticsearchClientSettings(uri)
                .DefaultFieldNameInferrer(f => f.ToLowerInvariant())
                .ServerCertificateValidationCallback((_, _, _, _) => true);

            settings.DefaultIndex(elasticOptions.DefaultIndex);

            if (!string.IsNullOrEmpty(elasticOptions.Username) && !string.IsNullOrEmpty(elasticOptions.Password))
            {
                settings = settings.Authentication(new BasicAuthentication(elasticOptions.Username, elasticOptions.Password));
            }

            var client = new ElasticsearchClient(settings);

            MapearIndicesAsync(client).GetAwaiter().GetResult();

            services.AddSingleton(client);
        }

        private static async Task MapearIndicesAsync(ElasticsearchClient elasticClient)
        {
            const string indiceAlunoMatriculaTurmaDre = IndicesElastic.INDICE_ALUNO_MATRICULA_TURMA_DRE;
            const string indiceTurma = IndicesElastic.INDICE_TURMA;

            try
            {
                // Verifica se o índice principal já existe
                var existsResponse = await elasticClient.Indices.ExistsAsync(indiceAlunoMatriculaTurmaDre);
                
                if (!existsResponse.IsValidResponse)
                {
                    Console.WriteLine($"Não foi possível verificar a existência do índice: {indiceAlunoMatriculaTurmaDre}");
                    return;
                }

                if (existsResponse.Exists)
                {
                    Console.WriteLine($"Índice {indiceAlunoMatriculaTurmaDre} já existe.");
                    return;
                }

                // Cria o índice de turma
                var createResponse = await elasticClient.Indices.CreateAsync(indiceTurma, c => c
                    .Mappings(m => m
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
                    Console.WriteLine($"Erro ao criar índice {indiceTurma}: {createResponse.ElasticsearchServerError?.Error?.Reason}");
                }
                else
                {
                    Console.WriteLine($"Índice {indiceTurma} criado com sucesso!");
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