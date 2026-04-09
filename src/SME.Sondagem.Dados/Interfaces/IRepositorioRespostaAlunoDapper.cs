using SME.Sondagem.Infrastructure.Dtos.Integracao;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioRespostaAlunoDapper
{
    Task AtualizarCamposAsync(ContextoRespostaAlunoDto contexto);
}
