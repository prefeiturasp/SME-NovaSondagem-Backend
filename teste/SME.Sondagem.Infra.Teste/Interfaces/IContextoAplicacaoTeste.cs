using SME.Sondagem.Infra.Interfaces;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Interfaces;

public class IContextoAplicacaoTeste
{
    private class ContextoAplicacaoMock : IContextoAplicacao
    {
        public IDictionary<string, object> Variaveis { get; set; } = new Dictionary<string, object>();

        public string UsuarioLogado { get; set; } = "usuario";
        public string NomeUsuario { get; set; } = "Nome Usuário";
        public string PerfilUsuario { get; set; } = "Perfil";
        public string Administrador { get; set; } = "admin";

        public T ObterVariavel<T>(string nome)
        {
            if (Variaveis.TryGetValue(nome, out var valor) && valor is T tValor)
                return tValor;
            return default!;
        }

        public IContextoAplicacao AtribuirContexto(IContextoAplicacao contexto)
        {
            UsuarioLogado = contexto.UsuarioLogado;
            NomeUsuario = contexto.NomeUsuario;
            PerfilUsuario = contexto.PerfilUsuario;
            Administrador = contexto.Administrador;
            Variaveis = new Dictionary<string, object>(contexto.Variaveis);
            return this;
        }

        public void AdicionarVariaveis(IDictionary<string, object> variaveis)
        {
            foreach (var kvp in variaveis)
                Variaveis[kvp.Key] = kvp.Value;
        }
    }

    [Fact]
    public void Deve_AdicionarEObterVariaveis()
    {
        var contexto = new ContextoAplicacaoMock();
        contexto.AdicionarVariaveis(new Dictionary<string, object> { { "chave", 123 } });

        Assert.True(contexto.Variaveis.ContainsKey("chave"));
        Assert.Equal(123, contexto.ObterVariavel<int>("chave"));
    }

    [Fact]
    public void Deve_AtribuirContexto()
    {
        var contextoOrigem = new ContextoAplicacaoMock
        {
            UsuarioLogado = "user1",
            NomeUsuario = "Nome1",
            PerfilUsuario = "Perfil1",
            Administrador = "admin1",
            Variaveis = new Dictionary<string, object> { { "x", 42 } }
        };

        var contextoDestino = new ContextoAplicacaoMock();
        contextoDestino.AtribuirContexto(contextoOrigem);

        Assert.Equal("user1", contextoDestino.UsuarioLogado);
        Assert.Equal("Nome1", contextoDestino.NomeUsuario);
        Assert.Equal("Perfil1", contextoDestino.PerfilUsuario);
        Assert.Equal("admin1", contextoDestino.Administrador);
        Assert.True(contextoDestino.Variaveis.ContainsKey("x"));
        Assert.Equal(42, contextoDestino.ObterVariavel<int>("x"));
    }

    [Fact]
    public void Deve_RetornarDefault_QuandoVariavelNaoExiste()
    {
        var contexto = new ContextoAplicacaoMock();
        var valor = contexto.ObterVariavel<int>("inexistente");
        Assert.Equal(0, valor);
    }
}
