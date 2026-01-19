using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SME.Sondagem.API.Middlewares
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ChaveIntegracaoApi : Attribute, IAsyncActionFilter
    {
        private const string ChaveIntegracaoHeader = "x-api-sondagem-key";
        private const string ChaveIntegracaoConfigurationKey = "ChaveIntegracaoApiSondagem";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
            string chaveApi = configuration?[ChaveIntegracaoConfigurationKey]!;

            if (!context.HttpContext.Request.Headers.TryGetValue(ChaveIntegracaoHeader, out var chaveRecebida) ||
                string.IsNullOrEmpty(chaveRecebida) ||
                !chaveRecebida.ToString().Equals(chaveApi, StringComparison.Ordinal))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}
