using SME.Sondagem.Aplicacao.Interfaces.Turma;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.Turma
{
    public class ObterPermissaoTurmaUseCase : IObterPermissaoTurmaUseCase
    {
        private static readonly HashSet<int> SERIE_ANO_PERMITIDOS = new() { 1, 2, 3 };

        private readonly IRepositorioElasticTurma _repositorioElasticTurma;
        public ObterPermissaoTurmaUseCase(
            IRepositorioElasticTurma repositorioElasticTurma)
        {
            _repositorioElasticTurma = repositorioElasticTurma ?? throw new ArgumentNullException(nameof(repositorioElasticTurma));
        }

        public async Task<bool> ObterPermissaoTurma(int turmaId, CancellationToken cancellationToken)
        {
            var filtro = new FiltroQuestionario(){ TurmaId = turmaId };

            var turma = await _repositorioElasticTurma.ObterTurmaPorId(filtro, cancellationToken)
            ?? throw new RegraNegocioException(MensagemNegocioComuns.TURMA_NAO_LOCALIZADA, 404);

            if (!ValidarSerieAnoEModalidadeTurma(turma))
                throw new RegraNegocioException(MensagemNegocioComuns.MODALIDADE_SERIEANO_TURMA_SONDAGEM_INVALIDA); 

            if (!ValidarAnoLetivoTurma(turma))
                throw new RegraNegocioException(MensagemNegocioComuns.ANO_LETIVO_TURMA_SONDAGEM_INVALIDA);

            return true;
        }


        private static bool ValidarSerieAnoEModalidadeTurma(TurmaElasticDto turma)
        {
            if (turma.Modalidade == (int)Modalidade.Fundamental 
                    && SERIE_ANO_PERMITIDOS.Contains(int.Parse(turma.AnoTurma)) 
                || turma.Modalidade == (int)Modalidade.EJA
                    && turma.TipoTurma == 1 && SERIE_ANO_PERMITIDOS.Contains(int.Parse(turma.AnoTurma)))
                return true;

            return false;
        }

        private static bool ValidarAnoLetivoTurma(TurmaElasticDto turma)
        {
            if (turma.AnoLetivo >= DateTime.Now.Year)
                return true;

            return false;
        }
    }
}
