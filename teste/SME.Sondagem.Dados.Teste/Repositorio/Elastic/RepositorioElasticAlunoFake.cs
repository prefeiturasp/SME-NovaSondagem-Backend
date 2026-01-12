using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SME.SERAp.Prova.Dados;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infra.Interfaces;

namespace SME.Sondagem.Dados.Teste.Repositorio.Elastic
{
    public class RepositorioElasticAlunoFake : IRepositorioElasticAluno
    {
        public IEnumerable<AlunoElasticDto> Retorno { get; set; } = Enumerable.Empty<AlunoElasticDto>();

        public Task<IEnumerable<AlunoElasticDto>> ObterAlunosPorIdTurma(int idTurma, CancellationToken cancellationToken)
        {
            return Task.FromResult(Retorno);
        }

        Task IRepositorioElasticBase<AlunoElasticDto>.ExcluirTodos<TDocument>(string indice, string nomeConsulta)
        {
            throw new NotImplementedException();
        }

        Task<bool> IRepositorioElasticBase<AlunoElasticDto>.ExisteAsync(string indice, string id, string nomeConsulta, object parametro)
        {
            throw new NotImplementedException();
        }

        Task<bool> IRepositorioElasticBase<AlunoElasticDto>.InserirAsync<TRequest>(TRequest entidade, string indice)
        {
            throw new NotImplementedException();
        }

        Task IRepositorioElasticBase<AlunoElasticDto>.InserirBulk<TRequest>(IEnumerable<TRequest> listaDeDocumentos, string indice)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<AlunoElasticDto>> IRepositorioElasticAluno.ObterAlunosPorIdTurma(int idTurma, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<AlunoElasticDto> IRepositorioElasticBase<AlunoElasticDto>.ObterAsync(string indice, string id, string nomeConsulta, object parametro)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<TResponse>> IRepositorioElasticBase<AlunoElasticDto>.ObterListaAsync<TResponse>(string indice, Func<QueryDescriptor<TResponse>, Query> request, string nomeConsulta, object parametro)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<TResponse>> IRepositorioElasticBase<AlunoElasticDto>.ObterTodosAsync<TResponse>(string indice, string nomeConsulta, object parametro)
        {
            throw new NotImplementedException();
        }

        Task<long> IRepositorioElasticBase<AlunoElasticDto>.ObterTotalDeRegistroAPartirDeUmaCondicaoAsync<TDocument>(string indice, string nomeConsulta, Func<QueryDescriptor<TDocument>, Query> request, object parametro)
        {
            throw new NotImplementedException();
        }

        Task<long> IRepositorioElasticBase<AlunoElasticDto>.ObterTotalDeRegistroAsync<TDocument>(string indice, string nomeConsulta, object parametro)
        {
            throw new NotImplementedException();
        }
    }
}
