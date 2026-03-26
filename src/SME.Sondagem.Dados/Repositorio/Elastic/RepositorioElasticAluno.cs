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
        private const int TAMANHOBATCH = 100;
        public RepositorioElasticAluno(IServicoTelemetria servicoTelemetria, ElasticsearchClient elasticClient) : base(
            servicoTelemetria, elasticClient)
        {
        }

        public async Task<IEnumerable<AlunoElasticDto>> ObterAlunosPorCodigo2(
            List<string> codigoAlunos, int anoLetivo, CancellationToken cancellationToken)
        {
            var codigos = codigoAlunos?.Distinct().ToList() ?? [];

            if (codigos.Count == 0)
                return [];

            var batches = codigos
                .Select((id, index) => new { id, index })
                .GroupBy(x => x.index / TAMANHOBATCH)
                .Select(g => g.Select(x => x.id).ToList())
                .ToList();

            var tasks = batches.Select(batch =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                Func<QueryDescriptor<AlunoElasticDto>, Query> query = q =>
                    q.Bool(b => b
                        .Must(
                            m => m.Terms(t => t
                                .Field(f => f.CodigoAluno)
                                .Terms(new TermsQueryField(
                                    batch.Select(id => FieldValue.String(id)).ToArray()
                                ))
                            ),
                            m => m.Term(t => t
                                .Field(f => f.AnoLetivo)
                                .Value(anoLetivo)
                            ),
                            m => m.Term(t => t
                                .Field(f => f.CodigoSituacaoMatricula)
                                .Value((int)SituacaoMatriculaAluno.Ativo)
                            )
                        )
                    );

                return ObterListaSemScrollAsync(
                    IndicesElastic.INDICE_ALUNO_MATRICULA_TURMA_DRE,
                    query,
                    "Obter alunos por codigo",
                    new { codigoaluno = batch, anoLetivo }
                );
            });

            var resultados = await Task.WhenAll(tasks);

            return resultados
                .SelectMany(lista => lista)
                .GroupBy(aluno => aluno.CodigoMatricula)
                .Select(agrupado => agrupado
                    .OrderByDescending(aluno => aluno.DataSituacao)
                    .ThenByDescending(aluno => aluno.NumeroAlunoChamada)
                    .First());
        }

        public async Task<IEnumerable<AlunoElasticDto>> ObterAlunosPorCodigo(List<string> codigoAlunos, int anoLetivo, CancellationToken cancellationToken)
        {
            try
            {
                var codigos = codigoAlunos?.Distinct().ToList() ?? [];

                if (codigos.Count == 0)
                    return [];

                //var batches = codigos.Select((id, index) => new { id, index })
                //                .GroupBy(x => x.index / TAMANHOBATCH)
                //                .Select(g => g.Select(x => x.id).ToList())
                //                .ToList();

                var resultado = new List<AlunoElasticDto>();
                //foreach (var codigo in codigos)
                //{
                    cancellationToken.ThrowIfCancellationRequested();
                //Func<QueryDescriptor<AlunoElasticDto>, Query> query = q =>
                //    q.Bool(b => b
                //        .Must(
                //            m => m.Terms(t => t
                //                .Field(f => f.CodigoAluno)
                //                .Terms(new TermsQueryField(
                //                    codigos.Select(id => FieldValue.String(id)).ToArray()
                //                ))
                //            ),
                //            m => m.Term(t => t
                //                .Field(f => f.AnoLetivo)
                //                .Value(anoLetivo)
                //            ),
                //            m => m.Term(t => t
                //                .Field(f => f.CodigoSituacaoMatricula)
                //                .Value((int)SituacaoMatriculaAluno.Ativo)
                //            )
                //        )
                //    );
                Func<QueryDescriptor<AlunoElasticDto>, Query> query = q =>
                                q.Terms(t => t
                                    .Field(f => f.CodigoAluno)
                                    .Terms(new TermsQueryField(
                                        codigos.Select(id => FieldValue.String(id)).ToArray()
                                    ))

                                );

                

                var parcial = await ObterListaAsync2(
                            IndicesElastic.INDICE_ALUNO_MATRICULA_TURMA_DRE,
                            query,
                            "Obter alunos por Id da turma",
                            new { codigoaluno = codigos, anoLetivo }
                        );

                    if (parcial != null)
                        resultado.AddRange(parcial);
               // }
                var listaAtivo = resultado?.GroupBy(aluno => aluno.CodigoMatricula)
                    .Select(agrupado => agrupado.OrderByDescending(aluno => aluno.DataSituacao)
                                                .ThenByDescending(aluno => aluno.NumeroAlunoChamada)
                                                .First())
                     .Where(aluno => aluno.CodigoSituacaoMatricula == (int)SituacaoMatriculaAluno.Ativo && aluno.AnoLetivo == anoLetivo);

                var lista = listaAtivo?.ToList() ?? [];
                return lista;
            }
            catch (Exception ex)
            {

                throw;
            }

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
    }
}