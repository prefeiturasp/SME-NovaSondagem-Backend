using SME.Sondagem.Aplicacao.Interfaces.ParametroSondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.ParametroSondagem;

public class CriarParametroSondagemUseCase : ICriarParametroSondagemUseCase
{
    private readonly IRepositorioParametroSondagem parametroSondagemRepositorio;

    public CriarParametroSondagemUseCase(IRepositorioParametroSondagem parametroSondagemRepositorio)
    {
        this.parametroSondagemRepositorio = parametroSondagemRepositorio;
    }

    public async Task<long> ExecutarAsync(ParametroSondagemDto parametroSondagemDto, CancellationToken cancellationToken = default)
    {
        var parametroSondagem = new SME.Sondagem.Dominio.Entidades.ParametroSondagem();
        parametroSondagem.Ativo = parametroSondagemDto.Ativo;
        parametroSondagem.Descricao = parametroSondagemDto.Descricao;
        parametroSondagem.Nome = parametroSondagemDto.Nome;
        parametroSondagem.Tipo = parametroSondagemDto.Tipo;

        return await parametroSondagemRepositorio.SalvarAsync(parametroSondagem, cancellationToken: cancellationToken);
    }
}