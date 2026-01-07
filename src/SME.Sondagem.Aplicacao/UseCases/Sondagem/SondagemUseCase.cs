
    using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem
{
    public class SondagemUseCase : ISondagemUseCase
    {
        private readonly IRepositorioSondagem _sondagemRepository;
        private readonly IRepositorioRespostaAluno _repositorioRespostaAluno;

        public SondagemUseCase(
            IRepositorioSondagem sondagemRepository,
            IRepositorioRespostaAluno repositorioRespostaAluno)
        {
            _sondagemRepository = sondagemRepository;
            _repositorioRespostaAluno = repositorioRespostaAluno;
        }

        public async Task<IEnumerable<object>> ObterTodasSondagensAsync()
        {
            return await _sondagemRepository.ObterTodosAsync();
        }

        public async Task<IEnumerable<long>> SalvarOuAtualizarSondagemAsync(SondagemSalvarDto dto)
        {
            return await _repositorioRespostaAluno.SalvarSondagemAsync(dto.SondagemId, dto.Alunos);
        }
    }
}
