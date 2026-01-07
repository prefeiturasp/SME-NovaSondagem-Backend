using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dominio.Entidades;

public class LogMensagem
{
    public LogMensagem(string mensagem, LogNivel nivel, string observacao, string? rastreamento = null, string? excecaoInterna = null, string projeto = "Serap-Prova-Worker")
    {
        Mensagem = mensagem;
        Nivel = nivel;
        Observacao = observacao;
        Projeto = projeto;
        Rastreamento = rastreamento ?? string.Empty;
        ExcecaoInterna = excecaoInterna ?? string.Empty;
        DataHora = DateTime.Now;
    }

    public string Mensagem { get; private set; }
    public LogNivel Nivel { get; private set; }
    public string Observacao { get; private set; }
    public string Projeto { get; private set; }
    public string Rastreamento { get; private set; }
    public string ExcecaoInterna { get; private set; }
    public DateTime DataHora { get; private set; }
}