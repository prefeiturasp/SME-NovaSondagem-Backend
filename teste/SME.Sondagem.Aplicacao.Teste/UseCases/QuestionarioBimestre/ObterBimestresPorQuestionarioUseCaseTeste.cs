using Moq;
using SME.Sondagem.Aplicacao.UseCases.QuestionarioBimestre;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.QuestionarioBimestre
{
    public class ObterBimestresPorQuestionarioUseCaseTeste
    {
        private readonly Mock<IRepositorioQuestionarioBimestre> _repositorioMock;
        private readonly ObterBimestresPorQuestionarioUseCase _useCase;

        public ObterBimestresPorQuestionarioUseCaseTeste()
        {
            _repositorioMock = new Mock<IRepositorioQuestionarioBimestre>();
            _useCase = new ObterBimestresPorQuestionarioUseCase(_repositorioMock.Object);
        }

        private static Dominio.Entidades.Questionario.QuestionarioBimestre CriarVinculoComBimestre(
            int questionarioId,
            int bimestreId,
            string descricaoBimestre,
            int codBimestreEol)
        {
            var vinculo = new Dominio.Entidades.Questionario.QuestionarioBimestre(questionarioId, bimestreId);

            var bimestre = new Dominio.Entidades.Bimestre(codBimestreEol, descricaoBimestre);

            var propriedadeId = typeof(Dominio.Entidades.Bimestre).GetProperty("Id");
            propriedadeId?.SetValue(bimestre, bimestreId);

            var propriedadeBimestre = typeof(Dominio.Entidades.Questionario.QuestionarioBimestre)
                .GetProperty("Bimestre");

            propriedadeBimestre?.SetValue(vinculo, bimestre);

            return vinculo;
        }

        [Fact]
        public async Task Deve_Retornar_Vinculos_Do_Questionario()
        {
            var questionarioId = 10;
            var vinculos = new List<Dominio.Entidades.Questionario.QuestionarioBimestre>
            {
                new Dominio.Entidades.Questionario.QuestionarioBimestre(questionarioId, 1),
                new Dominio.Entidades.Questionario.QuestionarioBimestre(questionarioId, 2)
            };

            _repositorioMock
                .Setup(x => x.ObterPorQuestionarioIdAsync(questionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vinculos);

            var resultado = (await _useCase.ExecutarAsync(questionarioId)).ToList();

            Assert.Equal(2, resultado.Count);
            Assert.All(resultado, r => Assert.Equal(questionarioId, r.QuestionarioId));
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Vazia_Quando_Questionario_Nao_Tem_Vinculos()
        {
            var questionarioId = 999;

            _repositorioMock
                .Setup(x => x.ObterPorQuestionarioIdAsync(questionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Dominio.Entidades.Questionario.QuestionarioBimestre>());

            var resultado = (await _useCase.ExecutarAsync(questionarioId)).ToList();

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task Deve_Mapear_Dados_Do_Bimestre()
        {
            var questionarioId = 5;

            var vinculo = CriarVinculoComBimestre(
                questionarioId,
                1,
                "1º Bimestre",
                1);

            _repositorioMock
                .Setup(x => x.ObterPorQuestionarioIdAsync(questionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Dominio.Entidades.Questionario.QuestionarioBimestre> { vinculo });

            var resultado = (await _useCase.ExecutarAsync(questionarioId)).First();

            Assert.Equal("1º Bimestre", resultado.DescricaoBimestre);
            Assert.Equal(1, resultado.CodBimestreEnsinoEol);
        }

        [Fact]
        public async Task Deve_Passar_CancellationToken_Para_Repositorio()
        {
            var questionarioId = 1;
            var cancellationToken = new CancellationToken();

            _repositorioMock
                .Setup(x => x.ObterPorQuestionarioIdAsync(questionarioId, cancellationToken))
                .ReturnsAsync(new List<Dominio.Entidades.Questionario.QuestionarioBimestre>());

            await _useCase.ExecutarAsync(questionarioId, cancellationToken);

            _repositorioMock.Verify(x => x.ObterPorQuestionarioIdAsync(questionarioId, cancellationToken), Times.Once);
        }
    }
}