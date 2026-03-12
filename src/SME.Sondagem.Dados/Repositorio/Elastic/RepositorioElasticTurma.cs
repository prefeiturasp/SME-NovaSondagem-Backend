using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio.Entidades.Elastic;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infra.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Repositorio.Elastic
{
    [ExcludeFromCodeCoverage]
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

            Func<QueryDescriptor<TurmaElasticDto>, Query> query = q => q
                .Bool(b =>
                {
                    var filters = new List<Action<QueryDescriptor<TurmaElasticDto>>>
                    {
                f => f.Term(t => t.Field(ff => ff.CodigoTurma).Value(filtro.TurmaId))
                    };

                    if (filtro.AnoLetivo != 0)
                        filters.Add(f => f.Term(t => t.Field(ff => ff.AnoLetivo).Value(filtro.AnoLetivo)));

                    if (filtro.SemestreId != 0)
                        filters.Add(f => f.Term(t => t.Field(ff => ff.Semestre).Value(filtro.SemestreId)));

                    if (filtro.Modalidade != 0)
                        filters.Add(f => f.Term(t => t.Field(ff => ff.Modalidade).Value(filtro.Modalidade)));

                    if (!string.IsNullOrEmpty(filtro.UeCodigo))
                        filters.Add(f => f.Term(t => t.Field(ff => ff.CodigoEscola).Value(filtro.UeCodigo)));

                    b.Filter(filters.Select<Action<QueryDescriptor<TurmaElasticDto>>, Query>(f =>
                    {
                        var qd = new QueryDescriptor<TurmaElasticDto>();
                        f(qd);
                        return qd;
                    }).ToArray());
                });

                var resultado = await ObterListaAsync(
                    IndicesElastic.INDICE_TURMA,
                    query,
                    "Obter turma por código",
                    new
                    {
                        filtro.TurmaId,
                        AnoLetivo = filtro.AnoLetivo != 0 ? filtro.AnoLetivo : (int?)null,
                        Semestre = filtro.SemestreId != 0 ? filtro.SemestreId : (int?)null,
                        filtro.Modalidade,
                        filtro.UeCodigo
                    }
                );

            return resultado.FirstOrDefault();
        }
    }
}