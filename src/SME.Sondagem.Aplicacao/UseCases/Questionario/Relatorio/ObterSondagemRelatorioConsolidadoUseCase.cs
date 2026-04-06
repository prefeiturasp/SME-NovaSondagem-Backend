using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoUseCase : IObterSondagemRelatorioConsolidadoUseCase
{
    private readonly IRepositorioElasticTurma _repositorioElasticTurma;
    private readonly IRepositorioSondagem _repositorioSondagem;

    public ObterSondagemRelatorioConsolidadoUseCase(IRepositorioElasticTurma repositorioElasticTurma, IRepositorioSondagem repositorioSondagem)
    {
        _repositorioElasticTurma = repositorioElasticTurma;
        _repositorioSondagem = repositorioSondagem;
    }

    public async Task<QuestionarioSondagemRelatorioDto> ObterSondagemRelatorio([FromQuery] FiltroConsolidadoDto filtro, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
