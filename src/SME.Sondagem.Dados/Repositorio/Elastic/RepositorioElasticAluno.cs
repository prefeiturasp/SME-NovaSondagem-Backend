using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio.Entidades.Elastic;
using SME.Sondagem.Dominio.Enums;
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

        public async Task<IEnumerable<AlunoElasticDto>> ObterAlunosPorIdTurma(int idTurma, int anoLetivo, CancellationToken cancellationToken)
        {
            Func<QueryDescriptor<AlunoElasticDto>, Query> query = q =>
                q.Bool(b => b
                    .Must(
                        m => m.Term(t => t
                            .Field(f => f.CodigoTurma)
                            .Value(idTurma)
                        ),
                        m => m.Term(r => r
                            .Field(f => f.AnoLetivo)
                            .Value(anoLetivo)
                        )
                    )
                );

            var alunosTurma = await ObterListaAsync(
                IndicesElastic.INDICE_ALUNO_MATRICULA_TURMA_DRE,
                query,
                "Obter alunos por Id da turma",
                new { idTurma, anoLetivo }
            );

            var hoje = DateTime.UtcNow;

            var resultado = alunosTurma?.GroupBy(aluno => aluno.CodigoMatricula)
                            .Select(agrupado => agrupado.OrderByDescending(aluno => aluno.DataSituacao)
                                                        .ThenByDescending(aluno => aluno.NumeroAlunoChamada)
                                                        .First())
                            .Where(aluno => aluno.CodigoSituacaoMatricula == (int)SituacaoMatriculaAluno.Ativo
                                         && aluno.DataSituacao.Date <= hoje);

            var lista = resultado?.ToList() ?? [];
            return lista;
        }
    }
}