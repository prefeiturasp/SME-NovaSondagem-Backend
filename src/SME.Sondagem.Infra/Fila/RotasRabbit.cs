namespace SME.Sondagem.Infra.Fila; 

public class RotasRabbit
{
    protected RotasRabbit() { }

    public static string RotaLogs => "ApplicationLog";
    public static string Log => "ApplicationLog";

    public const string IniciarSync = "Sondagem.iniciar.sync";
    public const string RelatorioSondagemPorTurmaAction = "relatorios/sondagem-questionario";
    public const string RelatorioSondagemPorTurma = "sr.relatorios.solicitados.sondagem.por.turma";
    public const string RelatorioSondagemPorTurmaError = "sgp.relatorios.erro.notificar.sondagemquestionario";
}