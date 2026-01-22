using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using System.Net;

namespace SME.Sondagem.Aplicacao.Services.EOL
{
    public class ControleAcessoService : IControleAcessoService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IHttpContextAccessor httpContextAccessor;

        public readonly static Guid PERFIL_PROFESSOR = Guid.Parse("40E1E074-37D6-E911-ABD6-F81654FE895D");

        private static int COMPONENTE_CURRICULAR_REG_CLASSE_EJA_ETAPA_ALFAB = 1113;
        private static int COMPONENTE_CURRICULAR_REG_CLASSE_SP_INTEGRAL_1A5_ANOS = 1213;

        private static readonly HashSet<int> COMPONENTE_CURRICULARES_PERMITIDOS = new()
        {
            COMPONENTE_CURRICULAR_REG_CLASSE_EJA_ETAPA_ALFAB,
            COMPONENTE_CURRICULAR_REG_CLASSE_SP_INTEGRAL_1A5_ANOS
        };

        public ControleAcessoService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            this.httpClientFactory = httpClientFactory;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> ValidarPermissaoAcessoAsync(
            CancellationToken cancellationToken = default)
        {            
            var usuario = httpContextAccessor.HttpContext?.User;

            if (usuario == null || !usuario.Identity!.IsAuthenticated)
                return false;

            var login = usuario.FindFirst("rf")?.Value;
            var perfil = usuario.FindFirst("perfil")?.Value;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(perfil) 
                                            || !Guid.TryParse(perfil, out var perfilGuid) 
                                            || perfilGuid != PERFIL_PROFESSOR)
                return false;


            var httpClient = httpClientFactory.CreateClient(ServicoEolConstants.SERVICO);

            var url = string.Format(ServicoEolConstants.URL_COMPONENTES_CURRICULARES_FUNCIONARIOS, login, perfil);

            var response = await httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
                return false;

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(json))
                return false;

            var resultado = JsonConvert.DeserializeObject<IEnumerable<ControleAcessoDto>>(json);
            var retorno = resultado?.Any(r => COMPONENTE_CURRICULARES_PERMITIDOS.Contains(r.Codigo)) ?? false;

            return retorno;
        }
    }
}
