using SME.Sondagem.Aplicacao.Interfaces.ParametroSondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.ParametroSondagem;

public class AtualizarParametroSondagemUseCase : IAtualizarParametroSondagemUseCase
{
    private readonly IRepositorioParametroSondagem parametroSondagemRepositorio;

    public AtualizarParametroSondagemUseCase(IRepositorioParametroSondagem parametroSondagemRepositorio)
    {
        this.parametroSondagemRepositorio = parametroSondagemRepositorio;
    }

    public async Task<ParametroSondagemDto?> ExecutarAsync(long id, ParametroSondagemDto parametroSondagemDto, CancellationToken cancellationToken = default)
    {
        var p = await parametroSondagemRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (p == null)
            return null;

        p.Atualizar(parametroSondagemDto.Ativo, parametroSondagemDto.Descricao, parametroSondagemDto.Nome, parametroSondagemDto.Tipo);

        var sucessoId = await parametroSondagemRepositorio.SalvarAsync(p, cancellationToken: cancellationToken);
        
        if (sucessoId == 0)
            return null;

        return new ParametroSondagemDto
        {
            Id = p.Id,
            Nome = p.Nome,
            Descricao = p.Descricao,
            Ativo = p.Ativo,
            Tipo = p.Tipo
        };
    }
}