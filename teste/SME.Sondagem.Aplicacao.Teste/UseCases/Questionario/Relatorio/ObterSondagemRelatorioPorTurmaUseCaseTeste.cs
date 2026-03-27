using Moq;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using SME.Sondagem.Infrastructure.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioPorTurmaUseCaseTeste
{
    private readonly Mock<IRepositorioElasticTurma> _mockRepositorioElasticTurma;
    private readonly Mock<IRepositorioElasticAluno> _mockRepositorioElasticAluno;
    private readonly Mock<IRepositorioRespostaAluno> _mockRepositorioRespostaAluno;
    private readonly Mock<IRepositorioSondagem> _mockRepositorioSondagem;
    private readonly Mock<IRepositorioQuestao> _mockRepositorioQuestao;
    private readonly Mock<IRepositorioBimestre> _mockRepositorioBimestre;
    private readonly Mock<IAlunoPapService> _mockAlunoPapService;
    private readonly Mock<IControleAcessoService> _mockControleAcessoService;
    private readonly Mock<IAlunoTurmaService> _mockAlunoTurmaService;
    private readonly Mock<IServicoUsuario> _mockServicoUsuario;
    private readonly ObterSondagemRelatorioPorTurmaUseCase _useCase;
    private readonly Mock<IRepositorioProficiencia> _repositorioProficiencia;
    private readonly Mock<IRepositorioComponenteCurricular> _componenteCurricular;


    public ObterSondagemRelatorioPorTurmaUseCaseTeste()
    {
        _mockRepositorioElasticTurma = new Mock<IRepositorioElasticTurma>();
        _mockRepositorioElasticAluno = new Mock<IRepositorioElasticAluno>();
        _mockRepositorioRespostaAluno = new Mock<IRepositorioRespostaAluno>();
        _mockRepositorioSondagem = new Mock<IRepositorioSondagem>();
        _mockRepositorioQuestao = new Mock<IRepositorioQuestao>();
        _mockRepositorioBimestre = new Mock<IRepositorioBimestre>();
        _mockAlunoPapService = new Mock<IAlunoPapService>();
        _mockControleAcessoService = new Mock<IControleAcessoService>();
        _mockAlunoTurmaService = new Mock<IAlunoTurmaService>();
        _mockServicoUsuario = new Mock<IServicoUsuario>();
        _repositorioProficiencia = new Mock<IRepositorioProficiencia>();
        _componenteCurricular = new Mock<IRepositorioComponenteCurricular>();


        var repositoriosElastic = new RepositoriosElastic(
            _mockRepositorioElasticTurma.Object,
            _mockRepositorioElasticAluno.Object
        );

        var repositoriosSondagem = new RepositoriosSondagem(
            _mockRepositorioSondagem.Object,
            _mockRepositorioQuestao.Object,
            _mockRepositorioRespostaAluno.Object,
            _mockRepositorioBimestre.Object,
                        _componenteCurricular.Object,
            _repositorioProficiencia.Object
        );

        _useCase = new ObterSondagemRelatorioPorTurmaUseCase(
            repositoriosElastic,
            repositoriosSondagem,
            _mockAlunoPapService.Object,
            _mockAlunoTurmaService.Object,
            _mockControleAcessoService.Object,
            _mockServicoUsuario.Object

        );
    }

    #region Testes do Construtor

    [Fact]
    public void Construtor_DeveLancarArgumentNullException_QuandoAlunoTurmaServiceForNulo()
    {
        var repositoriosElastic = new RepositoriosElastic(
            _mockRepositorioElasticTurma.Object,
            _mockRepositorioElasticAluno.Object
        );

        var repositoriosSondagem = new RepositoriosSondagem(
            _mockRepositorioSondagem.Object,
            _mockRepositorioQuestao.Object,
            _mockRepositorioRespostaAluno.Object,
            _mockRepositorioBimestre.Object,
                        _componenteCurricular.Object,
            _repositorioProficiencia.Object
        );

        Assert.Throws<ArgumentNullException>(() => new ObterSondagemRelatorioPorTurmaUseCase(
            repositoriosElastic,
            repositoriosSondagem,
            _mockAlunoPapService.Object,
            null!,
            _mockControleAcessoService.Object,
            _mockServicoUsuario.Object 
        ))
;
    }

    #endregion

    #region Testes de ValidarAnoRelatorio

    [Fact]
    public async Task ObterSondagemRelatorio_DeveLancarRegraNegocioException_QuandoTurmaNaoForLocalizada()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };

        _mockRepositorioElasticTurma
            .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TurmaElasticDto?)null);

        var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None));

        Assert.Equal("Turma não localizada", exception.Message);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveLancarRegraNegocioException_QuandoAnoLetivoDaTurmaForMenorQue2025()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 };

        _mockRepositorioElasticTurma
            .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(turma);

        var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None));

        Assert.Equal("Relatórios só podem ser extraídos para anos letivos a partir de 2025", exception.Message);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveLancarRegraNegocioException_QuandoAnoLetivoDaTurmaForIgualA2024()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 };

        _mockRepositorioElasticTurma
            .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(turma);

        var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None));

        Assert.Equal(400, exception.StatusCode);
    }

    [Theory]
    [InlineData(2025)]
    [InlineData(2026)]
    [InlineData(2030)]
    public async Task ObterSondagemRelatorio_NaoDeveLancarExcecaoDeAnoLetivo_QuandoAnoLetivoForIgualOuMaiorQue2025(int anoLetivo)
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = anoLetivo };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();
        var dadosAlunos = CriarDadosAlunosPorTurmaMock();

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos, dadosAlunos);

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        Assert.NotNull(resultado);
    }

    #endregion

    #region Testes de ObterSondagemRelatorio (fluxo principal)

    [Fact]
    public async Task ObterSondagemRelatorio_DeveRetornarRelatorioComSucesso_QuandoDadosForemValidos()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2025 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();
        var dadosAlunos = CriarDadosAlunosPorTurmaMock();

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos, dadosAlunos);

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        Assert.NotNull(resultado);
        Assert.IsType<QuestionarioSondagemRelatorioDto>(resultado);
        Assert.NotNull(resultado.Estudantes);
        Assert.Equal("Questão Teste", resultado.TituloTabelaRespostas);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveRetornarEstudantesOrdenadosPorNome()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2025 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();
        var dadosAlunos = CriarDadosAlunosPorTurmaMock();

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos, dadosAlunos);

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        var estudantes = resultado.Estudantes!.ToList();
        Assert.Equal(2, estudantes.Count);
        Assert.Equal("João Silva", estudantes[0].Nome);
        Assert.Equal("Maria Santos", estudantes[1].Nome);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DevePreencherRacaEGeneroNosEstudantes_QuandoDadosAlunoTurmaExistir()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2025 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();

        var dadosAlunos = new List<DadosAlunoPorTurmaDto>
        {
            new DadosAlunoPorTurmaDto { CodigoAluno = 1001, NomeAluno = "João Silva", Raca = "PARDA", Sexo = "M" },
            new DadosAlunoPorTurmaDto { CodigoAluno = 1002, NomeAluno = "Maria Santos", Raca = "BRANCA", Sexo = "F" }
        };

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos, dadosAlunos);

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        var estudantes = resultado.Estudantes!.ToList();
        var joao = estudantes.First(e => e.Codigo == 1001);
        var maria = estudantes.First(e => e.Codigo == 1002);

        Assert.Equal("Parda", joao.Raca);
        Assert.Equal("Masculino", joao.Genero);
        Assert.Equal("Branca", maria.Raca);
        Assert.Equal("Feminino", maria.Genero);
    }

    #endregion

    #region Testes de ConverterCodigoGeneroParaDescricao

    [Theory]
    [InlineData("M", "Masculino")]
    [InlineData("m", "Masculino")]
    [InlineData("F", "Feminino")]
    [InlineData("f", "Feminino")]
    public async Task ObterSondagemRelatorio_DeveConverterCodigoGeneroCorretamente(string codigoGenero, string descricaoEsperada)
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2025 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = new List<AlunoElasticDto>
        {
            new AlunoElasticDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", NumeroAlunoChamada = "1", PossuiDeficiencia = 0 }
        };

        var dadosAlunos = new List<DadosAlunoPorTurmaDto>
        {
            new DadosAlunoPorTurmaDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", Sexo = codigoGenero, Raca = string.Empty }
        };

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos, dadosAlunos);

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        var estudante = resultado.Estudantes!.First();
        Assert.Equal(descricaoEsperada, estudante.Genero);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveRetornarCodigoGeneroOriginal_QuandoCodigoNaoForReconhecido()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2025 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = new List<AlunoElasticDto>
        {
            new AlunoElasticDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", NumeroAlunoChamada = "1", PossuiDeficiencia = 0 }
        };

        var dadosAlunos = new List<DadosAlunoPorTurmaDto>
        {
            new DadosAlunoPorTurmaDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", Sexo = "X", Raca = string.Empty }
        };

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos, dadosAlunos);

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        var estudante = resultado.Estudantes!.First();
        Assert.Equal("X", estudante.Genero);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveRetornarStringVazia_QuandoCodigoGeneroForNulo()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2025 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = new List<AlunoElasticDto>
        {
            new AlunoElasticDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", NumeroAlunoChamada = "1", PossuiDeficiencia = 0 }
        };

        var dadosAlunos = new List<DadosAlunoPorTurmaDto>
        {
            new DadosAlunoPorTurmaDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", Sexo = null!, Raca = string.Empty }
        };

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos, dadosAlunos);

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        var estudante = resultado.Estudantes!.First();
        Assert.Equal(string.Empty, estudante.Genero);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveRetornarStringVazia_QuandoCodigoGeneroForVazio()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2025 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = new List<AlunoElasticDto>
        {
            new AlunoElasticDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", NumeroAlunoChamada = "1", PossuiDeficiencia = 0 }
        };

        var dadosAlunos = new List<DadosAlunoPorTurmaDto>
        {
            new DadosAlunoPorTurmaDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", Sexo = "   ", Raca = string.Empty }
        };

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos, dadosAlunos);

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        var estudante = resultado.Estudantes!.First();
        Assert.Equal(string.Empty, estudante.Genero);
    }

    #endregion

    #region Testes de ConverterCodigoRacaParaDescricao

    [Theory]
    [InlineData("BRANCA", "Branca")]
    [InlineData("branca", "Branca")]
    [InlineData("PRETA", "Preta")]
    [InlineData("preta", "Preta")]
    [InlineData("PARDA", "Parda")]
    [InlineData("parda", "Parda")]
    [InlineData("AMARELA", "Amarela")]
    [InlineData("amarela", "Amarela")]
    [InlineData("INDIGENA", "Indígena")]
    [InlineData("indigena", "Indígena")]
    [InlineData("INDÍGENA", "Indígena")]
    [InlineData("indígena", "Indígena")]
    public async Task ObterSondagemRelatorio_DeveConverterCodigoRacaCorretamente(string codigoRaca, string descricaoEsperada)
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2025 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = new List<AlunoElasticDto>
        {
            new AlunoElasticDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", NumeroAlunoChamada = "1", PossuiDeficiencia = 0 }
        };

        var dadosAlunos = new List<DadosAlunoPorTurmaDto>
        {
            new DadosAlunoPorTurmaDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", Sexo = string.Empty, Raca = codigoRaca }
        };

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos, dadosAlunos);

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        var estudante = resultado.Estudantes!.First();
        Assert.Equal(descricaoEsperada, estudante.Raca);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveRetornarStringVazia_QuandoCodigoRacaForNulo()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2025 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = new List<AlunoElasticDto>
        {
            new AlunoElasticDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", NumeroAlunoChamada = "1", PossuiDeficiencia = 0 }
        };

        var dadosAlunos = new List<DadosAlunoPorTurmaDto>
        {
            new DadosAlunoPorTurmaDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", Sexo = string.Empty, Raca = null! }
        };

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos, dadosAlunos);

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        var estudante = resultado.Estudantes!.First();
        Assert.Equal(string.Empty, estudante.Raca);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveRetornarStringVazia_QuandoCodigoRacaForVazio()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2025 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = new List<AlunoElasticDto>
        {
            new AlunoElasticDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", NumeroAlunoChamada = "1", PossuiDeficiencia = 0 }
        };

        var dadosAlunos = new List<DadosAlunoPorTurmaDto>
        {
            new DadosAlunoPorTurmaDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", Sexo = string.Empty, Raca = "   " }
        };

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos, dadosAlunos);

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        var estudante = resultado.Estudantes!.First();
        Assert.Equal(string.Empty, estudante.Raca);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveCapitalizarRacaDesconhecida_QuandoCodigoRacaNaoForReconhecido()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2025 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = new List<AlunoElasticDto>
        {
            new AlunoElasticDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", NumeroAlunoChamada = "1", PossuiDeficiencia = 0 }
        };

        var dadosAlunos = new List<DadosAlunoPorTurmaDto>
        {
            new DadosAlunoPorTurmaDto { CodigoAluno = 1001, NomeAluno = "Aluno Teste", Sexo = string.Empty, Raca = "CABOCLA" }
        };

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos, dadosAlunos);

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        var estudante = resultado.Estudantes!.First();
        Assert.Equal("Cabocla", estudante.Raca);
    }

    #endregion

    #region Testes de ObterDadosAlunos

    [Fact]
    public async Task ObterSondagemRelatorio_DevePreencherAlunosComPap_QuandoServicoRetornarDados()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2025 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();
        var dadosAlunos = CriarDadosAlunosPorTurmaMock();

        ConfigurarMocksBase(filtro, turma, sondagem, questoes);

        _mockRepositorioElasticAluno
            .Setup(x => x.ObterAlunosPorIdTurma(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(alunos);

        _mockAlunoTurmaService
            .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dadosAlunos);

        var alunosComPap = new Dictionary<int, bool> { { 1001, true }, { 1002, false } };
        _mockAlunoPapService
            .Setup(x => x.VerificarAlunosPossuemProgramaPapAsync(
                It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(alunosComPap);

        _mockRepositorioRespostaAluno
            .Setup(x => x.VerificarAlunosPossuiLinguaPortuguesaAsync(
                It.IsAny<List<int>>(), It.IsAny<Dominio.Entidades.Questionario.Questao>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, bool>());

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasAlunosPorQuestoesAsync(
                It.IsAny<List<long>>(), It.IsAny<List<long>>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<(long, long, int?), RespostaAluno>());

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        Assert.True(resultado.Estudantes!.First(e => e.Codigo == 1001).Pap);
        Assert.False(resultado.Estudantes!.First(e => e.Codigo == 1002).Pap);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DevePreencherAlunosComLinguaPortuguesa_QuandoServicoRetornarDados()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2025 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();
        var dadosAlunos = CriarDadosAlunosPorTurmaMock();

        ConfigurarMocksBase(filtro, turma, sondagem, questoes);

        _mockRepositorioElasticAluno
            .Setup(x => x.ObterAlunosPorIdTurma(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(alunos);

        _mockAlunoTurmaService
            .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dadosAlunos);

        _mockAlunoPapService
            .Setup(x => x.VerificarAlunosPossuemProgramaPapAsync(
                It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, bool>());

        var alunosComLingua = new Dictionary<int, bool> { { 1001, true }, { 1002, false } };
        _mockRepositorioRespostaAluno
            .Setup(x => x.VerificarAlunosPossuiLinguaPortuguesaAsync(
                It.IsAny<List<int>>(), It.IsAny<Dominio.Entidades.Questionario.Questao>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(alunosComLingua);

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasAlunosPorQuestoesAsync(
                It.IsAny<List<long>>(), It.IsAny<List<long>>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<(long, long, int?), RespostaAluno>());

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        Assert.True(resultado.Estudantes!.First(e => e.Codigo == 1001).LinguaPortuguesaSegundaLingua);
        Assert.False(resultado.Estudantes!.First(e => e.Codigo == 1002).LinguaPortuguesaSegundaLingua);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveChamarInformacoesAlunosPorTurmaComTurmaIdCorreto()
    {
        var turmaId = 42;
        var filtro = new FiltroQuestionario { TurmaId = turmaId, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2025 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();
        var dadosAlunos = CriarDadosAlunosPorTurmaMock();

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos, dadosAlunos);

        await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        _mockAlunoTurmaService.Verify(
            x => x.InformacoesAlunosPorTurma(turmaId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveGerarDicionarioRacaGeneroVazio_QuandoAlunoTurmaServiceRetornarListaVazia()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, Ano = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2025 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos, new List<DadosAlunoPorTurmaDto>());

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        Assert.NotNull(resultado);
        var estudantes = resultado.Estudantes!.ToList();
        Assert.All(estudantes, e =>
        {
            Assert.Empty(e.Raca);
            Assert.Empty(e.Genero);
        });
    }

    #endregion

    #region Métodos Auxiliares

    private static Dominio.Entidades.Sondagem.Sondagem CriarSondagemMock()
    {
        var sondagem = new Dominio.Entidades.Sondagem.Sondagem("Sondagem 2025", DateTime.Now.AddMonths(-1));

        var bimestre = new Dominio.Entidades.Bimestre(1, "1º Bimestre");
        var periodo = new Dominio.Entidades.Sondagem.SondagemPeriodoBimestre(
            1, 1, DateTime.Now.AddDays(-10), DateTime.Now.AddDays(10));

        sondagem.GetType().GetProperty("PeriodosBimestre")!
            .SetValue(sondagem, new List<Dominio.Entidades.Sondagem.SondagemPeriodoBimestre> { periodo });

        periodo.GetType().GetProperty("Bimestre")!.SetValue(periodo, bimestre);

        return sondagem;
    }

    private static List<Dominio.Entidades.Questionario.Questao> CriarQuestoesMock()
    {
        var questao = new Dominio.Entidades.Questionario.Questao(
            1, 1, "Questão Teste", "Observação", true, TipoQuestao.Combo, "", false, 1);

        var opcaoResposta1 = new Dominio.Entidades.Questionario.OpcaoResposta(1, "Opção 1", "A", "#FFFFFF", "#000000");
        var opcaoResposta2 = new Dominio.Entidades.Questionario.OpcaoResposta(1, "Opção 2", "B", "#FFFFFF", "#000000");

        var questaoOpcao1 = new Dominio.Entidades.Questionario.QuestaoOpcaoResposta(1, 1, 1);
        var questaoOpcao2 = new Dominio.Entidades.Questionario.QuestaoOpcaoResposta(1, 2, 2);

        questaoOpcao1.GetType().BaseType!.GetProperty("Id")!.SetValue(questaoOpcao1, 1);
        questaoOpcao2.GetType().BaseType!.GetProperty("Id")!.SetValue(questaoOpcao2, 2);

        questaoOpcao1.GetType().GetProperty("OpcaoResposta")!.SetValue(questaoOpcao1, opcaoResposta1);
        questaoOpcao2.GetType().GetProperty("OpcaoResposta")!.SetValue(questaoOpcao2, opcaoResposta2);

        questao.GetType().GetProperty("QuestaoOpcoes")!
            .SetValue(questao, new List<Dominio.Entidades.Questionario.QuestaoOpcaoResposta> { questaoOpcao1, questaoOpcao2 });

        return new List<Dominio.Entidades.Questionario.Questao> { questao };
    }

    private static List<AlunoElasticDto> CriarAlunosMock()
    {
        return new List<AlunoElasticDto>
        {
            new AlunoElasticDto { CodigoAluno = 1001, NumeroAlunoChamada = "1", NomeAluno = "João Silva", PossuiDeficiencia = 0, CodigoSituacaoMatricula = (int)SituacaoMatriculaAluno.Ativo, DataSituacao = DateTime.Today.AddDays(-30) },
            new AlunoElasticDto { CodigoAluno = 1002, NumeroAlunoChamada = "2", NomeAluno = "Maria Santos", PossuiDeficiencia = 0, CodigoSituacaoMatricula = (int)SituacaoMatriculaAluno.Ativo, DataSituacao = DateTime.Today.AddDays(-30) }
        };
    }

    private static List<DadosAlunoPorTurmaDto> CriarDadosAlunosPorTurmaMock()
    {
        return new List<DadosAlunoPorTurmaDto>
        {
            new DadosAlunoPorTurmaDto { CodigoAluno = 1001, NomeAluno = "João Silva", Sexo = "M", Raca = "PARDA" },
            new DadosAlunoPorTurmaDto { CodigoAluno = 1002, NomeAluno = "Maria Santos", Sexo = "F", Raca = "BRANCA" }
        };
    }

    private void ConfigurarMocksBase(
        FiltroQuestionario filtro,
        TurmaElasticDto turma,
        Dominio.Entidades.Sondagem.Sondagem sondagem,
        List<Dominio.Entidades.Questionario.Questao> questoes)
    {
        _mockRepositorioElasticTurma
            .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(turma);

        _mockRepositorioSondagem
            .Setup(x => x.ObterSondagemAtiva(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sondagem);

        _mockRepositorioQuestao
            .Setup(x => x.ObterQuestoesAtivasPorFiltroAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(questoes);

        _mockRepositorioBimestre
            .Setup(x => x.ObterBimestresPorQuestionarioIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Dominio.Entidades.Sondagem.SondagemPeriodoBimestre>());
    }

    private void ConfigurarMocksCompleto(
        FiltroQuestionario filtro,
        TurmaElasticDto turma,
        Dominio.Entidades.Sondagem.Sondagem sondagem,
        List<Dominio.Entidades.Questionario.Questao> questoes,
        List<AlunoElasticDto> alunos,
        IEnumerable<DadosAlunoPorTurmaDto> dadosAlunos)
    {
        ConfigurarMocksBase(filtro, turma, sondagem, questoes);

        _mockRepositorioElasticAluno
            .Setup(x => x.ObterAlunosPorIdTurma(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(alunos);

        _mockAlunoTurmaService
            .Setup(x => x.InformacoesAlunosPorTurma(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dadosAlunos);

        _mockAlunoPapService
            .Setup(x => x.VerificarAlunosPossuemProgramaPapAsync(
                It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, bool>());

        _mockRepositorioRespostaAluno
            .Setup(x => x.VerificarAlunosPossuiLinguaPortuguesaAsync(
                It.IsAny<List<int>>(), It.IsAny<Dominio.Entidades.Questionario.Questao>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, bool>());

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasAlunosPorQuestoesAsync(
                It.IsAny<List<long>>(), It.IsAny<List<long>>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<(long, long, int?), RespostaAluno>());
    }

    #endregion
}
