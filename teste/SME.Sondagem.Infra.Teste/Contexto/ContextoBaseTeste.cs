using SME.Sondagem.Infra.Contexto;
using SME.Sondagem.Infra.Interfaces;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Contexto;

public class ContextoBaseTeste
{
    private class ContextoBaseConcreto : ContextoBase
    {
        public override void AdicionarVariaveis(IDictionary<string, object> variaveis)
        {
            foreach (var variavel in variaveis)
            {
                Variaveis[variavel.Key] = variavel.Value;
            }
        }

        public override IContextoAplicacao AtribuirContexto(IContextoAplicacao contexto)
        {
            if (contexto?.Variaveis != null)
            {
                foreach (var variavel in contexto.Variaveis)
                {
                    Variaveis[variavel.Key] = variavel.Value;
                }
            }
            return this;
        }
    }

    [Fact]
    public void Construtor_DeveInicializarDicionarioVariaveis()
    {
        var contexto = new ContextoBaseConcreto();

        Assert.NotNull(contexto.Variaveis);
        Assert.Empty(contexto.Variaveis);
    }

    [Fact]
    public void NomeUsuario_QuandoNaoDefinido_DeveRetornarSistema()
    {
        var contexto = new ContextoBaseConcreto();

        var nomeUsuario = contexto.NomeUsuario;

        Assert.Equal("Sistema", nomeUsuario);
    }

    [Fact]      
    public void NomeUsuario_QuandoDefinido_DeveRetornarValorDefinido()
    {
        var contexto = new ContextoBaseConcreto();
        contexto.Variaveis["NomeUsuario"] = "Joao Silva";

        var nomeUsuario = contexto.NomeUsuario;

        Assert.Equal("Joao Silva", nomeUsuario);
    }

    [Fact]
    public void UsuarioLogado_QuandoNaoDefinido_DeveRetornarSistema()
    {
        var contexto = new ContextoBaseConcreto();

        var usuarioLogado = contexto.UsuarioLogado;

        Assert.Equal("Sistema", usuarioLogado);
    }

    [Fact]
    public void UsuarioLogado_QuandoDefinido_DeveRetornarValorDefinido()
    {
        var contexto = new ContextoBaseConcreto();
        contexto.Variaveis["UsuarioLogado"] = "usuario@exemplo.com";

        var usuarioLogado = contexto.UsuarioLogado;

        Assert.Equal("usuario@exemplo.com", usuarioLogado);
    }

    [Fact]
    public void PerfilUsuario_QuandoNaoDefinido_DeveRetornarStringVazia()
    {
        var contexto = new ContextoBaseConcreto();

        var perfilUsuario = contexto.PerfilUsuario;

        Assert.Equal(string.Empty, perfilUsuario);
    }

    [Fact]
    public void PerfilUsuario_QuandoDefinido_DeveRetornarValorDefinido()
    {
        var contexto = new ContextoBaseConcreto();
        contexto.Variaveis["PerfilUsuario"] = "Administrador";

        var perfilUsuario = contexto.PerfilUsuario;

        Assert.Equal("Administrador", perfilUsuario);
    }

    [Fact]
    public void Administrador_QuandoNaoDefinido_DeveRetornarStringVazia()
    {
        var contexto = new ContextoBaseConcreto();

        var administrador = contexto.Administrador;

        Assert.Equal(string.Empty, administrador);
    }

    [Fact]
    public void Administrador_QuandoDefinido_DeveRetornarValorDefinido()
    {
        var contexto = new ContextoBaseConcreto();
        contexto.Variaveis["Administrador"] = "admin@exemplo.com";

        var administrador = contexto.Administrador;

        Assert.Equal("admin@exemplo.com", administrador);
    }

    [Fact]
    public void ObterVariavel_QuandoVariavelExiste_DeveRetornarValorCorreto()
    {
        var contexto = new ContextoBaseConcreto();
        contexto.Variaveis["Chave"] = "Valor";

        var resultado = contexto.ObterVariavel<string>("Chave");

        Assert.Equal("Valor", resultado);
    }

    [Fact]
    public void ObterVariavel_QuandoVariavelNaoExiste_DeveRetornarValorPadrao()
    {
        var contexto = new ContextoBaseConcreto();

        var resultado = contexto.ObterVariavel<string>("ChaveInexistente");

        Assert.Null(resultado);
    }

    [Fact]
    public void ObterVariavel_QuandoTipoInt_DeveRetornarValorCorreto()
    {
        var contexto = new ContextoBaseConcreto();
        contexto.Variaveis["Numero"] = 42;

        var resultado = contexto.ObterVariavel<int>("Numero");

        Assert.Equal(42, resultado);
    }

    [Fact]
    public void ObterVariavel_QuandoTipoIntNaoExiste_DeveRetornarZero()
    {
        var contexto = new ContextoBaseConcreto();

        var resultado = contexto.ObterVariavel<int>("NumeroInexistente");

        Assert.Equal(0, resultado);
    }

    [Fact]
    public void ObterVariavel_QuandoTipoBool_DeveRetornarValorCorreto()
    {
        var contexto = new ContextoBaseConcreto();
        contexto.Variaveis["Ativo"] = true;

        var resultado = contexto.ObterVariavel<bool>("Ativo");

        Assert.True(resultado);
    }

    [Fact]
    public void ObterVariavel_QuandoTipoBoolNaoExiste_DeveRetornarFalse()
    {
        var contexto = new ContextoBaseConcreto();

        var resultado = contexto.ObterVariavel<bool>("AtivoInexistente");

        Assert.False(resultado);
    }

    [Fact]
    public void Variaveis_DevePermitirAtribuicao()
    {
        var contexto = new ContextoBaseConcreto();
        var novosDicionario = new Dictionary<string, object>
        {
            { "Chave1", "Valor1" },
            { "Chave2", 123 }
        };

        contexto.Variaveis = novosDicionario;

        Assert.Equal(novosDicionario, contexto.Variaveis);
        Assert.Equal(2, contexto.Variaveis.Count);
    }

    [Fact]
    public void ObterVariavel_QuandoTipoComplexo_DeveRetornarObjetoCorreto()
    {
        var contexto = new ContextoBaseConcreto();
        var objetoComplexo = new { Nome = "Teste", Valor = 100 };
        contexto.Variaveis["ObjetoComplexo"] = objetoComplexo;

        var resultado = contexto.ObterVariavel<object>("ObjetoComplexo");

        Assert.NotNull(resultado);
        Assert.Equal(objetoComplexo, resultado);
    }
}