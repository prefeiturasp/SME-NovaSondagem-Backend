using SME.Sondagem.Dominio.Entidades;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class AlunoTeste
    {
        [Fact]
        public void Deve_criar_aluno_com_todos_os_dados_preenchidos()
        {
            var ra = "123456";
            var nome = "Aluno Teste";
            var isPap = true;
            var isAee = true;
            var isPcd = false;
            var deficienciaId = 1;
            var deficienciaNome = "Visual";
            var racaId = 2;
            var racaNome = "Branca";
            var corId = 3;
            var corNome = "Parda";

            var aluno = new Aluno(
                ra,
                nome,
                isPap,
                isAee,
                isPcd,
                deficienciaId,
                deficienciaNome,
                racaId,
                racaNome,
                corId,
                corNome
            );

            Assert.Equal(ra, aluno.RaAluno);
            Assert.Equal(nome, aluno.NomeAluno);
            Assert.Equal(isPap, aluno.IsPap);
            Assert.Equal(isAee, aluno.IsAee);
            Assert.Equal(isPcd, aluno.IsPcd);
            Assert.Equal(deficienciaId, aluno.DeficienciaId);
            Assert.Equal(deficienciaNome, aluno.DeficienciaNome);
            Assert.Equal(racaId, aluno.RacaId);
            Assert.Equal(racaNome, aluno.RacaNome);
            Assert.Equal(corId, aluno.CorId);
            Assert.Equal(corNome, aluno.CorNome);
        }

        [Fact]
        public void Deve_permitir_campos_nulos()
        {
            var aluno = new Aluno(
                null,
                null,
                false,
                false,
                false,
                null,
                null,
                null,
                null,
                null,
                null
            );

            Assert.Null(aluno.RaAluno);
            Assert.Null(aluno.NomeAluno);
            Assert.False(aluno.IsPap);
            Assert.False(aluno.IsAee);
            Assert.False(aluno.IsPcd);
            Assert.Null(aluno.DeficienciaId);
            Assert.Null(aluno.DeficienciaNome);
            Assert.Null(aluno.RacaId);
            Assert.Null(aluno.RacaNome);
            Assert.Null(aluno.CorId);
            Assert.Null(aluno.CorNome);
        }

        [Fact]
        public void Deve_inicializar_colecao_de_respostas()
        {
            var aluno = new Aluno(
                "123",
                "Aluno",
                false,
                false,
                false,
                null,
                null,
                null,
                null,
                null,
                null
            );

            Assert.NotNull(aluno.Respostas);
            Assert.Empty(aluno.Respostas);
        }
    }
}
