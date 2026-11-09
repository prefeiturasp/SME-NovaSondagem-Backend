using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dominio;

public static class SituacaoMatriculaAlunoExtension
{
    public static bool EhCursandoTurma(this int codigoSituacaoMatricula)
        => codigoSituacaoMatricula is (int)SituacaoMatriculaAluno.Ativo
                                   or (int)SituacaoMatriculaAluno.Rematriculado;

    public static bool EhListadoNaTurma(this int codigoSituacaoMatricula)
        => codigoSituacaoMatricula.EhCursandoTurma()
           || codigoSituacaoMatricula == (int)SituacaoMatriculaAluno.RemanejadoSaida;
}
