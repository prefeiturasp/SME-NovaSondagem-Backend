namespace SME.Sondagem.Infra.Fila; 

public class RotasRabbit
{
    protected RotasRabbit() { }

    public static string RotaLogs => "ApplicationLog";
    public static string Log => "ApplicationLog";

    public const string RelatorioSondagemPorTurma = "sr.sondagem.relatorios.solicitados.sondagem.por.turma";
}