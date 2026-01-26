using SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;
using Xunit;

namespace SME.Sondagem.Infra.Teste.DTO.QuestionarioBimestre
{
    public class QuestionarioBimestreDtoTeste
    {
        [Fact]
        public void Deve_Criar_QuestionarioBimestreDto_Com_Propriedades_Padrao()
        {
            var dto = new QuestionarioBimestreDto();

            Assert.Equal(0, dto.QuestionarioId);
            Assert.Equal(0, dto.BimestreId);
            Assert.Null(dto.DescricaoBimestre);
            Assert.Null(dto.CodBimestreEnsinoEol);
        }

        [Fact]
        public void Deve_Definir_E_Obter_QuestionarioId()
        {
            var dto = new QuestionarioBimestreDto();
            var questionarioId = 10;

            dto.QuestionarioId = questionarioId;

            Assert.Equal(questionarioId, dto.QuestionarioId);
        }

        [Fact]
        public void Deve_Definir_E_Obter_BimestreId()
        {
            var dto = new QuestionarioBimestreDto();
            var bimestreId = 2;

            dto.BimestreId = bimestreId;

            Assert.Equal(bimestreId, dto.BimestreId);
        }

        [Fact]
        public void Deve_Definir_E_Obter_DescricaoBimestre()
        {
            var dto = new QuestionarioBimestreDto();
            var descricao = "1º Bimestre";

            dto.DescricaoBimestre = descricao;

            Assert.Equal(descricao, dto.DescricaoBimestre);
        }

        [Fact]
        public void Deve_Definir_E_Obter_CodBimestreEnsinoEol()
        {
            var dto = new QuestionarioBimestreDto();
            var codigo = 1;

            dto.CodBimestreEnsinoEol = codigo;

            Assert.Equal(codigo, dto.CodBimestreEnsinoEol);
        }

        [Fact]
        public void Deve_Aceitar_DescricaoBimestre_Nula()
        {
            var dto = new QuestionarioBimestreDto
            {
                DescricaoBimestre = null
            };

            Assert.Null(dto.DescricaoBimestre);
        }

        [Fact]
        public void Deve_Aceitar_CodBimestreEnsinoEol_Nulo()
        {
            var dto = new QuestionarioBimestreDto
            {
                CodBimestreEnsinoEol = null
            };

            Assert.Null(dto.CodBimestreEnsinoEol);
        }
    }
}