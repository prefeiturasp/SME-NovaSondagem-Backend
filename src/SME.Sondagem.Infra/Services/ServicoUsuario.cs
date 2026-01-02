using Microsoft.AspNetCore.Http;
using SME.Sondagem.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SME.Sondagem.Infrastructure.Services
{
    public class ServicoUsuario : IServicoUsuario
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServicoUsuario(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string ObterUsuarioLogado()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value
                ?? "Sistema";
        }

        public string ObterRFUsuarioLogado()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("RF")?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("rf")?.Value
                ?? "0";
        }
    }
}