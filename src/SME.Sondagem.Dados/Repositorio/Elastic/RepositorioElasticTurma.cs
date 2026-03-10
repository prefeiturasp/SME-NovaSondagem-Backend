using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
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

        public async Task<IEnumerable<TurmaElasticDto>> ObterTurmasPorIds(
            IEnumerable<int> turmaIds,
            CancellationToken cancellationToken)
        {
            var ids = turmaIds?.ToList() ?? [];

            if (ids.Count == 0)
                return [];

            Func<QueryDescriptor<TurmaElasticDto>, Query> query = q =>
                q.Terms(t => t
                    .Field(f => f.CodigoTurma)
                    .Terms(new TermsQueryField(ids.Select(id => FieldValue.Long(id)).ToArray()))
                );

            return await ObterListaAsync(
                IndicesElastic.INDICE_TURMA,
                query,
                "Obter turmas por lista de IDs",
                new { turmaIds = ids }
            );
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