using Microsoft.AspNetCore.Http;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;
using System;
using System.Collections.Generic;
using System.Text;

using SME.Sondagem.Infra.Dtos;

namespace SME.Sondagem.Dados.Repositorio.Postgres
{
    public class RepositorioRespostaAluno : RepositorioBase<RespostaAluno>, IRepositorioRespostaAluno
    {
        public RepositorioRespostaAluno(SondagemDbContext context) : base(context) { }

        public async Task<IEnumerable<long>> SalvarSondagemAsync(int sondagemId, List<AlunoSondagemDto> alunos)
        {
            var idsSalvos = new List<long>();

            foreach (var aluno in alunos)
            {
                foreach (var respostaDto in aluno.Respostas)
                {
                    var resposta = new RespostaAluno(
                        sondagemId,
                        aluno.AlunoId,
                        respostaDto.RespostaId,
                        respostaDto.RespostaSelecionada,
                        DateTime.UtcNow
                    );
                    // Auditoria vem automática do EntidadeBase + contexto

                    var id = await SalvarAsync(resposta);
                    idsSalvos.Add(id);
                }
            }
            return idsSalvos;
        }

    }
}
