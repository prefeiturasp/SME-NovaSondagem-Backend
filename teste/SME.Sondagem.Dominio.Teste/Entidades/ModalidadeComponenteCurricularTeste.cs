using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Entidades
{
    public class ModalidadeComponenteCurricularTeste
    {
        [Fact]
        public void Construtor_DeveAtribuirModalidadeIdCorretamente()
        {
            var modalidade = Modalidade.Fundamental;
            var componenteCurricularId = 10;

            var entidade = new ModalidadeComponenteCurricular(modalidade, componenteCurricularId);

            Assert.Equal(modalidade, entidade.ModalidadeId);
        }

        [Fact]
        public void Construtor_DeveAtribuirComponenteCurricularIdCorretamente()
        {
            var modalidade = Modalidade.EJA;
            var componenteCurricularId = 42;

            var entidade = new ModalidadeComponenteCurricular(modalidade, componenteCurricularId);

            Assert.Equal(componenteCurricularId, entidade.ComponenteCurricularId);
        }

        [Fact]
        public void Construtor_DeveInicializarComponenteCurricularComoNulo()
        {
            var entidade = new ModalidadeComponenteCurricular(Modalidade.EducacaoInfantil, 1);

            Assert.Null(entidade.ComponenteCurricular);
        }

        [Theory]
        [InlineData(Modalidade.EducacaoInfantil, 1)]
        [InlineData(Modalidade.EJA, 3)]
        [InlineData(Modalidade.CIEJA, 4)]
        [InlineData(Modalidade.Fundamental, 5)]
        [InlineData(Modalidade.Medio, 6)]
        [InlineData(Modalidade.CMCT, 7)]
        [InlineData(Modalidade.MOVA, 8)]
        [InlineData(Modalidade.ETEC, 9)]
        public void Construtor_DeveAceitarTodasAsModalidades(Modalidade modalidade, int componenteCurricularId)
        {
            var entidade = new ModalidadeComponenteCurricular(modalidade, componenteCurricularId);

            Assert.Equal(modalidade, entidade.ModalidadeId);
            Assert.Equal(componenteCurricularId, entidade.ComponenteCurricularId);
        }

        [Fact]
        public void Entidade_DeveHerdarDeEntidadeBase()
        {
            var entidade = new ModalidadeComponenteCurricular(Modalidade.Fundamental, 10);

            Assert.IsAssignableFrom<EntidadeBase>(entidade);
        }
    }
}
