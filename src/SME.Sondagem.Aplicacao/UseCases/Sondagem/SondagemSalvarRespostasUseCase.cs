using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Infra.Exceptions;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem
{
    public class SondagemSalvarRespostasUseCase : ISondagemSalvarRespostasUseCase
    {
        private readonly IRepositorioSondagem _repositorioSondagem;
        private readonly IRepositorioRespostaAluno _repositorioSondagemResposta;
        public SondagemSalvarRespostasUseCase(IRepositorioSondagem repositorioSondagem, IRepositorioRespostaAluno repositorioSondagemResposta)
        {
            _repositorioSondagem = repositorioSondagem;
            _repositorioSondagemResposta = repositorioSondagemResposta;
        }

        public async Task<bool> SalvarOuAtualizarSondagemAsync(SondagemSalvarDto dto)
        {
            var sondagemAtiva = await _repositorioSondagem.ObterSondagemAtiva();
            var respostas = new List<RespostaAluno>();
            var sondagemId = dto.SondagemId;

            //sondagemAtiva.PeriodosBimestre
            // comparar com o que vem dentro do aluno.respostas
            
            if (sondagemAtiva == null)
               throw new NegocioException("Nenhuma sondagem ativa encontrada para salvar as respostas.");

            if (sondagemAtiva.Id != dto.SondagemId)
                throw new NegocioException("As respostas só podem ser salvas para a sondagem ativa.");

            // criar uma régra que valida se o bimestre enviado pelo front está ativo na sondagem.periodosBimestres
            var periodosBimestresAtivos = sondagemAtiva.PeriodosBimestre;
            

            var repostasAlunos = await _repositorioSondagemResposta
                .ObterRespostasPorSondagemEAlunosAsync(sondagemId, dto.Alunos.Select(a => a.AlunoId).ToList(), dto.Alunos.SelectMany(a => a.Respostas.Select(r => r.QuestaoId)));

            foreach (var aluno in dto.Alunos)
            {
                foreach (var respostaDto in aluno.Respostas)
                {
                   var periodoBimestreAtivo = periodosBimestresAtivos
                        .FirstOrDefault(pb => pb.BimestreId == respostaDto.BimestreId);

                    if (periodoBimestreAtivo == null)
                        continue;

                    if (periodoBimestreAtivo.DataInicio < DateTime.Now || periodoBimestreAtivo.DataFim > DateTime.Now)
                        continue;

                    if(repostasAlunos is not null )
                    {
                        var respostaExistente = repostasAlunos
                            .FirstOrDefault(r => r.AlunoId == aluno.AlunoId && r.QuestaoId == respostaDto.RespostaId);
                        if (respostaExistente != null)
                        {
                            respostaExistente.AtualizarResposta(respostaDto.RespostaSelecionada, DateTime.UtcNow);
                            respostas.Add(respostaExistente);
                        }
                    }
                    else
                    {
                        var resposta = new RespostaAluno(
                            sondagemId,
                            aluno.AlunoId,
                            respostaDto.RespostaId,
                            respostaDto.RespostaSelecionada,
                            DateTime.UtcNow
                        );

                        respostas.Add(resposta);

                    }
                }
            }

            return await _repositorioSondagemResposta.SalvarAsync(respostas);
        }
    }
}
