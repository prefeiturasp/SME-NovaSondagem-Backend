using Dapper;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SME.Sondagem.Dominio.Entidades.Elastic;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infra.EnvironmentVariables;

const int TamanhoLote = 10;
var intervaloEntreLotes = TimeSpan.FromSeconds(5);

var dryRun = args.Contains("--dry-run");
var loteInicial = ObterLoteInicial(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("SondagemConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("ConnectionStrings:SondagemConnection não configurada.");
    return 1;
}

var elasticOptions = new ElasticOptions();
configuration.GetSection(ElasticOptions.Secao).Bind(elasticOptions);

if (string.IsNullOrWhiteSpace(elasticOptions.Urls))
{
    Console.WriteLine("ElasticSearch:Urls não configurada.");
    return 1;
}

var elasticClient = CriarElasticClient(elasticOptions);

await using var conexao = new NpgsqlConnection(connectionString);
await conexao.OpenAsync();

var turmaIds = (await conexao.QueryAsync<string>(
        @"SELECT DISTINCT turma_id
          FROM resposta_aluno
          WHERE turma_id IS NOT NULL AND ano_turma IS NULL AND excluido = false"))
    .ToList();

Console.WriteLine($"{turmaIds.Count} turma(s) distinta(s) com ano_turma nulo encontrada(s).");
if (dryRun)
    Console.WriteLine("Modo --dry-run: nenhuma atualização será gravada.");

var lotes = turmaIds
    .Select((turmaId, indice) => (turmaId, indice))
    .GroupBy(x => x.indice / TamanhoLote)
    .Select(g => g.Select(x => x.turmaId).ToList())
    .ToList();

if (loteInicial > 1)
    Console.WriteLine($"Retomando a partir do lote {loteInicial}/{lotes.Count} (--start-lote).");

if (loteInicial > lotes.Count)
{
    Console.WriteLine($"--start-lote={loteInicial} é maior que o total de lotes ({lotes.Count}). Nada a fazer.");
    return 0;
}

var totalRespostasAtualizadas = 0;
var turmasNaoResolvidas = new List<string>();

for (var i = loteInicial - 1; i < lotes.Count; i++)
{
    var lote = lotes[i];

    try
    {
        Console.WriteLine($"Lote {i + 1}/{lotes.Count}: turmas [{string.Join(", ", lote)}]");

        var turmasElastic = await ObterTurmasPorIdsAsync(elasticClient, lote);

        foreach (var turmaId in lote)
        {
            if (!int.TryParse(turmaId, out var codigoTurma))
            {
                Console.WriteLine($"  turma {turmaId}: turma_id não é numérico, ignorada.");
                turmasNaoResolvidas.Add(turmaId);
                continue;
            }

            var turma = turmasElastic.FirstOrDefault(t => t.CodigoTurma == codigoTurma);
            if (turma is null)
            {
                Console.WriteLine($"  turma {turmaId}: não encontrada no Elastic.");
                turmasNaoResolvidas.Add(turmaId);
                continue;
            }

            if (!int.TryParse(turma.AnoTurma, out var anoTurma))
            {
                Console.WriteLine($"  turma {turmaId}: AnoTurma '{turma.AnoTurma}' inválido no Elastic.");
                turmasNaoResolvidas.Add(turmaId);
                continue;
            }

            if (dryRun)
            {
                var qtd = await conexao.ExecuteScalarAsync<int>(
                    @"SELECT count(*) FROM resposta_aluno
                      WHERE turma_id = @TurmaId AND ano_turma IS NULL AND excluido = false",
                    new { TurmaId = turmaId });

                Console.WriteLine($"  turma {turmaId}: {qtd} resposta(s) seriam atualizada(s) para ano_turma={anoTurma}.");
                totalRespostasAtualizadas += qtd;
                continue;
            }

            var linhasAfetadas = await conexao.ExecuteAsync(
                @"UPDATE resposta_aluno
                  SET ano_turma = @AnoTurma
                  WHERE turma_id = @TurmaId AND ano_turma IS NULL AND excluido = false",
                new { TurmaId = turmaId, AnoTurma = anoTurma });

            Console.WriteLine($"  turma {turmaId}: {linhasAfetadas} resposta(s) atualizada(s) para ano_turma={anoTurma}.");
            totalRespostasAtualizadas += linhasAfetadas;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine();
        Console.WriteLine($"ERRO no lote {i + 1}/{lotes.Count} (turmas [{string.Join(", ", lote)}]): {ex.Message}");
        Console.WriteLine($"Lotes 1 a {i} (se sem --dry-run) já foram gravados. Para continuar de onde parou, rode novamente com --start-lote={i + 1}.");
        return 1;
    }

    if (i < lotes.Count - 1)
        await Task.Delay(intervaloEntreLotes);
}

Console.WriteLine();
Console.WriteLine($"Concluído. {totalRespostasAtualizadas} resposta(s) {(dryRun ? "seriam atualizadas" : "atualizadas")}.");

if (turmasNaoResolvidas.Count > 0)
    Console.WriteLine($"{turmasNaoResolvidas.Count} turma(s) não resolvida(s): {string.Join(", ", turmasNaoResolvidas)}");

return 0;

static int ObterLoteInicial(string[] args)
{
    const string prefixo = "--start-lote=";
    var arg = args.FirstOrDefault(a => a.StartsWith(prefixo, StringComparison.OrdinalIgnoreCase));

    if (arg is null)
        return 1;

    var valor = arg[prefixo.Length..];
    return int.TryParse(valor, out var lote) && lote > 0 ? lote : 1;
}

static ElasticsearchClient CriarElasticClient(ElasticOptions options)
{
    var uri = new Uri(options.Urls.Split(',')[0].Trim());
    var settings = new ElasticsearchClientSettings(uri)
        .DefaultFieldNameInferrer(f => f.ToLowerInvariant())
        .ServerCertificateValidationCallback((_, _, _, _) => true);

    if (!string.IsNullOrEmpty(options.Username) && !string.IsNullOrEmpty(options.Password))
        settings = settings.Authentication(new BasicAuthentication(options.Username, options.Password));

    return new ElasticsearchClient(settings);
}

static async Task<List<TurmaElasticDto>> ObterTurmasPorIdsAsync(ElasticsearchClient client, List<string> turmaIds)
{
    var codigos = turmaIds
        .Select(id => int.TryParse(id, out var codigo) ? codigo : (int?)null)
        .Where(codigo => codigo.HasValue)
        .Select(codigo => codigo!.Value)
        .ToList();

    if (codigos.Count == 0)
        return [];

    Func<QueryDescriptor<TurmaElasticDto>, Query> query = q => q
        .Terms(t => t
            .Field(f => f.CodigoTurma)
            .Terms(new TermsQueryField(codigos.Select(codigo => FieldValue.Long(codigo)).ToArray())));

    var response = await client.SearchAsync<TurmaElasticDto>(s => s
        .Indices(IndicesElastic.INDICE_TURMA)
        .Query(q => query(q))
        .Size(codigos.Count * 10));

    if (!response.IsValidResponse)
    {
        Console.WriteLine($"  Erro ao consultar Elastic: {response.ElasticsearchServerError?.Error?.Reason}");
        return [];
    }

    return response.Documents.ToList();
}
