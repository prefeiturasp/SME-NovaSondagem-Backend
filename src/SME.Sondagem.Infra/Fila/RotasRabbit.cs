namespace SME.Sondagem.Infra.Fila; 

public class RotasRabbit
{
    public static string RotaLogs => "ApplicationLog";
    public static string Log => "ApplicationLog";

    public const string IniciarSync = "Sondagem.iniciar.sync";
}