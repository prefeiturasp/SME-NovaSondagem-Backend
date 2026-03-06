using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio
{
    public interface IObterSondagemRelatorioPorTodasTurmaUseCase
    {
        //public Task<MemoryStream> ObterSondagemRelatorio(CancellationToken cancellationToken);
        public Task ObterSondagemRelatorioTodas(CancellationToken cancellationToken);
    }
}
