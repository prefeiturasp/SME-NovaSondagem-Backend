using SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;
using Xunit;

namespace SME.Sondagem.Infra.Teste.DTO.QuestionarioBimestre
{
    public class AtualizarVinculosBimestresDtoTeste
    {
        [Fact]
        public void Deve_Criar_AtualizarVinculosBimestresDto_Com_Valores_Padrao()
        {
            var dto = new AtualizarVinculosBimestresDto();

            Assert.Null(dto.QuestionarioId);
            Assert.NotNull(dto.BimestreIds);
            Assert.Empty(dto.BimestreIds);
        }

        [Fact]
        public void Deve_Definir_E_Obter_QuestionarioId()
        {
            var dto = new AtualizarVinculosBimestresDto();
            var questionarioId = 35;

            dto.QuestionarioId = questionarioId;

            Assert.Equal(questionarioId, dto.QuestionarioId);
        }

        [Fact]
        public void Deve_Aceitar_QuestionarioId_Nulo()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = null
            };

            Assert.Null(dto.QuestionarioId);
        }

        [Fact]
        public void Deve_Adicionar_BimestreIds_Na_Lista()
        {
            var dto = new AtualizarVinculosBimestresDto();

            dto.BimestreIds.Add(1);
            dto.BimestreIds.Add(2);

            Assert.Equal(2, dto.BimestreIds.Count);
        }

        [Fact]
        public void Deve_Criar_Com_Inicializador_Completo()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 40,
                BimestreIds = new List<int> { 1, 2, 3 }
            };

            Assert.Equal(40, dto.QuestionarioId);
            Assert.Equal(3, dto.BimestreIds.Count);
        }

        [Fact]
        public void Deve_Permitir_Lista_Vazia_De_BimestreIds()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 45,
                BimestreIds = new List<int>()
            };

            Assert.Empty(dto.BimestreIds);
        }
    }
}