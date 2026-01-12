namespace SME.Sondagem.Infra.Policies;

public class PoliticaPolly
{
    protected PoliticaPolly() { }
    public static string PublicaFila => "RetryPolicyFilasRabbit";
}