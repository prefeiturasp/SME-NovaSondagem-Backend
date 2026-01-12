namespace SME.Sondagem.Infrastructure.Dtos.Sondagem
{
    public class RespostaSondagemDto
    {
        //bimestreId
        //questaoId
        public int BimestreId { get; set; }  // 1° ciclo, 2° ciclo...
        public int QuestaoId { get; set; }     // questao_id
        public int OpcaoRepostaId { get; set; }  // opcao_resposta_id
    }
}
