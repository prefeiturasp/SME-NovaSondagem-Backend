using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades.Configuration;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Services.EOL
{
    public class PerfilService : IPerfilService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRepositorioCache _repositorioCache;
        private readonly IRepositorioControleAcessoOptions _repositorioControleAcessoOptions;

        private const int CacheTtlMinutos = 60;
        private const string CacheKeyPrefix = "perfil-config:";

        public PerfilService(
            IHttpClientFactory httpClientFactory,
            IRepositorioCache repositorioCache,
            IRepositorioControleAcessoOptions repositorioControleAcessoOptions)
        {
            _httpClientFactory = httpClientFactory
                ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _repositorioCache = repositorioCache
                ?? throw new ArgumentNullException(nameof(repositorioCache));
            _repositorioControleAcessoOptions = repositorioControleAcessoOptions
                ?? throw new ArgumentNullException(nameof(repositorioControleAcessoOptions));
        }

        public async Task<PerfilInfoSondagemDto> ObterPerfilPorIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheKeyPrefix}{id}";

            var perfilCached = await _repositorioCache.ObterRedisAsync<PerfilInfoSondagemDto>(cacheKey);
            if (perfilCached != null)
                return perfilCached;

            var options = await ObterControleAcessoOptionsAsync();
            var perfisDto = await BuscarPerfisNaApiAsync(options, cancellationToken);

            if (perfisDto == null)
                return SemAcesso();

            var perfilDto = perfisDto.FirstOrDefault(p => p.Id == id);
            if (perfilDto == null)
                return SemAcesso();

            PerfilInfoSondagemDto perfilInfo = CombinarPerfilComConfiguracao(perfilDto, options);

            if (!string.IsNullOrEmpty(perfilInfo.Nome))
                await _repositorioCache.SalvarRedisAsync(cacheKey, perfilInfo, CacheTtlMinutos);

            return perfilInfo;
        }

        private async Task<List<PerfilInfoEolDto>?> BuscarPerfisNaApiAsync(
            ControleAcessoOptions options,
            CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient(ServicoEolConstants.SERVICO);

            var url = string.Format(
                ServicoEolConstants.URL_ACESSOS_PERMISSOES_GRUPOS,
                options.GrupoSituacao,
                options.SistemaId,
                options.ModuloId);

            var response = await httpClient.GetAsync(url, cancellationToken);
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonConvert.DeserializeObject<List<PerfilInfoEolDto>>(json);
        }

        private async Task<ControleAcessoOptions> ObterControleAcessoOptionsAsync()
        {
            var consulta = await _repositorioControleAcessoOptions.ObterTodosComPerfis();
            return consulta.FirstOrDefault()
                ?? throw new InvalidOperationException("Nenhuma configuração de controle de acesso encontrada.");
        }

        private static PerfilInfoSondagemDto CombinarPerfilComConfiguracao(
            PerfilInfoEolDto perfilDto,
            ControleAcessoOptions options)
        {
            var codigoPerfil = perfilDto.Id.ToString().ToUpper();

            var config = options.ConfiguracaoPerfis
                .FirstOrDefault(x => x.Codigo.ToString().ToUpper().Equals(codigoPerfil, StringComparison.CurrentCultureIgnoreCase));

            if (config == null)
                return SemAcesso();

            return new PerfilInfoSondagemDto
            {
                Codigo = perfilDto.Id,
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

        private static PerfilInfoSondagemDto SemAcesso()
        {
            return new PerfilInfoSondagemDto
            {
                Codigo = Guid.NewGuid(),
                Nome = string.Empty,
                PermiteConsultar = false,
                PermiteInserir = false,
                PermiteAlterar = false,
                PermiteExcluir = false,
                TipoValidacao = string.Empty,
                ConsultarAbrangencia = false,
                AcessoIrrestrito = false
            };
        }
    }
}