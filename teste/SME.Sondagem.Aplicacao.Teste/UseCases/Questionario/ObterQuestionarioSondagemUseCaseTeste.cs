using Moq;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.UseCases.Questionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario
{
    public class ObterQuestionarioSondagemUseCaseTeste
    {
        private readonly Mock<IRepositorioElasticTurma> _repositorioTurmaMock;
        private readonly Mock<IRepositorioElasticAluno> _repositorioAlunoMock;
        private readonly Mock<IRepositorioOpcaoResposta> _repositorioOpcaoMock;
        private readonly Mock<IRepositorioRespostaAluno> _repositorioRespostaMock;
        private readonly Mock<IAlunoPapService> _alunoPapServiceMock;

        public ObterQuestionarioSondagemUseCaseTeste()
        {
            _repositorioTurmaMock = new Mock<IRepositorioElasticTurma>();
            _repositorioAlunoMock = new Mock<IRepositorioElasticAluno>();
            _repositorioOpcaoMock = new Mock<IRepositorioOpcaoResposta>();
            _repositorioRespostaMock = new Mock<IRepositorioRespostaAluno>();
            _alunoPapServiceMock = new Mock<IAlunoPapService>();
        }

        private ObterQuestionarioSondagemUseCase CriarUseCase()
            => new(
                _repositorioTurmaMock.Object,
                _repositorioAlunoMock.Object,
                _repositorioOpcaoMock.Object,
                _repositorioRespostaMock.Object,
                _alunoPapServiceMock.Object);

        [Fact]
        public async Task ObterQuestionarioSondagem_FiltroNulo_DeveLancarArgumentNullException()
        {
            var useCase = CriarUseCase();

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                useCase.ObterQuestionarioSondagem(null!, CancellationToken.None));
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_TurmaNaoLocalizada_DeveLancarErroInternoException()
        {
            var filtro = new FiltroQuestionario { TurmaId = 1, Modalidade = 1 };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TurmaElasticDto?)null);

            var useCase = CriarUseCase();

            var exception = await Assert.ThrowsAsync<ErroInternoException>(() =>
                useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));

            Assert.Equal("Turma não localizada", exception.Message);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_ModalidadeZero_DeveLancarRegraNegocioException()
        {
            var filtro = new FiltroQuestionario { TurmaId = 1, Modalidade = 0 };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto { Modalidade = 1, SerieEnsino = "1", AnoLetivo = 2024 });

            var useCase = CriarUseCase();

            var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
                useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));

            Assert.Equal("A modalidade é obrigatória no filtro", exception.Message);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_ProficienciaZero_DeveLancarRegraNegocioException()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 0
            };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto { Modalidade = 5, SerieEnsino = "1", AnoLetivo = 2024 });

            var useCase = CriarUseCase();

            var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
                useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));

            Assert.Equal("A proficiência é obrigatória no filtro", exception.Message);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_ModalidadeDiferenteDaTurma_DeveLancarRegraNegocioException()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 1
            };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto { Modalidade = 3, SerieEnsino = "1", AnoLetivo = 2024 });

            var useCase = CriarUseCase();

            var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
                useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));

            Assert.Equal("A modalidade do filtro não corresponde a esta turma", exception.Message);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_SerieEnsinoNaoInformada_DeveLancarErroInternoException()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 1
            };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto { Modalidade = 5, SerieEnsino = null, AnoLetivo = 2024 });

            var useCase = CriarUseCase();

            var exception = await Assert.ThrowsAsync<ErroInternoException>(() =>
                useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));

            Assert.Equal("Série de ensino da turma não informada", exception.Message);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_SerieEnsinoVazia_DeveLancarErroInternoException()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 1
            };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto { Modalidade = 5, SerieEnsino = "   ", AnoLetivo = 2024 });

            var useCase = CriarUseCase();

            var exception = await Assert.ThrowsAsync<ErroInternoException>(() =>
                useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));

            Assert.Equal("Série de ensino da turma não informada", exception.Message);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_AnoNaoCorrespondeTurma_DeveLancarRegraNegocioException()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 1
            };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto { Modalidade = 5, SerieEnsino = "2", AnoLetivo = 2024 });

            var useCase = CriarUseCase();

            var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
                useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));

            Assert.Equal("A série ano do filtro não corresponde a esta turma", exception.Message);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_SemAlunos_DeveLancarErroInternoException()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 1
            };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    Modalidade = 5,
                    SerieEnsino = "1",
                    AnoLetivo = 2024
                });

            _repositorioAlunoMock
                .Setup(x => x.ObterAlunosPorIdTurma(filtro.TurmaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<AlunoElasticDto>());

            var useCase = CriarUseCase();

            var exception = await Assert.ThrowsAsync<ErroInternoException>(() =>
                useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));

            Assert.Equal("Não há alunos cadastrados para a turma informada", exception.Message);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_AlunosNulo_DeveLancarErroInternoException()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 1
            };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    Modalidade = 5,
                    SerieEnsino = "1",
                    AnoLetivo = 2024
                });

            _repositorioAlunoMock
                .Setup(x => x.ObterAlunosPorIdTurma(filtro.TurmaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<AlunoElasticDto>?)null);

            var useCase = CriarUseCase();

            var exception = await Assert.ThrowsAsync<ErroInternoException>(() =>
                useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));

            Assert.Equal("Não há alunos cadastrados para a turma informada", exception.Message);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_SemOpcoesResposta_DeveLancarErroInternoException()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 1
            };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    Modalidade = 5,
                    SerieEnsino = "1",
                    AnoLetivo = 2024
                });

            _repositorioAlunoMock
                .Setup(x => x.ObterAlunosPorIdTurma(filtro.TurmaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new AlunoElasticDto
                    {
                        CodigoAluno = 10,
                        NumeroAlunoChamada = "1",
                        NomeAluno = "Aluno Teste",
                        PossuiDeficiencia = 0
                    }
                });

            _repositorioOpcaoMock
                .Setup(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>?)null);

            var useCase = CriarUseCase();

            var exception = await Assert.ThrowsAsync<ErroInternoException>(() =>
                useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));

            Assert.Equal("Não foi possível obter as opções de resposta", exception.Message);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_AnoInvalido_DeveLancarErroInternoException()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 5, 
                ProficienciaId = 1
            };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    Modalidade = 5,
                    SerieEnsino = "5",
                    AnoLetivo = 2024
                });

            var useCase = CriarUseCase();

            var exception = await Assert.ThrowsAsync<ErroInternoException>(() =>
                useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));

            Assert.Equal("Não há questionário para a série informada", exception.Message);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_ModalidadeInvalida_DeveLancarErroInternoException()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = 99, 
                Ano = 1,
                ProficienciaId = 1
            };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    Modalidade = 99,
                    SerieEnsino = "1",
                    AnoLetivo = 2024
                });

            var useCase = CriarUseCase();

            var exception = await Assert.ThrowsAsync<ErroInternoException>(() =>
                useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));

            Assert.Equal("Não há questionário para a modalidade informada", exception.Message);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_ModalidadeEJA_DeveRetornarQuestionario()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.EJA,
                Ano = 2,
                ProficienciaId = 1
            };

            ConfigurarMocksParaSucesso(filtro);

            var useCase = CriarUseCase();

            var result = await useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("Compreensão de textos", result.TituloTabelaRespostas);
            Assert.NotEmpty(result.Estudantes);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_CenarioCompleto_DeveRetornarQuestionarioComEstudantes()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 1
            };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    Modalidade = 5,
                    SerieEnsino = "1",
                    AnoLetivo = 2024
                });

            _repositorioAlunoMock
                .Setup(x => x.ObterAlunosPorIdTurma(filtro.TurmaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new AlunoElasticDto
                    {
                        CodigoAluno = 10,
                        NumeroAlunoChamada = "1",
                        NomeAluno = "Aluno Teste",
                        PossuiDeficiencia = 0
                    }
                });

            _repositorioOpcaoMock
                .Setup(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(1, "Sim", "S", "#FFF", "#000")
                });

            _alunoPapServiceMock
                .Setup(x => x.VerificarAlunosPossuemProgramaPapAsync(
                    It.IsAny<List<int>>(),
                    2024,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<int, bool> { { 10, true } });

            _repositorioRespostaMock
                .Setup(x => x.VerificarAlunosTemRespostaPorTipoQuestaoAsync(
                    It.IsAny<List<int>>(),
                    TipoQuestao.LinguaPortuguesaSegundaLingua,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<int, bool> { { 10, false } });

            _repositorioRespostaMock
                .Setup(x => x.ObterRespostasAlunosPorCiclosAsync(
                    It.IsAny<List<int>>(),
                    It.IsAny<List<int>>(),
                    filtro.ProficienciaId,
                    filtro.Ano,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<(int AlunoId, int CicloId), (int Id, int OpcaoRespostaId)>());

            var useCase = CriarUseCase();

            var result = await useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("Compreensão de textos", result.TituloTabelaRespostas);

            var estudante = Assert.Single(result.Estudantes);
            Assert.Equal(10, estudante.Codigo);
            Assert.Equal("1", estudante.NumeroAlunoChamada);
            Assert.Equal("Aluno Teste", estudante.Nome);
            Assert.True(estudante.Pap);
            Assert.False(estudante.LinguaPortuguesaSegundaLingua);
            Assert.False(estudante.PossuiDeficiencia);
            Assert.Equal(5, estudante.Coluna.Count()); 

            _repositorioTurmaMock.Verify(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()), Times.Once);
            _repositorioAlunoMock.Verify(x => x.ObterAlunosPorIdTurma(filtro.TurmaId, It.IsAny<CancellationToken>()), Times.Once);
            _repositorioOpcaoMock.Verify(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()), Times.Once);
            _alunoPapServiceMock.Verify(x => x.VerificarAlunosPossuemProgramaPapAsync(It.IsAny<List<int>>(), 2024, It.IsAny<CancellationToken>()), Times.Once);
            _repositorioRespostaMock.Verify(x => x.VerificarAlunosTemRespostaPorTipoQuestaoAsync(It.IsAny<List<int>>(), TipoQuestao.LinguaPortuguesaSegundaLingua, It.IsAny<CancellationToken>()), Times.Once);
            _repositorioRespostaMock.Verify(x => x.ObterRespostasAlunosPorCiclosAsync(It.IsAny<List<int>>(), It.IsAny<List<int>>(), filtro.ProficienciaId, filtro.Ano, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_ComMultiplosAlunos_DeveRetornarTodos()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 1
            };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    Modalidade = 5,
                    SerieEnsino = "1",
                    AnoLetivo = 2024
                });

            _repositorioAlunoMock
                .Setup(x => x.ObterAlunosPorIdTurma(filtro.TurmaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new AlunoElasticDto
                    {
                        CodigoAluno = 10,
                        NumeroAlunoChamada = "1",
                        NomeAluno = "Aluno 1",
                        PossuiDeficiencia = 1
                    },
                    new AlunoElasticDto
                    {
                        CodigoAluno = 20,
                        NumeroAlunoChamada = "2",
                        NomeAluno = "Aluno 2",
                        PossuiDeficiencia = 0
                    }
                });

            _repositorioOpcaoMock
                .Setup(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(1, "Sim", "S", "#FFF", "#000")
                });

            _alunoPapServiceMock
                .Setup(x => x.VerificarAlunosPossuemProgramaPapAsync(
                    It.IsAny<List<int>>(),
                    2024,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<int, bool> { { 10, true }, { 20, false } });

            _repositorioRespostaMock
                .Setup(x => x.VerificarAlunosTemRespostaPorTipoQuestaoAsync(
                    It.IsAny<List<int>>(),
                    TipoQuestao.LinguaPortuguesaSegundaLingua,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<int, bool> { { 10, true }, { 20, false } });

            _repositorioRespostaMock
                .Setup(x => x.ObterRespostasAlunosPorCiclosAsync(
                    It.IsAny<List<int>>(),
                    It.IsAny<List<int>>(),
                    filtro.ProficienciaId,
                    filtro.Ano,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<(int AlunoId, int CicloId), (int Id, int OpcaoRespostaId)>());

            var useCase = CriarUseCase();

            var result = await useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Estudantes.Count());

            var aluno1 = result.Estudantes.First(e => e.Codigo == 10);
            Assert.Equal("Aluno 1", aluno1.Nome);
            Assert.True(aluno1.PossuiDeficiencia);
            Assert.True(aluno1.Pap);
            Assert.True(aluno1.LinguaPortuguesaSegundaLingua);

            var aluno2 = result.Estudantes.First(e => e.Codigo == 20);
            Assert.Equal("Aluno 2", aluno2.Nome);
            Assert.False(aluno2.PossuiDeficiencia);
            Assert.False(aluno2.Pap);
            Assert.False(aluno2.LinguaPortuguesaSegundaLingua);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_ComRespostasExistentes_DeveMapeaCorreto()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 4
            };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    Modalidade = 5,
                    SerieEnsino = "1",
                    AnoLetivo = 2024
                });

            _repositorioAlunoMock
                .Setup(x => x.ObterAlunosPorIdTurma(filtro.TurmaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new AlunoElasticDto
                    {
                        CodigoAluno = 10,
                        NumeroAlunoChamada = "1",
                        NomeAluno = "Aluno Teste",
                        PossuiDeficiencia = 0
                    }
                });

            _repositorioOpcaoMock
                .Setup(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(1, "Sim", "S", "#FFF", "#000")
                });

            _alunoPapServiceMock
                .Setup(x => x.VerificarAlunosPossuemProgramaPapAsync(
                    It.IsAny<List<int>>(),
                    2024,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<int, bool>());

            _repositorioRespostaMock
                .Setup(x => x.VerificarAlunosTemRespostaPorTipoQuestaoAsync(
                    It.IsAny<List<int>>(),
                    TipoQuestao.LinguaPortuguesaSegundaLingua,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<int, bool>());

            var respostas = new Dictionary<(int AlunoId, int CicloId), (int Id, int OpcaoRespostaId)>
            {
                { (10, 1), (100, 1) },
                { (10, 2), (101, 2) }
            };

            _repositorioRespostaMock
                .Setup(x => x.ObterRespostasAlunosPorCiclosAsync(
                    It.IsAny<List<int>>(),
                    It.IsAny<List<int>>(),
                    filtro.ProficienciaId,
                    filtro.Ano,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(respostas);

            var useCase = CriarUseCase();

            var result = await useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

            Assert.NotNull(result);
            var estudante = Assert.Single(result.Estudantes);

            var coluna1 = estudante.Coluna.First(c => c.IdCiclo == 1);
            Assert.NotNull(coluna1.Resposta);
            Assert.Equal(100, coluna1.Resposta.Id);
            Assert.Equal(1, coluna1.Resposta.OpcaoRespostaId);

            var coluna2 = estudante.Coluna.First(c => c.IdCiclo == 2);
            Assert.NotNull(coluna2.Resposta);
            Assert.Equal(101, coluna2.Resposta.Id);
            Assert.Equal(2, coluna2.Resposta.OpcaoRespostaId);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_AlunoSemNome_DeveRetornarStringVazia()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 4
            };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    Modalidade = 5,
                    SerieEnsino = "1",
                    AnoLetivo = 2024
                });

            _repositorioAlunoMock
                .Setup(x => x.ObterAlunosPorIdTurma(filtro.TurmaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new AlunoElasticDto
                    {
                        CodigoAluno = 10,
                        NumeroAlunoChamada = "1",
                        NomeAluno = null,
                        PossuiDeficiencia = 0
                    }
                });

            ConfigurarMocksBasicosParaSucesso(filtro);

            var useCase = CriarUseCase();

            var result = await useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

            var estudante = Assert.Single(result.Estudantes);
            Assert.Equal(string.Empty, estudante.Nome);
        }

        [Theory]
        [InlineData(1, "Compreensão de textos")]
        [InlineData(2, "Compreensão de textos")]
        public async Task ObterQuestionarioSondagem_ProficienciaEscrita_DeveRetornarTituloCorreto(int ano, string tituloEsperado)
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = ano,
                ProficienciaId = 1
            };

            ConfigurarMocksParaSucesso(filtro);

            var useCase = CriarUseCase();

            var result = await useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

            Assert.Equal(tituloEsperado, result.TituloTabelaRespostas);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task ObterQuestionarioSondagem_ProficienciaLeitura_DeveRetornarCompreensaoDeTextos(int ano)
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = ano,
                ProficienciaId = 1
            };

            ConfigurarMocksParaSucesso(filtro);

            var useCase = CriarUseCase();

            var result = await useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

            Assert.Equal("Compreensão de textos", result.TituloTabelaRespostas);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_ProficienciaInvalida_DeveRetornarStringVazia()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 999 
            };

            ConfigurarMocksParaSucesso(filtro);

            var useCase = CriarUseCase();

            var result = await useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

            Assert.Equal(string.Empty, result.TituloTabelaRespostas);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_DeveCriar5Ciclos()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 4
            };

            ConfigurarMocksParaSucesso(filtro);

            var useCase = CriarUseCase();

            var result = await useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

            var estudante = Assert.Single(result.Estudantes);
            Assert.Equal(5, estudante.Coluna.Count());

            Assert.Contains(estudante.Coluna, c => c.IdCiclo == 1 && c.DescricaoColuna == "Sondagem inicial");
            Assert.Contains(estudante.Coluna, c => c.IdCiclo == 2 && c.DescricaoColuna == "1º bimestre");
            Assert.Contains(estudante.Coluna, c => c.IdCiclo == 3 && c.DescricaoColuna == "2º bimestre");
            Assert.Contains(estudante.Coluna, c => c.IdCiclo == 4 && c.DescricaoColuna == "3º bimestre");
            Assert.Contains(estudante.Coluna, c => c.IdCiclo == 5 && c.DescricaoColuna == "4º bimestre");
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_DeveMarcarPrimeiraColunaComoAtiva()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 4
            };

            ConfigurarMocksParaSucesso(filtro);

            var useCase = CriarUseCase();

            var result = await useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

            var estudante = Assert.Single(result.Estudantes);
            var colunaAtiva = estudante.Coluna.FirstOrDefault(c => c.PeriodoBimestreAtivo);

            Assert.NotNull(colunaAtiva);
            Assert.Equal(1, colunaAtiva.IdCiclo);
            Assert.Equal("Sondagem inicial", colunaAtiva.DescricaoColuna);
        }

        [Fact]
        public async Task ObterQuestionarioSondagem_OpcaoRespostaComValoresNulos_DeveTratarCorretamente()
        {
            var filtro = new FiltroQuestionario
            {
                TurmaId = 1,
                Modalidade = (int)Modalidade.Fundamental,
                Ano = 1,
                ProficienciaId = 4
            };

            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    Modalidade = 5,
                    SerieEnsino = "1",
                    AnoLetivo = 2024
                });

            _repositorioAlunoMock
                .Setup(x => x.ObterAlunosPorIdTurma(filtro.TurmaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new AlunoElasticDto
                    {
                        CodigoAluno = 10,
                        NumeroAlunoChamada = "1",
                        NomeAluno = "Aluno Teste",
                        PossuiDeficiencia = 0
                    }
                });

            _repositorioOpcaoMock
                .Setup(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(0, null, null, null, null)
                });

            _alunoPapServiceMock
                .Setup(x => x.VerificarAlunosPossuemProgramaPapAsync(
                    It.IsAny<List<int>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<int, bool>());

            _repositorioRespostaMock
                .Setup(x => x.VerificarAlunosTemRespostaPorTipoQuestaoAsync(
                    It.IsAny<List<int>>(),
                    It.IsAny<TipoQuestao>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<int, bool>());

            _repositorioRespostaMock
                .Setup(x => x.ObterRespostasAlunosPorCiclosAsync(
                    It.IsAny<List<int>>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<(int AlunoId, int CicloId), (int Id, int OpcaoRespostaId)>());

            var useCase = CriarUseCase();

            var result = await useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

            Assert.NotNull(result);
            var estudante = Assert.Single(result.Estudantes);
            var primeiraColuna = estudante.Coluna.First();

            var opcaoResposta = Assert.Single(primeiraColuna.OpcaoResposta);
            Assert.Equal(string.Empty, opcaoResposta.DescricaoOpcaoResposta);
            Assert.Equal(string.Empty, opcaoResposta.Legenda);
            Assert.Equal(string.Empty, opcaoResposta.CorFundo);
            Assert.Equal(string.Empty, opcaoResposta.CorTexto);
        }

        private void ConfigurarMocksBasicosParaSucesso(FiltroQuestionario filtro)
        {
            _repositorioOpcaoMock
                .Setup(x => x.ObterTodosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(1, "Sim", "S", "#FFF", "#000")
                });

            _alunoPapServiceMock
                .Setup(x => x.VerificarAlunosPossuemProgramaPapAsync(
                    It.IsAny<List<int>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<int, bool>());

            _repositorioRespostaMock
                .Setup(x => x.VerificarAlunosTemRespostaPorTipoQuestaoAsync(
                    It.IsAny<List<int>>(),
                    It.IsAny<TipoQuestao>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<int, bool>());

            _repositorioRespostaMock
                .Setup(x => x.ObterRespostasAlunosPorCiclosAsync(
                    It.IsAny<List<int>>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<(int AlunoId, int CicloId), (int Id, int OpcaoRespostaId)>());
        }

        private void ConfigurarMocksParaSucesso(FiltroQuestionario filtro)
        {
            _repositorioTurmaMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    Modalidade = filtro.Modalidade,
                    SerieEnsino = filtro.Ano.ToString(),
                    AnoLetivo = 2024
                });

            _repositorioAlunoMock
                .Setup(x => x.ObterAlunosPorIdTurma(filtro.TurmaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new AlunoElasticDto
                    {
                        CodigoAluno = 10,
                        NumeroAlunoChamada = "1",
                        NomeAluno = "Aluno Teste",
                        PossuiDeficiencia = 0
                    }
                });

            ConfigurarMocksBasicosParaSucesso(filtro);
        }
    }
}