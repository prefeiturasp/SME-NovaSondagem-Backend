using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Constantes;
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

        public readonly static Guid PERFIL_PROFESSOR = Guid.Parse("40E1E074-37D6-E911-ABD6-F81654FE895D");

        public ControleAcessoService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IRepositorioCache repositorioCache)
        {
            this.httpClientFactory = httpClientFactory;
            this.httpContextAccessor = httpContextAccessor;
            this.repositorioCache = repositorioCache;
        }

        public async Task<bool> ValidarPermissaoAcessoAsync(string turmaId, CancellationToken cancellationToken = default)
        {         
            if (string.IsNullOrEmpty(turmaId))
                return false;            

            var resultado = await ObterControleAcessoUsuarioAutenticadoAsync(cancellationToken);

            var retorno = resultado?.Any(r => r.Regencia && r.TurmaCodigo == turmaId) ?? false;

            return retorno;
        }


        private async Task<IEnumerable<ControleAcessoDto>> ObterControleAcessoUsuarioAutenticadoAsync(CancellationToken cancellationToken)
        {
            var usuario = httpContextAccessor.HttpContext?.User;

            if (usuario == null || !usuario.Identity!.IsAuthenticated)
                return default!;

            var login = usuario.FindFirst("rf")?.Value;
            var perfil = usuario.FindFirst("perfil")?.Value;

            if(string.IsNullOrEmpty(login) || string.IsNullOrEmpty(perfil)
                                              || !Guid.TryParse(perfil, out var perfilGuid)
                                              || perfilGuid != PERFIL_PROFESSOR)
              return default!;

            var chave = string.Format(NomeChaveCache.CONTROLE_ACESSO_USUARIO, login, perfil);

            var cacheRedis = await repositorioCache.ObterRedisToJsonAsync(chave);

            if(!string.IsNullOrEmpty(cacheRedis))
                return JsonConvert.DeserializeObject<IEnumerable<ControleAcessoDto>>(cacheRedis)!;


            var httpClient = httpClientFactory.CreateClient(ServicoEolConstants.SERVICO);

            var url = string.Format(ServicoEolConstants.URL_COMPONENTES_CURRICULARES_FUNCIONARIOS, login, perfil);

            var response = await httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
                return default!;

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(json))
                return default!;

            var resultado = JsonConvert.DeserializeObject<IEnumerable<ControleAcessoDto>>(json);

            await repositorioCache.SalvarRedisToJsonAsync(chave, json);

            return resultado!;
        } 

    }
}
