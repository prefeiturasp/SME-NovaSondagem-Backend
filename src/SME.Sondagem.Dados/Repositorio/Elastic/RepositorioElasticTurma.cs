using Elastic.Clients.Elasticsearch;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio.Entidades.Elastic;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infra.Interfaces;

namespace SME.Sondagem.Dados.Repositorio.Elastic
{
    public class RepositorioElasticTurma : RepositorioElasticBase<TurmaElasticDto>, IRepositorioElasticTurma
    {
        public RepositorioElasticTurma(IServicoTelemetria servicoTelemetria, ElasticsearchClient elasticClient) : base(
            servicoTelemetria, elasticClient)
        {
        }

        public async Task<IEnumerable<TurmaElasticDto>> ObterTodasTurma()
        {

            var retorno = await ObterTodosAsync<TurmaElasticDto>(IndicesElastic.INDICE_TURMA,"Obter turma por código");

            return (IEnumerable<TurmaElasticDto>)(retorno.Any() ? retorno : []);
        }

        public async Task<TurmaElasticDto?> ObterTurmaPorId(FiltroQuestionario filtro, CancellationToken cancellationToken)
        {
            if (filtro.TurmaId == 0)
                return null;

            var retorno = await ObterAsync(
                IndicesElastic.INDICE_TURMA,
                filtro.TurmaId.ToString(),
                "Obter turma por código",
                new
                {
                    filtro.TurmaId,
                    AnoLetivo = filtro.AnoLetivo != 0 ? filtro.AnoLetivo : (int?)null,
                    BimestreId = filtro.BimestreId != 0 ? filtro.BimestreId : (int?)null,
                    SemestreId = filtro.SemestreId != 0 ? filtro.SemestreId : (int?)null,
                    Ano = filtro.Ano != 0 ? filtro.Ano : (int?)null,
                    filtro.Modalidade,
                    filtro.UeCodigo
                }
            );

            return retorno;
        }
    }
}