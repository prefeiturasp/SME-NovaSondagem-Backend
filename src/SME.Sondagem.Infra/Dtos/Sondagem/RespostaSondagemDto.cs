namespace SME.Sondagem.Infrastructure.Dtos.Sondagem
{
    public class RespostaSondagemDto
    {
        //bimestreId
        //questaoId
        public string NomeColuna { get; set; } = null!;  // 1° ciclo, 2° ciclo...
        public int RespostaId { get; set; }     // questao_id
        public int RespostaSelecionada { get; set; }  // opcao_resposta_id
    }
}
