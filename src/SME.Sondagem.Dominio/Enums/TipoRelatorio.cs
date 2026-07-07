using System.ComponentModel.DataAnnotations;

namespace SME.Sondagem.Dominio.Enums;

public enum TipoRelatorio
{
    [Display(Name = "relatorios/sondagem-por-turma", ShortName = "RelatorioSondagemPorTurma", Description = "Relatório de sondagem por turma")]
    SondagemPorTurma = 58,

    [Display(Name = "relatorios/consolidado-por-raca", ShortName = "RelatorioSondagemConsolidadoPorRaca", Description = "Relatório de sondagem consolidado por raça")]
    ConsolidadoPorRaca = 59,

    [Display(Name = "relatorios/consolidado-por-genero", ShortName = "RelatorioSondagemConsolidadoPorGenero", Description = "Relatório de sondagem consolidado por gênero")]
    ConsolidadoPorGenero = 60,

    [Display(Name = "relatorios/consolidado-por-raca-genero", ShortName = "RelatorioSondagemConsolidadoPorRacaGenero", Description = "Relatório de sondagem consolidado por raça e gênero")]
    ConsolidadoPorRacaGenero = 61,

    [Display(Name = "relatorios/consolidado-por-ano", ShortName = "RelatorioSondagemConsolidadoPorAno", Description = "Relatório de sondagem consolidado por ano")]
    ConsolidadoPorAno = 62,

    [Display(Name = "relatorios/consolidado-por-bimestre", ShortName = "RelatorioSondagemConsolidadoPorBimestre", Description = "Relatório de sondagem consolidado por bimestre")]
    ConsolidadoPorBimestre = 63,
}
