using System.Net;

namespace SME.Sondagem.Dominio;

public class RegraNegocioException : Exception
{
    public RegraNegocioException(string mensagem, int statusCode = 601) : base(mensagem)
    {
        StatusCode = statusCode;
    }

    public RegraNegocioException(string mensagem, HttpStatusCode statusCode) : base(mensagem)
    {
        StatusCode = (int)statusCode;
    }

    public RegraNegocioException(string mensagem, Exception innerException) : base(mensagem, innerException)
    { }

    public int StatusCode { get; }
}