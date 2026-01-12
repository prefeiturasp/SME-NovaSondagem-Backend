using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.Sondagem.Infra.Exceptions;

[ExcludeFromCodeCoverage]
public class ErroException : Exception
{
    public ErroException(string mensagem, int statusCode = 500) : base(mensagem)
    {
        StatusCode = statusCode;
    }

    public ErroException(string mensagem, HttpStatusCode statusCode) : base(mensagem)
    {
        StatusCode = (int)statusCode;
    }

    public int StatusCode { get; }
}