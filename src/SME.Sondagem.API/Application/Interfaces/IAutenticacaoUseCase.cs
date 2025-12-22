using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Infrastructure.Dtos.Autenticacao;

namespace SME.Sondagem.API.Application.Interfaces
{
    public interface IAutenticacaoUseCase
    {
        Task<TokenSondagemDto> Autenticar(string tokenSgp);
    }
}
