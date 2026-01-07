using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SME.Sondagem.Infra.Contexto;
using System.Security.Claims;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Contexto;

public class ContextoHttpTeste
{
    [Fact]
    public void DeveCriarContextoHttpComVariaveisBasicas()
    {
        var httpContextAccessor = CriarHttpContextAccessor();

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.NotNull(contexto);
        Assert.NotNull(contexto.Variaveis);
        Assert.True(contexto.Variaveis.ContainsKey("RF"));
        Assert.True(contexto.Variaveis.ContainsKey("Claims"));
        Assert.True(contexto.Variaveis.ContainsKey("login"));
        Assert.True(contexto.Variaveis.ContainsKey("NumeroPagina"));
        Assert.True(contexto.Variaveis.ContainsKey("NumeroRegistros"));
        Assert.True(contexto.Variaveis.ContainsKey("UsuarioLogado"));
        Assert.True(contexto.Variaveis.ContainsKey("NomeUsuario"));
        Assert.True(contexto.Variaveis.ContainsKey("Administrador"));
        Assert.True(contexto.Variaveis.ContainsKey("NomeAdministrador"));
        Assert.True(contexto.Variaveis.ContainsKey("PerfilUsuario"));
        Assert.True(contexto.Variaveis.ContainsKey("TemAuthorizationHeader"));
        Assert.True(contexto.Variaveis.ContainsKey("TokenAtual"));
    }

    [Fact]
    public void DeveCapturarRFDoUsuario()
    {
        var claims = new List<Claim>
        {
            new Claim("RF", "1234567")
        };
        var httpContextAccessor = CriarHttpContextAccessor(claims);

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("1234567", contexto.Variaveis["RF"]);
    }

    [Fact]
    public void DeveRetornarRFZeroQuandoNaoHouverClaim()
    {
        var httpContextAccessor = CriarHttpContextAccessor();

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("0", contexto.Variaveis["RF"]);
    }

    [Fact]
    public void DeveCapturarLoginDoUsuario()
    {
        var claims = new List<Claim>
        {
            new Claim("login", "usuario.teste")
        };
        var httpContextAccessor = CriarHttpContextAccessor(claims);

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("usuario.teste", contexto.Variaveis["login"]);
    }

    [Fact]
    public void DeveRetornarLoginVazioQuandoNaoHouverClaim()
    {
        var httpContextAccessor = CriarHttpContextAccessor();

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal(string.Empty, contexto.Variaveis["login"]);
    }

    [Fact]
    public void DeveCapturarNumeroPaginaDaQuery()
    {
        var query = new Dictionary<string, StringValues>
        {
            { "NumeroPagina", new StringValues("5") }
        };
        var httpContextAccessor = CriarHttpContextAccessor(queryString: query);

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("5", contexto.Variaveis["NumeroPagina"]);
    }

    [Fact]
    public void DeveRetornarNumeroPaginaZeroQuandoNaoHouverQuery()
    {
        var httpContextAccessor = CriarHttpContextAccessor();

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("0", contexto.Variaveis["NumeroPagina"]);
    }

    [Fact]
    public void DeveCapturarNumeroRegistrosDaQuery()
    {
        var query = new Dictionary<string, StringValues>
        {
            { "NumeroRegistros", new StringValues("10") }
        };
        var httpContextAccessor = CriarHttpContextAccessor(queryString: query);

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("10", contexto.Variaveis["NumeroRegistros"]);
    }

    [Fact]
    public void DeveRetornarNumeroRegistrosZeroQuandoNaoHouverQuery()
    {
        var httpContextAccessor = CriarHttpContextAccessor();

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("0", contexto.Variaveis["NumeroRegistros"]);
    }

    [Fact]
    public void DeveCapturarUsuarioLogado()
    {
        var httpContextAccessor = CriarHttpContextAccessor(identityName: "usuario.teste");

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("usuario.teste", contexto.Variaveis["UsuarioLogado"]);
    }

    [Fact]
    public void DeveRetornarSistemaQuandoNaoHouverUsuarioLogado()
    {
        var httpContextAccessor = CriarHttpContextAccessor();

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("Sistema", contexto.Variaveis["UsuarioLogado"]);
    }

    [Fact]
    public void DeveCapturarNomeUsuario()
    {
        var claims = new List<Claim>
        {
            new Claim("Nome", "Usu�rio Teste")
        };
        var httpContextAccessor = CriarHttpContextAccessor(claims);

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("Usu�rio Teste", contexto.Variaveis["NomeUsuario"]);
    }

    [Fact]
    public void DeveRetornarSistemaQuandoNaoHouverNomeUsuario()
    {
        var httpContextAccessor = CriarHttpContextAccessor();

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("Sistema", contexto.Variaveis["NomeUsuario"]);
    }

    [Fact]
    public void DeveCapturarAdministrador()
    {
        var claims = new List<Claim>
        {
            new Claim("login_adm_suporte", "admin.teste")
        };
        var httpContextAccessor = CriarHttpContextAccessor(claims);

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("admin.teste", contexto.Variaveis["Administrador"]);
    }

    [Fact]
    public void DeveRetornarAdministradorVazioQuandoNaoHouverClaim()
    {
        var httpContextAccessor = CriarHttpContextAccessor();

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal(string.Empty, contexto.Variaveis["Administrador"]);
    }

    [Fact]
    public void DeveCapturarNomeAdministrador()
    {
        var claims = new List<Claim>
        {
            new Claim("nome_adm_suporte", "Administrador Teste")
        };
        var httpContextAccessor = CriarHttpContextAccessor(claims);

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("Administrador Teste", contexto.Variaveis["NomeAdministrador"]);
    }

    [Fact]
    public void DeveRetornarNomeAdministradorVazioQuandoNaoHouverClaim()
    {
        var httpContextAccessor = CriarHttpContextAccessor();

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal(string.Empty, contexto.Variaveis["NomeAdministrador"]);
    }

    [Fact]
    public void DeveCapturarPerfilUsuario()
    {
        var claims = new List<Claim>
        {
            new Claim("perfil", "Professor")
        };
        var httpContextAccessor = CriarHttpContextAccessor(claims);

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("Professor", contexto.Variaveis["PerfilUsuario"]);
    }

    [Fact]
    public void DeveCapturarPerfilUsuarioComPerfilEmMaiuscula()
    {
        var claims = new List<Claim>
        {
            new Claim("Perfil", "Coordenador")
        };
        var httpContextAccessor = CriarHttpContextAccessor(claims);

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("Coordenador", contexto.Variaveis["PerfilUsuario"]);
    }

    [Fact]
    public void DeveRetornarPerfilUsuarioNuloQuandoNaoHouverClaim()
    {
        var httpContextAccessor = CriarHttpContextAccessor();

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.True(string.IsNullOrEmpty(contexto.Variaveis["PerfilUsuario"]?.ToString()));
    }

    [Fact]
    public void DeveCapturarTokenQuandoHouverAuthorizationHeader()
    {
        var headers = new Dictionary<string, StringValues>
        {
            { "authorization", new StringValues("Bearer abc123xyz456") }
        };
        var httpContextAccessor = CriarHttpContextAccessor(headers: headers);

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal(true, contexto.Variaveis["TemAuthorizationHeader"]);
        Assert.Equal("abc123xyz456", contexto.Variaveis["TokenAtual"]);
    }

    [Fact]
    public void DeveRetornarFalsoQuandoNaoHouverAuthorizationHeader()
    {
        var httpContextAccessor = CriarHttpContextAccessor();

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal(false, contexto.Variaveis["TemAuthorizationHeader"]);
        Assert.Equal(string.Empty, contexto.Variaveis["TokenAtual"]);
    }

    [Fact]
    public void DeveRetornarFalsoQuandoAuthorizationHeaderForVazio()
    {
        var headers = new Dictionary<string, StringValues>
        {
            { "authorization", StringValues.Empty }
        };
        var httpContextAccessor = CriarHttpContextAccessor(headers: headers);

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal(false, contexto.Variaveis["TemAuthorizationHeader"]);
        Assert.Equal(string.Empty, contexto.Variaveis["TokenAtual"]);
    }

    [Fact]
    public void DeveCapturarClaimsDoUsuario()
    {
        var claims = new List<Claim>
        {
            new Claim("RF", "1234567"),
            new Claim("Nome", "Usu�rio Teste"),
            new Claim("perfil", "Professor")
        };
        var httpContextAccessor = CriarHttpContextAccessor(claims);

        var contexto = new ContextoHttp(httpContextAccessor);

        var internalClaims = contexto.Variaveis["Claims"] as IEnumerable<InternalClaim>;
        Assert.NotNull(internalClaims);
        Assert.Equal(3, internalClaims.Count());
        Assert.Contains(internalClaims, c => c.Type == "RF" && c.Value == "1234567");
        Assert.Contains(internalClaims, c => c.Type == "Nome" && c.Value == "Usu�rio Teste");
        Assert.Contains(internalClaims, c => c.Type == "perfil" && c.Value == "Professor");
    }

    [Fact]
    public void DeveRetornarClaimsVazioQuandoNaoHouverUsuario()
    {
        var httpContextAccessor = CriarHttpContextAccessor();

        var contexto = new ContextoHttp(httpContextAccessor);

        var internalClaims = contexto.Variaveis["Claims"] as IEnumerable<InternalClaim>;
        Assert.NotNull(internalClaims);
        Assert.Empty(internalClaims);
    }

    [Fact]
    public void DeveLancarExcecaoAoTentarAtribuirContexto()
    {
        var httpContextAccessor = CriarHttpContextAccessor();
        var contexto = new ContextoHttp(httpContextAccessor);
        var novoContexto = new ContextoHttp(httpContextAccessor);

        var exception = Assert.Throws<NotImplementedException>(() => contexto.AtribuirContexto(novoContexto));
        Assert.Equal("Este tipo de conexto não permite atribuição", exception.Message);
    }

    [Fact]
    public void DeveAdicionarVariaveis()
    {
        var httpContextAccessor = CriarHttpContextAccessor();
        var contexto = new ContextoHttp(httpContextAccessor);
        var novasVariaveis = new Dictionary<string, object>
        {
            { "Variavel1", "Valor1" },
            { "Variavel2", 123 }
        };

        contexto.AdicionarVariaveis(novasVariaveis);

        Assert.Equal(novasVariaveis, contexto.Variaveis);
        Assert.Equal("Valor1", contexto.Variaveis["Variavel1"]);
        Assert.Equal(123, contexto.Variaveis["Variavel2"]);
    }

    [Fact]
    public void DeveCapturarTodasAsVariaveisEmCenarioCompleto()
    {
        var claims = new List<Claim>
        {
            new Claim("RF", "9876543"),
            new Claim("login", "professor.teste"),
            new Claim("Nome", "Professor Teste"),
            new Claim("login_adm_suporte", "admin.teste"),
            new Claim("nome_adm_suporte", "Admin Teste"),
            new Claim("Perfil", "Professor")
        };
        var query = new Dictionary<string, StringValues>
        {
            { "NumeroPagina", new StringValues("3") },
            { "NumeroRegistros", new StringValues("25") }
        };
        var headers = new Dictionary<string, StringValues>
        {
            { "authorization", new StringValues("Bearer token123456") }
        };
        var httpContextAccessor = CriarHttpContextAccessor(claims, "professor.teste", query, headers);

        var contexto = new ContextoHttp(httpContextAccessor);

        Assert.Equal("9876543", contexto.Variaveis["RF"]);
        Assert.Equal("professor.teste", contexto.Variaveis["login"]);
        Assert.Equal("3", contexto.Variaveis["NumeroPagina"]);
        Assert.Equal("25", contexto.Variaveis["NumeroRegistros"]);
        Assert.Equal("professor.teste", contexto.Variaveis["UsuarioLogado"]);
        Assert.Equal("Professor Teste", contexto.Variaveis["NomeUsuario"]);
        Assert.Equal("admin.teste", contexto.Variaveis["Administrador"]);
        Assert.Equal("Admin Teste", contexto.Variaveis["NomeAdministrador"]);
        Assert.Equal("Professor", contexto.Variaveis["PerfilUsuario"]);
        Assert.Equal(true, contexto.Variaveis["TemAuthorizationHeader"]);
        Assert.Equal("token123456", contexto.Variaveis["TokenAtual"]);

        var internalClaims = contexto.Variaveis["Claims"] as IEnumerable<InternalClaim>;
        Assert.NotNull(internalClaims);
        Assert.Equal(7, internalClaims.Count());
    }

    private IHttpContextAccessor CriarHttpContextAccessor(
        List<Claim>? claims = null,
        string? identityName = null,
        Dictionary<string, StringValues>? queryString = null,
        Dictionary<string, StringValues>? headers = null)
    {
        var httpContext = new DefaultHttpContext();

        if (claims != null || identityName != null)
        {
            var claimsToUse = claims ?? new List<Claim>();

            if (identityName != null)
            {
                claimsToUse.Add(new Claim(ClaimTypes.Name, identityName));
            }

            var identity = new ClaimsIdentity(claimsToUse, "TestAuthType");
            httpContext.User = new ClaimsPrincipal(identity);
        }

        if (queryString != null)
        {
            httpContext.Request.Query = new QueryCollection(queryString);
        }

        if (headers != null)
        {
            foreach (var header in headers)
            {
                httpContext.Request.Headers[header.Key] = header.Value;
            }
        }

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        return httpContextAccessor;
    }
}