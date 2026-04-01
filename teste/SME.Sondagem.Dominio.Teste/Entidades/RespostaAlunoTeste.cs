using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
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
            var turmaid = 1;
            var dreId = 2;
            var ueId = 3;
            var modalidadeId = 4;
            var anoLetivo = 2026;

            var respostaAluno = new RespostaAluno(sondagemId, alunoId, questaoId, opcaoRespostaId, dataResposta,turmaid,ueId,dreId,anoLetivo,modalidadeId);

            Assert.Equal(sondagemId, respostaAluno.SondagemId);
            Assert.Equal(alunoId, respostaAluno.AlunoId);
            Assert.Equal(questaoId, respostaAluno.QuestaoId);
            Assert.Equal(opcaoRespostaId, respostaAluno.OpcaoRespostaId);
            Assert.Equal(dataResposta, respostaAluno.DataResposta);
            Assert.Null(respostaAluno.BimestreId);
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

            var turmaid = 1;
            var dreId = 2;
            var ueId = 3;
            var modalidadeId = 4;
            var anoLetivo = 2026;

            var respostaAluno = new RespostaAluno(sondagemId, alunoId, questaoId, opcaoRespostaId, dataResposta, turmaid, ueId, dreId, anoLetivo, modalidadeId, bimestre);

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
            var turmaid = 1;
            var dreId = 2;
            var ueId = 3;
            var modalidadeId = 4;
            var anoLetivo = 2026;
            var respostaAluno = new RespostaAluno(1, 2, 3, 4, DateTime.Now, turmaid, ueId, dreId, anoLetivo, modalidadeId);

            Assert.NotNull(respostaAluno);
            Assert.Null(respostaAluno.Sondagem);
        }

        [Fact]
        public void Deve_possuir_navegacao_para_aluno()
        {
            var turmaid = 1;
            var dreId = 2;
            var ueId = 3;
            var modalidadeId = 4;
            var anoLetivo = 2026;
            var respostaAluno = new RespostaAluno(1, 2, 3, 4, DateTime.Now, turmaid, ueId, dreId, anoLetivo, modalidadeId);

            Assert.NotNull(respostaAluno);
        }

        [Fact]
        public void Deve_possuir_navegacao_para_questao()
        {
            var turmaid = 1;
            var dreId = 2;
            var ueId = 3;
            var modalidadeId = 4;
            var anoLetivo = 2026;
            var respostaAluno = new RespostaAluno(1, 2, 3, 4, DateTime.Now, turmaid, ueId, dreId, anoLetivo, modalidadeId);

            Assert.NotNull(respostaAluno);
            Assert.Null(respostaAluno.Questao);
        }

        [Fact]
        public void Deve_possuir_navegacao_para_opcao_resposta()
        {
            var turmaid = 1;
            var dreId = 2;
            var ueId = 3;
            var modalidadeId = 4;
            var anoLetivo = 2026;
            var respostaAluno = new RespostaAluno(1, 2, 3, 4, DateTime.Now, turmaid, ueId, dreId, anoLetivo, modalidadeId);

            Assert.NotNull(respostaAluno);
            Assert.Null(respostaAluno.OpcaoResposta);
        }

        [Fact]
        public void Deve_possuir_navegacao_para_bimestre()
        {
            var turmaid = 1;
            var dreId = 2;
            var ueId = 3;
            var modalidadeId = 4;
            var anoLetivo = 2026;
            var respostaAluno = new RespostaAluno(1, 2, 3, 4, DateTime.Now, turmaid, ueId, dreId, anoLetivo, modalidadeId, 5);

            Assert.NotNull(respostaAluno);
            Assert.Null(respostaAluno.Bimestre);
        }

        [Fact]
        public void Deve_herdar_propriedades_da_entidade_base()
        {
            var turmaid = 1;
            var dreId = 2;
            var ueId = 3;
            var modalidadeId = 4;
            var anoLetivo = 2026;

            var respostaAluno = new RespostaAluno(15, 20, 25, 30, DateTime.Now, turmaid, ueId, dreId, anoLetivo, modalidadeId);
            respostaAluno.CriadoEm = DateTime.UtcNow;
            Assert.Equal(0, respostaAluno.Id);
            Assert.Null(respostaAluno.AlteradoEm);
            Assert.Null(respostaAluno.AlteradoPor);
            Assert.Null(respostaAluno.AlteradoRF);
            Assert.NotEqual(default, respostaAluno.CriadoEm);
            Assert.Equal(string.Empty, respostaAluno.CriadoPor);
            Assert.Equal(string.Empty, respostaAluno.CriadoRF);
            Assert.False(respostaAluno.Excluido);
        }
    }
}
