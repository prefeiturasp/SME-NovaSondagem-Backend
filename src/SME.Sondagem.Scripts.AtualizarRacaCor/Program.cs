using System.Net.Http.Headers;
using System.Text.Json;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

// ===== Configuração da execução — editar aqui em vez de passar argumentos de linha de comando =====
var dryRun = false; // true: só reporta o que mudaria, não grava nada
var fase = "aluno"; // "turma" (padrão, em lote) ou "aluno" (fallback, roda depois da fase turma)
long? startFrom = 7852587; // id da turma (fase turma) ou do aluno (fase aluno) pra retomar após uma parada
int? limite = 1000; // fase turma: tamanho de cada bloco buscado do banco (evita SELECT gigante de uma vez); fase aluno: só controla o checkpoint no console. null = padrão 3000 na fase turma, sem checkpoint na fase aluno
// ====================================================================================================

var intervaloEntreChamadas = TimeSpan.FromSeconds(3);
var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

var diretorioBase = AppContext.BaseDirectory;
var diretorioSaida = @"C:\Projetos\SME-NovaSondagem-Backend";
var caminhoNaoResolvidos = Path.Combine(diretorioSaida, "nao-resolvidos.txt");
var caminhoPendentesFallback = Path.Combine(diretorioSaida, "pendentes-fallback.txt");
var caminhoAlteracoes = Path.Combine(diretorioSaida, "alteracoes.csv");

var configuration = new ConfigurationBuilder()
    .SetBasePath(diretorioBase)
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

var eolBaseUrl = configuration["EolApi:BaseUrl"];
var eolApiKey = configuration["EolApi:ApiKey"];
if (string.IsNullOrWhiteSpace(eolBaseUrl) || string.IsNullOrWhiteSpace(eolApiKey))
{
    Console.WriteLine("EolApi:BaseUrl e EolApi:ApiKey precisam estar configurados.");
    return 1;
}

using var httpClient = new HttpClient
{
    BaseAddress = new Uri(eolBaseUrl),
    Timeout = TimeSpan.FromSeconds(180)
};
httpClient.DefaultRequestHeaders.Add("x-api-eol-key", eolApiKey);
httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

await using var conexao = new NpgsqlConnection(connectionString);
await conexao.OpenAsync();

var racaCorPorDescricao = await CarregarRacaCorAsync(conexao);
if (racaCorPorDescricao.Count == 0)
{
    Console.WriteLine("Tabela raca_cor não retornou nenhuma linha (excluido = false). Nada a fazer.");
    return 1;
}

Console.WriteLine($"{racaCorPorDescricao.Count} descrição(ões) de raca_cor carregada(s).");
if (dryRun)
    Console.WriteLine("Modo --dry-run: nenhuma atualização será gravada.");

return fase.Equals("aluno", StringComparison.OrdinalIgnoreCase)
    ? await ExecutarFaseAlunoAsync()
    : await ExecutarFaseTurmaAsync();

async Task<int> ExecutarFaseTurmaAsync()
{
    // turma_id fora do padrão numérico não entra na paginação (o cast pra bigint quebraria) —
    // são poucos, então uma consulta única pra achar e logar é suficiente.
    var naoNumericas = await conexao.QueryAsync<string>(
        @"SELECT DISTINCT turma_id FROM resposta_aluno
          WHERE excluido = false AND turma_id IS NOT NULL AND turma_id !~ '^[0-9]+$'");

    foreach (var turmaTexto in naoNumericas)
        RegistrarNaoResolvido(0, $"turma_id não numérico: '{turmaTexto}'", "turma");

    var tamanhoBloco = limite ?? 3000;
    var cursor = startFrom ?? 0L;

    var totalTurmasAProcessar = await conexao.ExecuteScalarAsync<long>(
        @"SELECT count(DISTINCT turma_id::bigint)
          FROM resposta_aluno
          WHERE excluido = false AND turma_id IS NOT NULL AND turma_id ~ '^[0-9]+$' AND turma_id::bigint > @Cursor",
        new { Cursor = cursor });

    Console.WriteLine($"{totalTurmasAProcessar} turma(s) numérica(s) pra processar (turma_id > {cursor}). Buscando em blocos de {tamanhoBloco} (tabela é grande, não carrega tudo de uma vez).");

    var totalAlunosAtualizados = 0;
    var totalLinhasAtualizadas = 0;
    var totalTurmasProcessadas = 0;
    var numeroBloco = 0;

    while (true)
    {
        // Paginação por keyset (turma_id::bigint > cursor), não OFFSET — não refaz trabalho e não
        // pula/duplica turma mesmo se novas linhas forem inseridas em resposta_aluno durante a execução.
        var bloco = (await conexao.QueryAsync<long>(
                @"SELECT DISTINCT turma_id::bigint AS turma_id
                  FROM resposta_aluno
                  WHERE excluido = false AND turma_id IS NOT NULL AND turma_id ~ '^[0-9]+$' AND turma_id::bigint > @Cursor
                  ORDER BY turma_id::bigint
                  LIMIT @TamanhoBloco",
                new { Cursor = cursor, TamanhoBloco = tamanhoBloco }))
            .ToList();

        if (bloco.Count == 0)
            break;

        numeroBloco++;
        Console.WriteLine($"--- bloco {numeroBloco}: {bloco.Count} turma(s) (turma_id > {cursor}) ---");

        foreach (var turmaId in bloco)
        {
            try
            {
                var alunosDaTurmaNoBanco = (await conexao.QueryAsync<long>(
                        @"SELECT DISTINCT aluno_id FROM resposta_aluno WHERE turma_id = @TurmaId AND excluido = false",
                        new { TurmaId = turmaId.ToString() }))
                    .ToHashSet();

                var alunosRetornados = await ObterAlunosDaTurmaNoEolAsync(turmaId);
                totalTurmasProcessadas++;
                var progresso = $"[{totalTurmasProcessadas}/{totalTurmasAProcessar} turmas executadas]";

                if (alunosRetornados is null)
                {
                    Console.WriteLine($"{progresso} turma {turmaId}: sem retorno do EOL, {alunosDaTurmaNoBanco.Count} aluno(s) foram para o fallback.");
                    foreach (var alunoId in alunosDaTurmaNoBanco)
                        RegistrarPendenteFallback(alunoId);
                }
                else
                {
                    var alunosResolvidosNestaTurma = new HashSet<long>();
                    var alunosAtualizados = 0;
                    var linhasAtualizadasNestaTurma = 0;

                    foreach (var aluno in alunosRetornados)
                    {
                        alunosResolvidosNestaTurma.Add(aluno.CodigoAluno);
                        var linhas = await AtualizarRacaDoAlunoAsync(aluno.CodigoAluno, aluno.Raca, "turma");
                        if (linhas > 0)
                        {
                            alunosAtualizados++;
                            linhasAtualizadasNestaTurma += linhas;
                        }
                    }

                    var alunosParaFallback = 0;
                    foreach (var alunoId in alunosDaTurmaNoBanco)
                    {
                        if (!alunosResolvidosNestaTurma.Contains(alunoId))
                        {
                            RegistrarPendenteFallback(alunoId);
                            alunosParaFallback++;
                        }
                    }

                    totalAlunosAtualizados += alunosAtualizados;
                    totalLinhasAtualizadas += linhasAtualizadasNestaTurma;

                    Console.WriteLine($"{progresso} turma {turmaId}: {alunosRetornados.Count} aluno(s) no EOL, {alunosAtualizados} aluno(s) {(dryRun ? "seriam atualizados" : "atualizados")} ({linhasAtualizadasNestaTurma} linha(s)), {alunosParaFallback} para o fallback.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"ERRO na turma {turmaId}: {ex.Message}");
                Console.WriteLine($"Parcial até aqui: {totalTurmasProcessadas} turma(s) em {numeroBloco} bloco(s), {totalAlunosAtualizados} aluno(s) {(dryRun ? "seriam atualizados" : "atualizados")}, {totalLinhasAtualizadas} linha(s) de resposta_aluno.");
                Console.WriteLine($"Os arquivos '{caminhoNaoResolvidos}', '{caminhoPendentesFallback}' e '{caminhoAlteracoes}' já estão gravados até este ponto — nada se perde.");
                Console.WriteLine($"Para continuar, ajuste startFrom = {turmaId}L no código e rode de novo.");
                return 1;
            }

            cursor = turmaId; // só avança depois de processar com sucesso
            await Task.Delay(intervaloEntreChamadas);
        }
    }

    Console.WriteLine();
    Console.WriteLine($"Concluído. {totalTurmasProcessadas} turma(s) em {numeroBloco} bloco(s). {totalAlunosAtualizados} aluno(s) {(dryRun ? "seriam atualizados" : "atualizados")}, {totalLinhasAtualizadas} linha(s) de resposta_aluno.");

    if (File.Exists(caminhoPendentesFallback))
        Console.WriteLine($"Há aluno(s) pendente(s) em '{caminhoPendentesFallback}'. Ajuste fase = \"aluno\" e rode de novo pra tentar resolvê-los individualmente.");

    return 0;
}

async Task<int> ExecutarFaseAlunoAsync()
{
    if (!File.Exists(caminhoPendentesFallback))
    {
        Console.WriteLine($"Arquivo '{caminhoPendentesFallback}' não existe. Rode a fase turma primeiro.");
        return 0;
    }

    var alunosPendentes = File.ReadAllLines(caminhoPendentesFallback)
        .Select(linha => long.TryParse(linha.Trim(), out var id) ? id : (long?)null)
        .Where(id => id.HasValue)
        .Select(id => id!.Value)
        .Distinct()
        .Order()
        .ToList();

    var worklist = startFrom.HasValue
        ? alunosPendentes.Where(a => a >= startFrom.Value).ToList()
        : alunosPendentes;

    Console.WriteLine($"{alunosPendentes.Count} aluno(s) pendente(s) de fallback.");
    if (startFrom.HasValue)
        Console.WriteLine($"Retomando a partir do aluno {startFrom.Value} (variável startFrom). {worklist.Count} aluno(s) restante(s).");
    if (limite.HasValue)
        Console.WriteLine($"Processando em lotes de {limite.Value} aluno(s), sem parar entre lotes (limite só controla o checkpoint no console).");

    var totalAtualizados = 0;
    var totalLinhas = 0;

    for (var i = 0; i < worklist.Count; i++)
    {
        var alunoId = worklist[i];

        try
        {
            var progresso = $"[{i + 1}/{worklist.Count}]";
            var grupoEtnico = await ObterGrupoEtnicoDoAlunoNoEolAsync(alunoId);

            if (string.IsNullOrWhiteSpace(grupoEtnico))
            {
                Console.WriteLine($"{progresso} aluno {alunoId}: sem retorno do EOL.");
                RegistrarNaoResolvido(alunoId, "sem retorno do EOL na fase aluno", "aluno");
            }
            else
            {
                var linhas = await AtualizarRacaDoAlunoAsync(alunoId, grupoEtnico, "aluno");
                if (linhas > 0)
                {
                    totalAtualizados++;
                    totalLinhas += linhas;
                }

                Console.WriteLine($"{progresso} aluno {alunoId}: {linhas} linha(s) {(dryRun ? "seriam atualizadas" : "atualizadas")}.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"ERRO no aluno {alunoId}: {ex.Message}");
            Console.WriteLine($"Parcial até aqui: {totalAtualizados} aluno(s) {(dryRun ? "seriam atualizados" : "atualizados")}, {totalLinhas} linha(s) de resposta_aluno.");
            Console.WriteLine($"Os arquivos '{caminhoNaoResolvidos}', '{caminhoPendentesFallback}' e '{caminhoAlteracoes}' já estão gravados até este ponto — nada se perde.");
            Console.WriteLine($"Para continuar, ajuste fase = \"aluno\" e startFrom = {alunoId}L no código e rode de novo.");
            return 1;
        }

        if (limite.HasValue && (i + 1) % limite.Value == 0 && i < worklist.Count - 1)
            Console.WriteLine($"--- checkpoint: {i + 1}/{worklist.Count} aluno(s) processado(s), próximo seria startFrom={worklist[i + 1]} (com fase=\"aluno\") se precisar retomar depois. Continuando... ---");

        if (i < worklist.Count - 1)
            await Task.Delay(intervaloEntreChamadas);
    }

    Console.WriteLine();
    Console.WriteLine($"Concluído. {totalAtualizados} aluno(s) {(dryRun ? "seriam atualizados" : "atualizados")}, {totalLinhas} linha(s) de resposta_aluno.");
    Console.WriteLine($"Não resolvidos definitivamente: veja '{caminhoNaoResolvidos}'.");

    return 0;
}

async Task<int> AtualizarRacaDoAlunoAsync(long alunoId, string? textoRaca, string contexto)
{
    var descricaoNormalizada = NormalizarDescricao(textoRaca);

    if (descricaoNormalizada is null || !racaCorPorDescricao.TryGetValue(descricaoNormalizada, out var novoRacaCorId))
    {
        RegistrarNaoResolvido(alunoId, $"raça não reconhecida: '{textoRaca}'", contexto);
        return 0;
    }

    int linhasAfetadas;

    if (dryRun)
    {
        linhasAfetadas = await conexao.ExecuteScalarAsync<int>(
            @"SELECT count(*) FROM resposta_aluno
              WHERE aluno_id = @AlunoId AND excluido = false AND raca_cor_id IS DISTINCT FROM @NovoRacaCorId",
            new { AlunoId = alunoId, NovoRacaCorId = novoRacaCorId });
    }
    else
    {
        linhasAfetadas = await conexao.ExecuteAsync(
            @"UPDATE resposta_aluno
              SET raca_cor_id = @NovoRacaCorId
              WHERE aluno_id = @AlunoId AND excluido = false AND raca_cor_id IS DISTINCT FROM @NovoRacaCorId",
            new { AlunoId = alunoId, NovoRacaCorId = novoRacaCorId });
    }

    if (linhasAfetadas > 0)
        RegistrarAlteracao(alunoId, novoRacaCorId, linhasAfetadas);

    return linhasAfetadas;
}

async Task<List<AlunoTurmaInfo>?> ObterAlunosDaTurmaNoEolAsync(long turmaId)
{
    // Exceções de rede/timeout propagam pro catch do laço principal e interrompem a execução
    // (comportamento esperado: só resposta "sem dado" é tratada aqui, erro de verdade não é engolido).
    var resposta = await httpClient.GetAsync($"alunos/{turmaId}/turma/informacoes");

    if (!resposta.IsSuccessStatusCode || resposta.StatusCode == System.Net.HttpStatusCode.NoContent)
        return null;

    var json = await resposta.Content.ReadAsStringAsync();
    if (string.IsNullOrWhiteSpace(json))
        return null;

    var alunos = JsonSerializer.Deserialize<List<AlunoTurmaInfo>>(json, jsonOptions);
    return alunos is { Count: > 0 } ? alunos : null;
}

async Task<string?> ObterGrupoEtnicoDoAlunoNoEolAsync(long alunoId)
{
    var resposta = await httpClient.GetAsync($"alunos/{alunoId}/informacoes");

    if (!resposta.IsSuccessStatusCode || resposta.StatusCode == System.Net.HttpStatusCode.NoContent)
        return null;

    var json = await resposta.Content.ReadAsStringAsync();
    if (string.IsNullOrWhiteSpace(json))
        return null;

    var dados = JsonSerializer.Deserialize<AlunoInformacoes>(json, jsonOptions);
    return dados?.GrupoEtnico;
}

void RegistrarNaoResolvido(long alunoId, string motivo, string contexto)
{
    File.AppendAllText(caminhoNaoResolvidos,
        $"{DateTime.UtcNow:O}\tcontexto={contexto}\talunoId={alunoId}\tmotivo={motivo}{Environment.NewLine}");
}

void RegistrarPendenteFallback(long alunoId)
{
    File.AppendAllText(caminhoPendentesFallback, $"{alunoId}{Environment.NewLine}");
}

void RegistrarAlteracao(long alunoId, int novoRacaCorId, int linhasAtualizadas)
{
    if (!File.Exists(caminhoAlteracoes))
        File.AppendAllText(caminhoAlteracoes, $"aluno_id,raca_cor_id_novo,linhas_atualizadas,timestamp{Environment.NewLine}");

    File.AppendAllText(caminhoAlteracoes,
        $"{alunoId},{novoRacaCorId},{linhasAtualizadas},{DateTime.UtcNow:O}{Environment.NewLine}");
}

static string? NormalizarDescricao(string? texto) =>
    string.IsNullOrWhiteSpace(texto) ? null : texto.Trim().ToUpperInvariant();

static async Task<Dictionary<string, int>> CarregarRacaCorAsync(NpgsqlConnection conexao)
{
    var linhas = await conexao.QueryAsync<RacaCorRow>(
        "SELECT descricao AS Descricao, id AS Id FROM raca_cor WHERE excluido = false");

    var dicionario = new Dictionary<string, int>();

    foreach (var grupo in linhas.GroupBy(l => NormalizarDescricao(l.Descricao)))
    {
        if (grupo.Key is null)
            continue;

        var primeiro = grupo.First();

        if (grupo.Count() > 1)
            Console.WriteLine($"AVISO: descrição '{grupo.Key}' duplicada em raca_cor (ids: {string.Join(", ", grupo.Select(g => g.Id))}). Usando id={primeiro.Id}.");

        dicionario[grupo.Key] = primeiro.Id;
    }

    return dicionario;
}

record AlunoTurmaInfo(long CodigoAluno, string? Raca);

record AlunoInformacoes(string? GrupoEtnico);

class RacaCorRow
{
    public string Descricao { get; set; } = string.Empty;
    public int Id { get; set; }
}
