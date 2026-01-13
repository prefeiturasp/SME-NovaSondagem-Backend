namespace SME.Sondagem.Dominio;

public class ErroNaoEncontradoException : Exception
{
    public ErroNaoEncontradoException(string mensagem)
    : base(mensagem) { }

    public ErroNaoEncontradoException(string mensagem, Exception innerException)
        : base(mensagem, innerException) { }
}
