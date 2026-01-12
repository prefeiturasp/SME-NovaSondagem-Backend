using SME.Sondagem.Infra.Fila;

namespace SME.Sondagem.Infra.Interfaces;

public interface IServicoMensageria
{
    Task<bool> Publicar(MensagemRabbit mensagemRabbit, string rota, string exchange, string nomeAcao);
}