using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario
{
    public class ObterQuestionariosUseCaseTeste
    {
        private readonly Mock<IRepositorioQuestionario> _questionarioRepositorioMock;
        private readonly ObterQuestionariosUseCase _useCase;

        public ObterQuestionariosUseCaseTeste()
        {
            _questionarioRepositorioMock = new Mock<IRepositorioQuestionario>();
            _useCase = new ObterQuestionariosUseCase(_questionarioRepositorioMock.Object);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_De_Questionarios_Com_Sucesso()
        {
            var questionarios = new List<Dominio.Entidades.Questionario.Questionario>
            {
                new Dominio.Entidades.Questionario.Questionario(
                    "Questionário 1",
                    TipoQuestionario.SondagemEscrita,
                    2024,
                    1,
                    1,
                    1,
                    1,
                    1
                )
                {
                    Id = 1,
                    CriadoEm = DateTime.UtcNow,
                    CriadoPor = "Sistema",
                    CriadoRF = "1234567"
                },
                new Dominio.Entidades.Questionario.Questionario(
                    "Questionário 2",
                    TipoQuestionario.SondagemLeitura,
                    2024,
                    2,
                    2,
                    2,
                    2,
                    2
                )
                {
                    Id = 2,
                    CriadoEm = DateTime.UtcNow,
                    CriadoPor = "Sistema",
                    CriadoRF = "7654321"
                }
            };

            _questionarioRepositorioMock
                .Setup(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(questionarios);

            var resultado = await _useCase.ExecutarAsync();

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            
            var primeiroQuestionario = resultado.First();
            Assert.Equal(1, primeiroQuestionario.Id);
            Assert.Equal("Questionário 1", primeiroQuestionario.Nome);
            Assert.Equal(TipoQuestionario.SondagemEscrita, primeiroQuestionario.Tipo);
            
            var segundoQuestionario = resultado.Last();
            Assert.Equal(2, segundoQuestionario.Id);
            Assert.Equal("Questionário 2", segundoQuestionario.Nome);
            Assert.Equal(TipoQuestionario.SondagemLeitura, segundoQuestionario.Tipo);
            
            _questionarioRepositorioMock.Verify(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Vazia_Quando_Nao_Houver_Questionarios()
        {
            var questionariosVazio = new List<Dominio.Entidades.Questionario.Questionario>();

            _questionarioRepositorioMock
                .Setup(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(questionariosVazio);

            var resultado = await _useCase.ExecutarAsync();

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
            _questionarioRepositorioMock.Verify(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Passar_Cancellation_Token_Para_Repositorio()
        {
            var cancellationToken = new CancellationToken();
            var questionarios = new List<Dominio.Entidades.Questionario.Questionario>();

            _questionarioRepositorioMock
                .Setup(x => x.ObterTodosAsync(cancellationToken))
                .ReturnsAsync(questionarios);

            await _useCase.ExecutarAsync(cancellationToken);

            _questionarioRepositorioMock.Verify(x => x.ObterTodosAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Deve_Mapear_Todos_Os_Campos_Dos_Questionarios()
        {
            var dataAtual = DateTime.UtcNow;
            var questionarios = new List<Dominio.Entidades.Questionario.Questionario>
            {
                new Dominio.Entidades.Questionario.Questionario(
                    "Questionário Completo",
                    TipoQuestionario.SondagemEscrita,
                    2024,
                    5,
                    6,
                    7,
                    8,
                    9
                )
                {
                    Id = 10,
                    CriadoEm = dataAtual,
                    CriadoPor = "Usuário Criação",
                    CriadoRF = "1111111",
                    AlteradoEm = dataAtual.AddDays(1),
                    AlteradoPor = "Usuário Alteração",
                    AlteradoRF = "2222222"
                }
            };

            _questionarioRepositorioMock
                .Setup(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(questionarios);

            var resultado = await _useCase.ExecutarAsync();

            Assert.NotNull(resultado);
            Assert.Single(resultado);
            
            var questionarioDto = resultado.First();
            Assert.Equal(10, questionarioDto.Id);
            Assert.Equal("Questionário Completo", questionarioDto.Nome);
            Assert.Equal(TipoQuestionario.SondagemEscrita, questionarioDto.Tipo);
            Assert.Equal(2024, questionarioDto.AnoLetivo);
            Assert.Equal(5, questionarioDto.ComponenteCurricularId);
            Assert.Equal(6, questionarioDto.ProficienciaId);
            Assert.Equal(7, questionarioDto.SondagemId);
            Assert.Equal(8, questionarioDto.ModalidadeId);
            Assert.Equal(9, questionarioDto.SerieAno);
            Assert.Equal(dataAtual, questionarioDto.CriadoEm);
            Assert.Equal("Usuário Criação", questionarioDto.CriadoPor);
            Assert.Equal("1111111", questionarioDto.CriadoRF);
            Assert.Equal(dataAtual.AddDays(1), questionarioDto.AlteradoEm);
            Assert.Equal("Usuário Alteração", questionarioDto.AlteradoPor);
            Assert.Equal("2222222", questionarioDto.AlteradoRF);
        }

        [Fact]
        public async Task Deve_Retornar_Multiplos_Questionarios_Com_Dados_Corretos()
        {
            var questionarios = new List<Dominio.Entidades.Questionario.Questionario>
            {
                new Dominio.Entidades.Questionario.Questionario(
                    "Matemática 1º Ano",
                    TipoQuestionario.SondagemEscrita,
                    2024,
                    1,
                    1,
                    1,
                    1,
                    1
                )
                {
                    Id = 1,
                    CriadoEm = DateTime.UtcNow,
                    CriadoPor = "Professor A",
                    CriadoRF = "1111111"
                },
                new Dominio.Entidades.Questionario.Questionario(
                    "Português 2º Ano",
                    TipoQuestionario.SondagemLeitura,
                    2024,
                    2,
                    2,
                    2,
                    2,
                    2
                )
                {
                    Id = 2,
                    CriadoEm = DateTime.UtcNow.AddDays(-1),
                    CriadoPor = "Professor B",
                    CriadoRF = "2222222"
                },
                new Dominio.Entidades.Questionario.Questionario(
                    "Ciências 3º Ano",
                    TipoQuestionario.SondagemEscrita,
                    2024,
                    3,
                    3,
                    3,
                    3,
                    3
                )
                {
                    Id = 3,
                    CriadoEm = DateTime.UtcNow.AddDays(-2),
                    CriadoPor = "Professor C",
                    CriadoRF = "3333333"
                }
            };

            _questionarioRepositorioMock
                .Setup(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(questionarios);

            var resultado = await _useCase.ExecutarAsync();

            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.Count());
            
            var listaResultado = resultado.ToList();
            
            Assert.Equal("Matemática 1º Ano", listaResultado[0].Nome);
            Assert.Equal("Professor A", listaResultado[0].CriadoPor);
            Assert.Equal(1, listaResultado[0].ComponenteCurricularId);
            
            Assert.Equal("Português 2º Ano", listaResultado[1].Nome);
            Assert.Equal("Professor B", listaResultado[1].CriadoPor);
            Assert.Equal(2, listaResultado[1].ComponenteCurricularId);
            
            Assert.Equal("Ciências 3º Ano", listaResultado[2].Nome);
            Assert.Equal("Professor C", listaResultado[2].CriadoPor);
            Assert.Equal(3, listaResultado[2].ComponenteCurricularId);
        }

        [Fact]
        public async Task Deve_Retornar_IEnumerable_De_QuestionarioDto()
        {
            var questionarios = new List<Dominio.Entidades.Questionario.Questionario>
            {
                new Dominio.Entidades.Questionario.Questionario(
                    "Questionário Teste",
                    TipoQuestionario.SondagemEscrita,
                    2024,
                    1,
                    1,
                    1,
                    1,
                    1
                )
                {
                    Id = 1
                }
            };

            _questionarioRepositorioMock
                .Setup(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(questionarios);

            var resultado = await _useCase.ExecutarAsync();

            Assert.NotNull(resultado);
            Assert.IsType<List<Infra.Dtos.Questionario.QuestionarioDto>>(resultado.ToList());
            Assert.All(resultado, item => Assert.IsType<Infra.Dtos.Questionario.QuestionarioDto>(item));
        }
    }
}
