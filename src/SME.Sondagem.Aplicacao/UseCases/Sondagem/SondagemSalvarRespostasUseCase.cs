using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
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
            
            if (sondagemAtiva == null)
               throw new NegocioException(MensagemNegocioComuns.NENHUM_SONDAGEM_ATIVA_ENCONRADA);

            if (sondagemAtiva.Id != dto.SondagemId)
                throw new NegocioException(MensagemNegocioComuns.SALVAR_SOMENTE_PARA_SONDAGEM_ATIVA);

            var periodosBimestresAtivos = sondagemAtiva.PeriodosBimestre.Where(x => !x.Excluido);

            var alunosIds = dto.Alunos.Select(a => a.AlunoId);
            var questoresIds = dto.Alunos.SelectMany(a => a.Respostas.Select(r => r.QuestaoId));
            var repostasAlunos = await _repositorioSondagemResposta.ObterRespostasPorSondagemEAlunosAsync(sondagemId, alunosIds, questoresIds);

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
                            .FirstOrDefault(r => r.AlunoId == aluno.AlunoId && r.QuestaoId == respostaDto.QuestaoId);
                        if (respostaExistente == null) continue;
                        respostaExistente.AtualizarResposta(respostaDto.OpcaoRepostaId, DateTime.UtcNow);
                        respostas.Add(respostaExistente);
                    }
                    else
                    {
                        respostas.Add(new RespostaAluno(
                            sondagemId,
                            aluno.AlunoId,
                            respostaDto.QuestaoId,
                            respostaDto.OpcaoRepostaId,
                            DateTime.UtcNow,
                            respostaDto.BimestreId
                        ));
                    }
                }
            }

            return await _repositorioSondagemResposta.SalvarAsync(respostas);
        }
    }
}
