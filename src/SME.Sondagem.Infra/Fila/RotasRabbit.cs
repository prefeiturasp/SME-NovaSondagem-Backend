namespace SME.Sondagem.Infra.Fila; 

public class RotasRabbit
{
    protected RotasRabbit() { }

    public static string RotaLogs => "ApplicationLog";
    public static string Log => "ApplicationLog";

    public const string RelatorioSondagemPorTurma = "sr.sondagem.relatorios.solicitados.sondagem.por.turma";
    public const string RelatorioSondagemConsolidadoPorRaca = "sr.sondagem.relatorios.solicitados.consolidado.por.raca";
    public const string RelatorioSondagemConsolidadoPorGenero = "sr.sondagem.relatorios.solicitados.consolidado.por.genero";
    public const string RelatorioSondagemConsolidadoPorRacaGenero = "sr.sondagem.relatorios.solicitados.consolidado.por.raca.genero";
    public const string RelatorioSondagemConsolidadoPorAno = "sr.sondagem.relatorios.solicitados.consolidado.por.ano";
    public const string RelatorioSondagemConsolidadoPorBimestre = "sr.sondagem.relatorios.solicitados.consolidado.por.bimestre";
}
