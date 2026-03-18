using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Configuration;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Services.EOL
{
    public class PerfilService : IPerfilService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IRepositorioCache repositorioCache;
        private readonly ControleAcessoOptions options;

        private const int CACHE_TTL_MINUTOS = 60;

        public PerfilService(IHttpClientFactory httpClientFactory, IRepositorioCache repositorioCache, ControleAcessoOptions options)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.repositorioCache = repositorioCache ?? throw new ArgumentNullException(nameof(repositorioCache));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<PerfilInfo?> ObterPerfilPorIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var cacheKey = $"perfil-config:{id}";
            var perfilCached = await repositorioCache.ObterRedisAsync<PerfilInfo>(cacheKey);
            if (perfilCached != null)
                return perfilCached;

            var httpClient = httpClientFactory.CreateClient(ServicoEolConstants.SERVICO);

            var url = string.Format(ServicoEolConstants.URL_ACESSOS_PERMISSOES_GRUPOS,
                                        options.GrupoSituacao,
                                        options.SistemaId,
                                        options.ModuloId);

            var response = await httpClient.GetAsync(url, cancellationToken);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var perfisDto = JsonConvert.DeserializeObject<List<PerfilDto>>(json);
            if (perfisDto == null)
                return null;

            var perfilDto = perfisDto.FirstOrDefault(p => p.Codigo == id);
            if (perfilDto == null)
                return null;

            var perfilInfo = CombinarPerfilComConfiguracao(perfilDto);

            if (perfilInfo != null)
            {
                await repositorioCache.SalvarRedisAsync(
                    cacheKey,
                    perfilInfo,
                    CACHE_TTL_MINUTOS);
            }

            return perfilInfo;

        }

        private PerfilInfo? CombinarPerfilComConfiguracao(PerfilDto perfilDto)
        {
            var idString = perfilDto.Codigo.ToString().ToUpper();

            //Buscar No banco
            if (!options.ConfiguracaoPerfis.TryGetValue(idString, out var config))
                return null;

            // Combina dados da API (permissões) com configuração local (regras)
            // Atualizar o perfil no banco
            return new PerfilInfo
            {
                Codigo = perfilDto.Codigo,
                Nome = perfilDto.Nome,
                PermiteConsultar = perfilDto.PermiteConsultar,
                PermiteInserir = perfilDto.PermiteInserir,
                PermiteAlterar = perfilDto.PermiteAlterar,
                PermiteExcluir = perfilDto.PermiteExcluir,
                TipoValidacao = config.TipoValidacao,
                ConsultarAbrangencia = config.ConsultarAbrangencia,
                AcessoIrrestrito = config.AcessoIrrestrito
            };
        }
    }
}
