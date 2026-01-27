using SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;
using Xunit;

namespace SME.Sondagem.Infra.Teste.DTO.QuestionarioBimestre
{
    public class VincularBimestresDtoTeste
    {
        [Fact]
        public void Deve_Criar_VincularBimestresDto_Com_Lista_Vazia()
        {
            var dto = new VincularBimestresDto();

            Assert.Equal(0, dto.QuestionarioId);
            Assert.NotNull(dto.BimestreIds);
            Assert.Empty(dto.BimestreIds);
        }

        [Fact]
        public void Deve_Definir_E_Obter_QuestionarioId()
        {
            var dto = new VincularBimestresDto();
            var questionarioId = 25;

            dto.QuestionarioId = questionarioId;

            Assert.Equal(questionarioId, dto.QuestionarioId);
        }

        [Fact]
        public void Deve_Adicionar_BimestreIds_Na_Lista()
        {
            var dto = new VincularBimestresDto();

            dto.BimestreIds.Add(1);
            dto.BimestreIds.Add(2);
            dto.BimestreIds.Add(3);

            Assert.Equal(3, dto.BimestreIds.Count);
            Assert.Contains(1, dto.BimestreIds);
            Assert.Contains(2, dto.BimestreIds);
            Assert.Contains(3, dto.BimestreIds);
        }

        [Fact]
        public void Deve_Criar_Com_Inicializador_De_Objeto_E_Collection_Initializer()
        {
            var dto = new VincularBimestresDto
            {
                QuestionarioId = 30,
                BimestreIds = new List<int> { 1, 2, 3, 4 }
            };

            Assert.Equal(30, dto.QuestionarioId);
            Assert.Equal(4, dto.BimestreIds.Count);
        }

        [Fact]
        public void Deve_Permitir_Lista_Vazia_De_BimestreIds()
        {
            var dto = new VincularBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int>()
            };

            Assert.Empty(dto.BimestreIds);
        }
    }
}