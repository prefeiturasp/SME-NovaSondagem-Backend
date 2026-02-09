using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio.Constantes;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infra.Services;
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

        public static readonly Guid PERFIL_CP = Guid.Parse("44e1e074-37d6-e911-abd6-f81654fe895d");
        public static readonly Guid PERFIL_AD = Guid.Parse("45e1e074-37d6-e911-abd6-f81654fe895d");
        public static readonly Guid PERFIL_DIRETOR = Guid.Parse("46e1e074-37d6-e911-abd6-f81654fe895d");
        public static readonly Guid PERFIL_PROFESSOR = Guid.Parse("40e1e074-37d6-e911-abd6-f81654fe895d");
        public static readonly Guid PERFIL_ADM_SME = Guid.Parse("5ae1e074-37d6-e911-abd6-f81654fe895d");

        private const int CACHE_TTL_TURMA_MINUTOS = 30;

        public ControleAcessoService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IRepositorioCache repositorioCache,
            IRepositorioElasticTurma repositorioTurma)
        {
            this.httpClientFactory = httpClientFactory;
            this.httpContextAccessor = httpContextAccessor;
            this.repositorioCache = repositorioCache;
            this.repositorioTurma = repositorioTurma;
        }

        public async Task<bool> ValidarPermissaoAcessoAsync(
            string turmaId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(turmaId))
                return false;

            var perfil = httpContextAccessor.HttpContext?.User?.FindFirst("perfil")?.Value;

            if (!Guid.TryParse(perfil, out var perfilGuid))
                return false;

            if (perfilGuid == PERFIL_ADM_SME)
            {
                if (!int.TryParse(turmaId, out var codigoTurma))
                    return false;

                var turmaElastic = await ObterTurmaComCacheAsync(codigoTurma, cancellationToken);
                return turmaElastic is not null;
            }

            var acessos = await ObterControleAcessoUsuarioAutenticadoAsync(cancellationToken);
            if (!acessos.Any())
                return false;

            if (perfilGuid == PERFIL_PROFESSOR)
            {
                return acessos.Any(a =>
                    a.Regencia &&
                    a.TurmaCodigos.Contains(turmaId));
            }

            if (!int.TryParse(turmaId, out var codigoTurmaValidacao))
                return false;

            var turmaElasticValidacao = await ObterTurmaComCacheAsync(
                codigoTurmaValidacao,
                cancellationToken);

            if (turmaElasticValidacao is null)
                return false;

            return acessos.Any(a =>
                a.IdUes.Contains(turmaElasticValidacao.CodigoEscola));
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
                CancellationToken cancellationToken)
        {
            var usuario = httpContextAccessor.HttpContext?.User;

            if (usuario == null || !usuario.Identity!.IsAuthenticated)
                return Enumerable.Empty<ControleAcessoDto>();

            var login = usuario.FindFirst("rf")?.Value;
            var perfil = usuario.FindFirst("perfil")?.Value;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(perfil))
                return Enumerable.Empty<ControleAcessoDto>();

            var chave = string.Format(
                NomeChaveCache.CONTROLE_ACESSO_USUARIO,
                login,
                perfil);

            var cacheRedis = await repositorioCache.ObterRedisToJsonAsync(chave);
            if (!string.IsNullOrEmpty(cacheRedis))
                return MapearControleAcesso(cacheRedis, perfil);

            if (!Guid.TryParse(perfil, out var perfilGuid))
                return Enumerable.Empty<ControleAcessoDto>();

            var httpClient = httpClientFactory.CreateClient(ServicoEolConstants.SERVICO);

            var url = perfilGuid == PERFIL_PROFESSOR
                ? string.Format(
                    ServicoEolConstants.URL_COMPONENTES_CURRICULARES_FUNCIONARIOS,
                    login,
                    perfil)
                : string.Format(
                    ServicoEolConstants.URL_ABRANGENCIA_COMPACTA_VIGENTE_PERFIL,
                    login,
                    perfil);

            var response = await httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
                return Enumerable.Empty<ControleAcessoDto>();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
                return Enumerable.Empty<ControleAcessoDto>();

            await repositorioCache.SalvarRedisToJsonAsync(chave, json);

            return MapearControleAcesso(json, perfil);
        }

        private static IEnumerable<ControleAcessoDto> MapearControleAcesso(
            string json,
            string perfil)
        {
            if (!Guid.TryParse(perfil, out var perfilGuid))
                return Enumerable.Empty<ControleAcessoDto>();

            if (perfilGuid == PERFIL_PROFESSOR)
            {
                var dados = JsonConvert.DeserializeObject<List<dynamic>>(json);

                return dados!
                    .Where(d => d.regencia)
                    .Select(d => new ControleAcessoDto
                    {
                        Regencia = true,
                        TurmaCodigos = new[] { (string)d.turmaCodigo }
                    });
            }

            var abrangencia = JsonConvert.DeserializeObject<dynamic>(json);

            return new List<ControleAcessoDto>
            {
                new ControleAcessoDto
                {
                    IdUes = ((IEnumerable<dynamic>)abrangencia!.idUes)
                        .Select(u => (string)u)
                        .ToList()
                }
            };
        }
    }
}