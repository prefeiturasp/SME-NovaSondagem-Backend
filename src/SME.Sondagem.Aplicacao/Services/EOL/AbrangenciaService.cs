using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Constantes;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Services;
using System.Net;

namespace SME.Sondagem.Aplicacao.Services.EOL
{
    public class AbrangenciaService : IAbrangenciaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepositorioCache _repositorioCache;
        private readonly IPerfilService _perfilService;

        public AbrangenciaService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IRepositorioCache repositorioCache,
            IPerfilService perfilService)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _repositorioCache = repositorioCache ?? throw new ArgumentNullException(nameof(repositorioCache));
            _perfilService = perfilService ?? throw new ArgumentNullException(nameof(perfilService));
        }

        public async Task<bool> DeveIgnorarAbrangenciaAsync(string? perfil = null, CancellationToken cancellationToken = default)
        {
            var perfilIdString = !string.IsNullOrWhiteSpace(perfil)
                ? perfil
                : _httpContextAccessor.HttpContext?.User?.FindFirst("perfil")?.Value;

            if (!Guid.TryParse(perfilIdString, out var perfilId))
                return false;

            var perfilInfo = await _perfilService.ObterPerfilPorIdAsync(perfilId, cancellationToken);

            return perfilInfo is not null &&
                   (perfilInfo.AcessoIrrestrito || perfilInfo.TipoValidacao == "AcessoTotal");
        }

        public async Task<(List<string> Dres, List<string> Ues, List<string> Turmas)> ObterAbrangenciaCompletaAsync(
            int anoLetivo, int modalidade, string? codigoDre, string? codigoUe = null, int semestre = 0, string? rf = null, string? perfil = null, CancellationToken cancellationToken = default)
        {
            var (loginCtx, perfilCtx) = ObterLoginEPerfil();
            var login = !string.IsNullOrWhiteSpace(rf) ? rf : loginCtx;
            perfil = !string.IsNullOrWhiteSpace(perfil) ? perfil : perfilCtx;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(perfil))
                return ([], [], []);

            var chave = string.Format(NomeChaveCache.ABRANGENCIA_COMPLETA_USUARIO, login, perfil, anoLetivo, modalidade, codigoDre ?? "all", semestre);
            var url = string.Format(ServicoSgpConstants.URL_ABRANGENCIA_COMPLETA, login, perfil, anoLetivo, modalidade);

            if (!string.IsNullOrEmpty(codigoDre))
                url += $"&codigoDre={codigoDre}";

            if (!string.IsNullOrEmpty(codigoUe))
                url += $"&codigoUe={codigoUe}";

            url += $"&semestre={semestre}";
            url += "&includeTurmas=true";

            var json = await ObterJsonComCacheAsync(chave, url, cancellationToken, ServicoSgpConstants.SERVICO)
                ?? throw new InvalidOperationException("Falha ao obter abrangência do SGP. Tente novamente.");

            if (string.IsNullOrWhiteSpace(json))
                return ([], [], []);

            var resultado = JsonConvert.DeserializeObject<dynamic>(json);

            var dres = ((IEnumerable<dynamic>)resultado!.dres)
                .Select(d => (string)d.codigo)
                .ToList();

            var ues = ((IEnumerable<dynamic>)resultado!.ues)
                .Select(u => (string)u.codigo)
                .ToList();

            var turmas = ((IEnumerable<dynamic>)resultado!.turmas)
                .Select(t => (string)t.codigo)
                .ToList();

            return (dres, ues, turmas);
        }

        private (string? login, string? perfil) ObterLoginEPerfil()
        {
            var usuario = _httpContextAccessor.HttpContext?.User;
            if (usuario == null || !usuario.Identity!.IsAuthenticated)
                return (null, null);
            return (usuario.FindFirst("rf")?.Value, usuario.FindFirst("perfil")?.Value);
        }

        private async Task<string?> ObterJsonComCacheAsync(
            string chave,
            string url,
            CancellationToken cancellationToken,
            string serviceName = ServicoEolConstants.SERVICO)
        {
            var cacheRedis = await _repositorioCache.ObterRedisToJsonAsync(chave);
            if (!string.IsNullOrEmpty(cacheRedis))
                return cacheRedis;

            var httpClient = _httpClientFactory.CreateClient(serviceName);
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            if (response.StatusCode == HttpStatusCode.NoContent)
                return string.Empty;

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
                return null;

            await _repositorioCache.SalvarRedisToJsonAsync(chave, json);
            return json;
        }
    }
}
