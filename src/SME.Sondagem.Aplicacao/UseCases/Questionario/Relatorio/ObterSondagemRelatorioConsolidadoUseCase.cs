using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoUseCase : IObterSondagemRelatorioConsolidadoUseCase
{
    private readonly IRepositorioElasticTurma _repositorioElasticTurma;
    private readonly RepositoriosSondagem _repositorioSondagem;

    public ObterSondagemRelatorioConsolidadoUseCase(IRepositorioElasticTurma repositorioElasticTurma, RepositoriosSondagem repositorioSondagem)
    {
        _repositorioElasticTurma = repositorioElasticTurma;
        _repositorioSondagem = repositorioSondagem;
    }

    public async Task<QuestionarioSondagemRelatorioDto> ObterSondagemRelatorio([FromQuery] FiltroConsolidadoDto filtro, CancellationToken cancellationToken)
    {
        var teste = await _repositorioSondagem.RepositorioRespostaAluno.ObterRespostasComDependenciasAsync(filtro, cancellationToken);
        //var teste2 = teste.Where(x => x.Resposta != null).ToList();
        throw new NotImplementedException();
    }
}
