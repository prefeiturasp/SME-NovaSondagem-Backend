using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Dominio.ValueObjects;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{

    public class RespostaAlunoTeste
    {

        [Fact]
        public void Deve_criar_resposta_aluno_com_dados_validos()
        {
            var sondagemId = 10;
            var alunoId = 5;
            var questaoId = 3;
            var opcaoRespostaId = 7;
            var dataResposta = new DateTime(2026, 1, 8);
            var contexto = CriarContextoEducacional();

            var respostaAluno = new RespostaAluno(sondagemId, alunoId, questaoId, opcaoRespostaId, dataResposta,contexto);

            Assert.Equal(sondagemId, respostaAluno.SondagemId);
            Assert.Equal(alunoId, respostaAluno.AlunoId);
            Assert.Equal(questaoId, respostaAluno.QuestaoId);
            Assert.Equal(opcaoRespostaId, respostaAluno.OpcaoRespostaId);
            Assert.Equal(dataResposta, respostaAluno.DataResposta);
            Assert.Equal(contexto.AnoTurma, respostaAluno.AnoTurma);
            Assert.NotNull(respostaAluno.BimestreId);
        }

        [Fact]
        public void Deve_criar_resposta_aluno_com_bimestre_id()
        {
            var sondagemId = 10;
            var alunoId = 5;
            var questaoId = 3;
            var opcaoRespostaId = 7;
            var dataResposta = new DateTime(2026, 1, 8);
            var bimestre = 2;
            var contexto = CriarContextoEducacional();

            var respostaAluno = new RespostaAluno(sondagemId, alunoId, questaoId, opcaoRespostaId, dataResposta, contexto);

            Assert.Equal(sondagemId, respostaAluno.SondagemId);
            Assert.Equal(alunoId, respostaAluno.AlunoId);
            Assert.Equal(questaoId, respostaAluno.QuestaoId);
            Assert.Equal(opcaoRespostaId, respostaAluno.OpcaoRespostaId);
            Assert.Equal(dataResposta, respostaAluno.DataResposta);
            Assert.Equal(bimestre, respostaAluno.BimestreId);
        }

        [Fact]
        public void Deve_possuir_navegacao_para_sondagem()
        {
            var contexto = CriarContextoEducacional();
            var respostaAluno = new RespostaAluno(1, 2, 3, 4, DateTime.Now,contexto);

            Assert.NotNull(respostaAluno);
            Assert.Null(respostaAluno.Sondagem);
        }

        [Fact]
        public void Deve_possuir_navegacao_para_aluno()
        {
            var contexto = CriarContextoEducacional();
            var respostaAluno = new RespostaAluno(1, 2, 3, 4, DateTime.Now, contexto);

            Assert.NotNull(respostaAluno);
        }

        [Fact]
        public void Deve_possuir_navegacao_para_questao()
        {
            var contexto = CriarContextoEducacional();
            var respostaAluno = new RespostaAluno(1, 2, 3, 4, DateTime.Now,contexto);

            Assert.NotNull(respostaAluno);
            Assert.Null(respostaAluno.Questao);
        }

        [Fact]
        public void Deve_possuir_navegacao_para_opcao_resposta()
        {
            var contexto = CriarContextoEducacional();
            var respostaAluno = new RespostaAluno(1, 2, 3, 4, DateTime.Now,contexto);

            Assert.NotNull(respostaAluno);
            Assert.Null(respostaAluno.OpcaoResposta);
        }

        [Fact]
        public void Deve_possuir_navegacao_para_bimestre()
        {
            var contexto = CriarContextoEducacional();
            var respostaAluno = new RespostaAluno(1, 2, 3, 4, DateTime.Now, contexto);

            Assert.NotNull(respostaAluno);
            Assert.Null(respostaAluno.Bimestre);
        }

        [Fact]
        public void Deve_herdar_propriedades_da_entidade_base()
        {
            var contexto = CriarContextoEducacional();

            var respostaAluno = new RespostaAluno(15, 20, 25, 30, DateTime.Now, contexto)
            {
                CriadoEm = DateTime.UtcNow
            };
            Assert.Equal(0, respostaAluno.Id);
            Assert.Null(respostaAluno.AlteradoEm);
            Assert.Null(respostaAluno.AlteradoPor);
            Assert.Null(respostaAluno.AlteradoRF);
            Assert.NotEqual(default, respostaAluno.CriadoEm);
            Assert.Equal(string.Empty, respostaAluno.CriadoPor);
            Assert.Equal(string.Empty, respostaAluno.CriadoRF);
            Assert.False(respostaAluno.Excluido);
        }

        [Fact]
        public void Nao_deve_atualizar_resposta_quando_opcao_e_contexto_nao_mudaram()
        {
            var contexto = CriarContextoEducacional();
            var dataResposta = DateTime.UtcNow.AddDays(-1);
            var respostaAluno = new RespostaAluno(1, 2, 3, 4, dataResposta, contexto);

            var atualizado = respostaAluno.AtualizarResposta(4, DateTime.UtcNow, contexto);

            Assert.False(atualizado);
            Assert.Equal(dataResposta, respostaAluno.DataResposta);
        }

        [Fact]
        public void Deve_atualizar_resposta_quando_opcao_mudar()
        {
            var contexto = CriarContextoEducacional();
            var novaDataResposta = DateTime.UtcNow;
            var respostaAluno = new RespostaAluno(1, 2, 3, 4, novaDataResposta.AddDays(-1), contexto);

            var atualizado = respostaAluno.AtualizarResposta(5, novaDataResposta, contexto);

            Assert.True(atualizado);
            Assert.Equal(5, respostaAluno.OpcaoRespostaId);
            Assert.Equal(novaDataResposta, respostaAluno.DataResposta);
        }

        private static ContextoEducacional CriarContextoEducacional()
        {
            return new ContextoEducacional
            {
                TurmaId = "1",
                UeId = "3",
                DreId = "2",
                AnoLetivo = 2026,
                AnoTurma = 3,
                ModalidadeId = 4,
                RacaCorId = 1,
                GeneroSexoId = 1,
                BimestreId = 2
            };
        }
    }
}
