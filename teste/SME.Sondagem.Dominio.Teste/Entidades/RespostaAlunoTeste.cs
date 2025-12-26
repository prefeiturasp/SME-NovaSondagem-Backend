using SME.Sondagem.Dominio.Entidades.Sondagem;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class RespostaAlunoTeste
    {
        [Fact]
        public void Deve_criar_resposta_aluno_com_todos_os_dados()
        {
            var sondagemId = 1;
            var alunoId = 2;
            var questaoId = 3;
            var opcaoRespostaId = 4;
            var dataResposta = new DateTime(2025, 1, 10, 14, 30, 0);

            var resposta = new RespostaAluno(
                sondagemId,
                alunoId,
                questaoId,
                opcaoRespostaId,
                dataResposta
            );

            Assert.Equal(sondagemId, resposta.SondagemId);
            Assert.Equal(alunoId, resposta.AlunoId);
            Assert.Equal(questaoId, resposta.QuestaoId);
            Assert.Equal(opcaoRespostaId, resposta.OpcaoRespostaId);
            Assert.Equal(dataResposta, resposta.DataResposta);
        }

        [Fact]
        public void Deve_possuir_navegacoes_para_ef_core()
        {
            var resposta = CriarRespostaPadrao();

            Assert.Null(resposta.Sondagem);
            Assert.Null(resposta.Aluno);
            Assert.Null(resposta.Questao);
            Assert.Null(resposta.OpcaoResposta);
        }

        private static RespostaAluno CriarRespostaPadrao()
        {
            return new RespostaAluno(
                sondagemId: 10,
                alunoId: 20,
                questaoId: 30,
                opcaoRespostaId: 40,
                dataResposta: DateTime.UtcNow
            );
        }
    }
}
