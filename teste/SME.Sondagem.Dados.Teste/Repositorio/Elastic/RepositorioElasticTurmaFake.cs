using Elastic.Clients.Elasticsearch.QueryDsl;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Dados.Teste.Repositorio.Elastic
{
    public class RepositorioElasticTurmaFake : IRepositorioElasticTurma
    {
        public TurmaElasticDto Retorno { get; set; }

        public RepositorioElasticTurmaFake()
        {
        }

        public Task<TurmaElasticDto> ObterTurmaPorId(FiltroQuestionario filtro, CancellationToken cancellationToken)
        {
            return Task.FromResult(Retorno);
        }

        public Task<TurmaElasticDto> ObterAsync(string indice, string id, string nomeConsulta, object parametro = null)
        {
            return Task.FromResult(Retorno);
        }

        public Task<IEnumerable<TResponse>> ObterListaAsync<TResponse>(string indice, Func<QueryDescriptor<TResponse>, Query> request, string nomeConsulta, object parametro = null) where TResponse : class
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TResponse>> ObterTodosAsync<TResponse>(string indice, string nomeConsulta, object parametro = null) where TResponse : class
        {
            throw new NotImplementedException();
        }

        public Task<long> ObterTotalDeRegistroAsync<TDocument>(string indice, string nomeConsulta, object parametro = null) where TDocument : class
        {
            throw new NotImplementedException();
        }

        public Task<long> ObterTotalDeRegistroAPartirDeUmaCondicaoAsync<TDocument>(string indice, string nomeConsulta, Func<QueryDescriptor<TDocument>, Query> request, object parametro = null) where TDocument : class
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExisteAsync(string indice, string id, string nomeConsulta, object parametro = null)
        {
            throw new NotImplementedException();
        }

        public Task InserirBulk<TRequest>(IEnumerable<TRequest> listaDeDocumentos, string indice) where TRequest : class
        {
            throw new NotImplementedException();
        }

        public Task<bool> InserirAsync<TRequest>(TRequest entidade, string indice) where TRequest : class
        {
            throw new NotImplementedException();
        }

        public Task ExcluirTodos<TDocument>(string indice = "", string nomeConsulta = "") where TDocument : class
        {
            throw new NotImplementedException();
        }
    }

}
