using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio.Constantes;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using System.Net;

namespace SME.Sondagem.Aplicacao.Services.EOL
{
    public class ControleAcessoService : IControleAcessoService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IRepositorioCache repositorioCache;
        private readonly IRepositorioElasticTurma repositorioTurma;
        private readonly IPerfilService perfilService;

        private const int CACHE_TTL_TURMA_MINUTOS = 30;

        public ControleAcessoService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IRepositorioCache repositorioCache,
            IRepositorioElasticTurma repositorioTurma, IPerfilService perfilService)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            this.repositorioCache = repositorioCache ?? throw new ArgumentNullException(nameof(repositorioCache));
            this.repositorioTurma = repositorioTurma ?? throw new ArgumentNullException(nameof(repositorioTurma));
            this.perfilService = perfilService ?? throw new ArgumentNullException(nameof(perfilService));
        }

        public async Task<bool> ValidarPermissaoAcessoAsync(
            string turmaId,
            string codigoEscola = "",
            string anoTurma = "",
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(turmaId))
                return false;

            var perfilIdString = httpContextAccessor.HttpContext?.User?.FindFirst("perfil")?.Value;

            if (!Guid.TryParse(perfilIdString, out var perfilId))
                return false;

            var perfilInfo = await perfilService.ObterPerfilPorIdAsync(perfilId, cancellationToken);

            if (perfilInfo == null || !perfilInfo.PermiteConsultar)
                return false;

            if (perfilInfo.AcessoIrrestrito)
            {
                if (!int.TryParse(turmaId, out var codigoTurma))
                    return false;

                var turmaElastic = await ObterTurmaComCacheAsync(codigoTurma, cancellationToken);
                return turmaElastic is not null;
            }

            if (!perfilInfo.ConsultarAbrangencia)
                return false;

            var acessos = await ObterControleAcessoUsuarioAutenticadoAsync(
                perfilInfo,
                cancellationToken);

            if (!acessos.Any())
                return false;

            return perfilInfo.TipoValidacao switch
            {
                "Regencia" => ValidarPorRegencia(acessos, turmaId),
                "UE" => await ValidarPorUEAsync(acessos, turmaId, cancellationToken),
                _ => false
            };
        }

        private bool ValidarPorRegencia(IEnumerable<ControleAcessoDto> acessos, string turmaId)
        {
            return acessos.Any(a => a.Regencia && a.TurmaCodigos.Contains(turmaId));
        }

        private async Task<bool> ValidarPorUEAsync(
            IEnumerable<ControleAcessoDto> acessos,
            string turmaId,
            CancellationToken cancellationToken)
        {
            if (!int.TryParse(turmaId, out var codigoTurma))
                return false;

            var turma = await ObterTurmaComCacheAsync(codigoTurma, cancellationToken);

            if (turma is null)
                return false;

            return acessos.Any(a => a.IdUes.Contains(turma.CodigoEscola));
        }

        private async Task<TurmaElasticDto?> ObterTurmaComCacheAsync(
                    int codigoTurma,
                    CancellationToken cancellationToken)
        {
            var chaveCache = $"turma-elastic:{codigoTurma}";

            var turmaCached = await repositorioCache.ObterRedisAsync<TurmaElasticDto>(chaveCache);

            if (turmaCached is not null)
                return turmaCached;

            var turmaElastic = await repositorioTurma.ObterTurmaPorId(
                new FiltroQuestionario { TurmaId = codigoTurma },
                cancellationToken);

            if (turmaElastic is not null)
            {
                await repositorioCache.SalvarRedisAsync(
                    chaveCache,
                    turmaElastic,
                    CACHE_TTL_TURMA_MINUTOS);
            }

            return turmaElastic;
        }

        private async Task<IEnumerable<ControleAcessoDto>>
            ObterControleAcessoUsuarioAutenticadoAsync(
                PerfilInfoSondagemDto perfilInfo,
                CancellationToken cancellationToken)
        {
            var usuario = httpContextAccessor.HttpContext?.User;

            if (usuario == null || !usuario.Identity!.IsAuthenticated)
                return Enumerable.Empty<ControleAcessoDto>();

            var login = usuario.FindFirst("rf")?.Value;

            if (string.IsNullOrWhiteSpace(login))
                return Enumerable.Empty<ControleAcessoDto>();

            var chave = string.Format(
                NomeChaveCache.CONTROLE_ACESSO_USUARIO,
                login,
                perfilInfo.Codigo);

            var cacheRedis = await repositorioCache.ObterRedisToJsonAsync(chave);
            if (!string.IsNullOrEmpty(cacheRedis))
                return MapearControleAcesso(cacheRedis, perfilInfo);

            var httpClient = httpClientFactory.CreateClient(ServicoEolConstants.SERVICO);

            var url = perfilInfo.TipoValidacao == "Regencia"
                ? string.Format(
                    ServicoEolConstants.URL_COMPONENTES_CURRICULARES_FUNCIONARIOS,
                    login,
                    perfilInfo.Codigo)
                : string.Format(
                    ServicoEolConstants.URL_ABRANGENCIA_COMPACTA_VIGENTE_PERFIL,
                    login,
                    perfilInfo.Codigo);

            var response = await httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
                return Enumerable.Empty<ControleAcessoDto>();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
                return Enumerable.Empty<ControleAcessoDto>();

            await repositorioCache.SalvarRedisToJsonAsync(chave, json);

            return MapearControleAcesso(json, perfilInfo);
        }

        private static IEnumerable<ControleAcessoDto> MapearControleAcesso(
            string json,
            PerfilInfoSondagemDto perfilInfo)
        {
            if (perfilInfo.TipoValidacao == "Regencia")
            {
                var dados = JsonConvert.DeserializeObject<List<dynamic>>(json);

                return dados!
                    .Where(d => d.regencia)
                    .Select(d => new ControleAcessoDto
                    {
                        Regencia = true,
                        TurmaCodigos = [(string)d.turmaCodigo]
                    });
            }

            var abrangencia = JsonConvert.DeserializeObject<dynamic>(json);

            return
            [
                new() {
                    IdUes = ((IEnumerable<dynamic>)abrangencia!.idUes)
                        .Select(u => (string)u)
                        .ToList()
                }
            ];
        }
    }
}