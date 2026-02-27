using System.ComponentModel.DataAnnotations;

namespace SME.Sondagem.Dominio.Enums;

public enum TipoRelatorio
{
    [Display(Name = "relatorios/sondagem-por-turma", ShortName = "RelatorioSondagemPorTurma", Description = "Relatório de sondagem por turma")]
    SondagemPorTurma = 58,
}
