using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio.Entidades.Elastic;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infra.Interfaces;

namespace SME.Sondagem.Dados.Repositorio.Elastic
{
    public class RepositorioElasticAluno : RepositorioElasticBase<AlunoElasticDto>, IRepositorioElasticAluno
    {
        public RepositorioElasticAluno(IServicoTelemetria servicoTelemetria, ElasticsearchClient elasticClient) : base(
            servicoTelemetria, elasticClient)
        {
        }

        public async Task<IEnumerable<AlunoElasticDto>> ObterAlunosPorIdTurma(int idTurma, CancellationToken cancellationToken)
        {
            Func<QueryDescriptor<AlunoElasticDto>, Query> query = q =>
                q.Bool(b => b
                    .Must(
                        m => m.Term(t => t
                            .Field(f => f.CodigoTurma)
                            .Value(idTurma)
                        )
                    )
                );

            var resultado = await ObterListaAsync(
                IndicesElastic.INDICE_ALUNO_MATRICULA_TURMA_DRE,
                query,
                "Obter alunos por Id da turma",
                new { idTurma }
            );

            return resultado.DistinctBy(a => a.CodigoAluno);
        }
    }
}