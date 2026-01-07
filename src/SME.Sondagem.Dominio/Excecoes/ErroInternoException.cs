namespace SME.Sondagem.Dominio;

public class ErroInternoException : Exception
{
    public ErroInternoException(string mensagem)
    : base(mensagem) { }

    public ErroInternoException(string mensagem, Exception innerException)
        : base(mensagem, innerException) { }
}
