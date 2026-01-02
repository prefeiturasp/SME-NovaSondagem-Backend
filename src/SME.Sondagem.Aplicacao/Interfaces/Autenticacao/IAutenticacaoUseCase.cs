using SME.Sondagem.Infrastructure.Dtos.Autenticacao;

namespace SME.Sondagem.Aplicacao.Interfaces.Autenticacao;

public interface IAutenticacaoUseCase
{
    Task<TokenSondagemDto> Autenticar(string tokenSgp);
}
