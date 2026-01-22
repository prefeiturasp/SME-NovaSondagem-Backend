using SME.Sondagem.Aplicacao.Interfaces.Turma;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.Turma
{
    public class ObterPermissaoTurmaUseCase : IObterPermissaoTurmaUseCase
    {
        private static readonly HashSet<int> MODALIDADES_PERMITIDAS = new() { (int)Modalidade.Fundamental, (int)Modalidade.EJA };
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
            ?? throw new RegraNegocioException("Turma não localizada", 400);

            if (!ValidarSerieAnoEModalidadeTurma(turma))
                throw new RegraNegocioException("Somente é possível utilizar a Sondagem para turmas de 1° a 3º no do Ensino Fundamental e etapa de alfabetização da EJA."); 

            if (!ValidarAnoLetivoTurma(turma))
                throw new RegraNegocioException("A Sondagem não se aplica para turmas deste ano letivo.");

            return true;
        }


        private static bool ValidarSerieAnoEModalidadeTurma(TurmaElasticDto turma)
        {
            if (MODALIDADES_PERMITIDAS.Contains(turma.Modalidade) && SERIE_ANO_PERMITIDOS.Contains(int.Parse(turma.AnoTurma)))
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
