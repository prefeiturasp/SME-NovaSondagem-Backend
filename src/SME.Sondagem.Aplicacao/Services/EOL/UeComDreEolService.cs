using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace SME.Sondagem.Aplicacao.Services.EOL
{
    [ExcludeFromCodeCoverage]
    public class UeComDreEolService : IUeComDreEolService
    {
        private const int CacheTtlMinutos = 15;

        private readonly IHttpClientFactory httpClientFactory;
        private readonly IServicoLog _servicoLog;
        private readonly IRepositorioCache _repositorioCache;

        public UeComDreEolService(IHttpClientFactory httpClientFactory, IServicoLog servicoLog, IRepositorioCache repositorioCache)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _servicoLog = servicoLog ?? throw new ArgumentNullException(nameof(servicoLog));
            _repositorioCache = repositorioCache ?? throw new ArgumentNullException(nameof(repositorioCache));
        }

        public async Task<IEnumerable<UeComDreEolDto>> ObterUesComDrePorCodigosUes(IEnumerable<string> codigosUes, CancellationToken cancellationToken = default)
        {
            try
            {
                var resultado = new List<UeComDreEolDto>();

                if (!codigosUes.Any())
                    return resultado;

                var codigosOrdenados = codigosUes.OrderBy(x => x).ToList();
                var codigosHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join(",", codigosOrdenados))))[..16];
                var chave = $"sondagem-ues-com-dre:{codigosHash}";

                var cached = await _repositorioCache.ObterRedisAsync<List<UeComDreEolDto>>(chave);
                if (cached != null) return cached;

                var httpClient = httpClientFactory.CreateClient(ServicoEolConstants.SERVICO);
                var jsonParaPost = new StringContent(JsonConvert.SerializeObject(codigosUes), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(ServicoEolConstants.URL_OBTER_UE_COM_DRE, jsonParaPost, cancellationToken);

                if (!response.IsSuccessStatusCode) return resultado;
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                resultado = JsonConvert.DeserializeObject<List<UeComDreEolDto>>(json) ?? resultado;
                await _repositorioCache.SalvarRedisAsync(chave, resultado, CacheTtlMinutos);
                return resultado;
            }
            catch (Exception e)
            {
                _servicoLog.Registrar($"Erro ao executar UeComDreEolService", e);
                return [];

            }
        }
    }
}
