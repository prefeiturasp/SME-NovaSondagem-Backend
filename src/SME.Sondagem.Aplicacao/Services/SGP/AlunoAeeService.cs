using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Services;
using System.Net;

namespace SME.Sondagem.Aplicacao.Services.SGP;

public class AlunoAeeService : IAlunoAeeService
{
    private readonly IHttpClientFactory httpClientFactory;

    public AlunoAeeService(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task<Dictionary<int, bool>> VerificarAlunosPossuemPlanoAeeAsync(
        IEnumerable<int> codigosAlunos,
        int codigoTurma,
        string? codigoUe,
        CancellationToken cancellationToken = default)
    {
        var resultado = new Dictionary<int, bool>();

        if (codigosAlunos == null || !codigosAlunos.Any())
            return resultado;

        foreach (var codigoAluno in codigosAlunos)
            resultado[codigoAluno] = false;

        var httpClient = httpClientFactory.CreateClient(ServicoSgpConstants.SERVICO);
        var url = string.Format(ServicoSgpConstants.URL_PLANO_AEE_TURMA_EXISTE, codigoTurma);

        if (!string.IsNullOrWhiteSpace(codigoUe))
            url += $"?codigoUe={Uri.EscapeDataString(codigoUe)}";

        var response = await httpClient.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
            return resultado;

        ValidarRespostaSgp(response);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(json))
            return resultado;

        var planosAee = JsonConvert.DeserializeObject<IEnumerable<PlanoAEEResumoIntegracaoDto>>(json);
        var codigosComAee = planosAee?
            .Select(plano => int.TryParse(plano.CodigoAluno, out var codigoAluno) ? codigoAluno : (int?)null)
            .Where(codigoAluno => codigoAluno.HasValue)
            .Select(codigoAluno => codigoAluno!.Value)
            .ToHashSet() ?? new HashSet<int>();

        foreach (var codigoAluno in codigosAlunos)
            resultado[codigoAluno] = codigosComAee.Contains(codigoAluno);

        return resultado;
    }

    private static void ValidarRespostaSgp(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new RegraNegocioException(
                "Falha de autorização ao consultar planos AEE no SGP.",
                response.StatusCode);

        throw new RegraNegocioException(
            $"Falha ao consultar planos AEE no SGP. StatusCode: {(int)response.StatusCode}",
            HttpStatusCode.BadGateway);
    }
}
