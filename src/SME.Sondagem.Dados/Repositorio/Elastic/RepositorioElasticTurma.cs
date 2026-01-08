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

        public async Task<TurmaElasticDto> ObterTurmaPorId(FiltroQuestionario filtro, CancellationToken cancellationToken)
        {
            if (filtro.TurmaId == 0)
            {
                return null;
            }

            Func<QueryDescriptor<TurmaElasticDto>, Query> query = q =>
                q.Bool(b => b
                    .Must(
                        m => m.Term(t => t
                            .Field("codigoturma")
                            .Value(filtro.TurmaId)
                        )
                    )
                );

            return await ObterAsync(
                IndicesElastic.INDICE_TURMA,
                filtro.TurmaId.ToString(),
                "Obter turma por código",
                new { filtro.TurmaId }
            );
        }
    }
}