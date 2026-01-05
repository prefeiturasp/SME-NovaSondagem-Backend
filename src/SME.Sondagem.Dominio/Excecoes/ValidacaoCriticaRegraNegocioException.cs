using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dominio;

[ExcludeFromCodeCoverage]
public class ValidacaoCriticaRegraNegocioException : Exception
{
    public ValidacaoCriticaRegraNegocioException(string mensagem) : base(mensagem)
    {
    }
}