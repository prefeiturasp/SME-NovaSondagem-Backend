using SME.Sondagem.Infra.Extensions;

namespace SME.Sondagem.Infra.Fila;

public class MensagemRabbit
{
    public MensagemRabbit(object mensagem, Guid codigoCorrelacao)
    {
        Mensagem = mensagem;
        CodigoCorrelacao = codigoCorrelacao;
    }

    public MensagemRabbit(string action, string rotaErro, string usuarioLogadoRF, object mensagem, Guid codigoCorrelacao)
    {
        Action = action;
        RotaErro = rotaErro;
        UsuarioLogadoRF = usuarioLogadoRF;
        Mensagem = mensagem;
        CodigoCorrelacao = codigoCorrelacao;
    }

    public string Action { get; set; }
    public string RotaErro { get; set; }
    public string UsuarioLogadoRF { get; set; }
    public object Mensagem { get; set; }
    public Guid CodigoCorrelacao { get; set; }

    public T ObterObjetoMensagem<T>() where T : class
    {
        return Mensagem?.ToString()?.ConverterObjectStringPraObjeto<T>()!;
    }
}