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

            var resultado = alunosTurma?.GroupBy(aluno => aluno.CodigoMatricula)
                              .Select(agrupado => agrupado.OrderByDescending(aluno => aluno.DataSituacao)
                                                          .ThenByDescending(aluno => aluno.NumeroAlunoChamada)
                                                          .First())
                              .Where(aluno => aluno.CodigoSituacaoMatricula == (int)SituacaoMatriculaAluno.Ativo);

            var lista = resultado?.ToList() ?? [];
            return lista;
        }

        public async Task<IEnumerable<AlunoElasticDto>> ObterAlunosPorCodigosAlunos(
            IEnumerable<int> codigosAlunos,
            CancellationToken cancellationToken)
        {
            const int tamanhoBatch = 100;

            var codigos = codigosAlunos?.Distinct().ToList() ?? [];
            if (codigos.Count == 0)
                return [];

            var batches = codigos
                .Select((codigo, index) => new { codigo, index })
                .GroupBy(x => x.index / tamanhoBatch)
                .Select(g => g.Select(x => x.codigo).ToList())
                .ToList();

            var resultado = new List<AlunoElasticDto>();
            foreach (var batch in batches)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Func<QueryDescriptor<AlunoElasticDto>, Query> query = q =>
                    q.Terms(t => t
                        .Field(f => f.CodigoAluno)
                        .Terms(new TermsQueryField(
                            batch.Select(id => FieldValue.Long(id)).ToArray()
                        ))
                    );

                var parcial = await ObterListaAsync(
                    IndicesElastic.INDICE_ALUNO_MATRICULA_TURMA_DRE,
                    query,
                    "Obter alunos por lista de códigos",
                    new { codigosAlunos = batch }
                );

                if (parcial != null)
                    resultado.AddRange(parcial);
            }

            return resultado;
        }
    }
}