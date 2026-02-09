namespace SME.Sondagem.Infra.Fila;

public static class ExchangeRabbit
{
    public static string Sondagem => "Sondagem.workers";
    public static string SondagemEstudante => "Sondagem.estudante.workers";
    public static string SondagemDeadLetter => "Sondagem.workers.deadletter";
    public static string Logs => "EnterpriseApplicationLog";
    public static int SondagemDeadLetterTtl => 10 * 60 * 1000; /*10 Min * 60 Seg * 1000 milisegundos = 10 minutos em milisegundos*/
    public static int SondagemDeadLetterTtl_3 => 3 * 60 * 1000; /*10 Min * 60 Seg * 1000 milisegundos = 10 minutos em milisegundos*/
}