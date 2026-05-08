using Moq;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.UseCases.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;
using RacaCorEntidade = SME.Sondagem.Dominio.Entidades.RacaCor;
using SondagemEntidade = SME.Sondagem.Dominio.Entidades.Sondagem.Sondagem;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Sondagem;

public class AtualizarContextoRespostasLegadoUseCaseTeste
{
    [Fact]
    public async Task ExecutarAsync_Quando_Nao_Ha_Respostas_Deve_Retornar_Zero_e_Nao_Chama_Atualizar_Lote()
    {
        var repositorioResposta = new Mock<IRepositorioRespostaAluno>();
        repositorioResposta
            .Setup(r => r.ObterRespostasSemContextoPaginadoAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<RespostaAlunoLegadoDto>());

        var sut = CriarUseCase(repositorioResposta);

        var resultado = await sut.ExecutarAsync(0, 1, 50, CancellationToken.None);

        Assert.Equal(0, resultado);
        repositorioResposta.Verify(
            r => r.AtualizarContextoLoteAsync(It.IsAny<IEnumerable<AtualizarContextoRespostaAlunoDto>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_Quando_Resposta_Existe_Mas_Sondagem_Nao_Carregada_Deve_Retornar_Zero_Sem_Lote()
    {
        var repositorioResposta = new Mock<IRepositorioRespostaAluno>();
        var repositorioSondagem = new Mock<IRepositorioSondagem>();

        repositorioResposta
            .Setup(r => r.ObterRespostasSemContextoPaginadoAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RespostaAlunoLegadoDto>
            {
                new() { Id = 1, AlunoId = 100, SondagemId = 9, AnoLetivo = 2026 },
            });

        repositorioSondagem
            .Setup(s => s.ObterPorIdAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SondagemEntidade?)null);

        var sut = CriarUseCase(repositorioResposta, repositorioSondagem);

        var resultado = await sut.ExecutarAsync(0, 1, 50, CancellationToken.None);

        Assert.Equal(0, resultado);
        repositorioResposta.Verify(
            r => r.AtualizarContextoLoteAsync(It.IsAny<IEnumerable<AtualizarContextoRespostaAlunoDto>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_Quando_Aluno_Nao_Vem_Do_Eol_Deve_Retornar_Zero_Sem_Lote()
    {
        var repositorioResposta = new Mock<IRepositorioRespostaAluno>();
        var repositorioSondagem = new Mock<IRepositorioSondagem>();
        var dadosAlunosService = new Mock<IDadosAlunosService>();

        repositorioResposta
            .Setup(r => r.ObterRespostasSemContextoPaginadoAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RespostaAlunoLegadoDto>
            {
                new() { Id = 1, AlunoId = 100, SondagemId = 1, AnoLetivo = 2026 },
            });

        repositorioSondagem
            .Setup(s => s.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarSondagem(1, new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc   )));

        dadosAlunosService
            .Setup(s => s.ObterDadosAlunosPorCodigoUe(
                It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<AlunoEolDto>());

        var sut = CriarUseCase(repositorioResposta, repositorioSondagem, dadosAlunosService);

        var resultado = await sut.ExecutarAsync(0, 1, 50, CancellationToken.None);

        Assert.Equal(0, resultado);
        repositorioResposta.Verify(
            r => r.AtualizarContextoLoteAsync(It.IsAny<IEnumerable<AtualizarContextoRespostaAlunoDto>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

   
    [Fact]
    public async Task ExecutarAsync_Quando_Dados_Completos_Deve_Persistir_Lote_e_Retornar_Contagem_Do_Repositorio()
    {
        var repositorioResposta = new Mock<IRepositorioRespostaAluno>();
        var repositorioSondagem = new Mock<IRepositorioSondagem>();
        var dadosAlunosService = new Mock<IDadosAlunosService>();
        var elasticTurma = new Mock<IRepositorioElasticTurma>();
        var elasticAluno = new Mock<IRepositorioElasticAluno>();
        var ueService = new Mock<IUeComDreEolService>();
        var alunoPap = new Mock<IAlunoPapService>();
        var repositorioRacaCor = new Mock<IRepositorioRacaCor>();

        repositorioResposta
            .Setup(r => r.ObterRespostasSemContextoPaginadoAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RespostaAlunoLegadoDto>
            {
                new() { Id = 42, AlunoId = 100, SondagemId = 1, AnoLetivo = 2026 },
            });

        repositorioSondagem
            .Setup(s => s.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarSondagem(1, new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)));

        repositorioRacaCor
            .Setup(r => r.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RacaCorEntidade>
            {
                new() { Id = 7, Descricao = "Branca", CodigoEolRacaCor = 10 },
            });

        dadosAlunosService
            .Setup(s => s.ObterDadosAlunosPorCodigoUe(
                It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AlunoEolDto>
            {
                new()
                {
                    CodigoAluno = 100,
                    CodigoTurma = 50,
                    CodigoEscola = "UE1",
                    DataMatricula = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                },
            });

        dadosAlunosService
            .Setup(s => s.ObterDadosRacaGeneroAlunos(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AlunoRacaGeneroDto>
            {
                new() { CodigoAluno = 100, RacaId = 10, SexoId = 3 },
            });

        elasticTurma
            .Setup(t => t.ObterTurmasPorIds(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TurmaElasticDto>
            {
                new()
                {
                    CodigoTurma = 50,
                    CodigoEscola = "UE1",
                    AnoLetivo = 2026,
                    AnoTurma = "5",
                    Modalidade = 8,
                },
            });

        elasticAluno
            .Setup(a => a.ObterAlunosPorIdTurma(50, 2026, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AlunoElasticDto>
            {
                new() { CodigoAluno = 100, PossuiDeficiencia = 1 },
            });

        ueService
            .Setup(u => u.ObterUesComDrePorCodigosUes(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UeComDreEolDto>
            {
                new("UE1", "Nome", "DRE", "SG", "DRE-01", "Tipo", "ST", 1) { CodigoEscola = "UE1", CodigoDRE = "DRE-01" },
            });

        alunoPap
            .Setup(p => p.VerificarAlunosPossuemProgramaPapAsync(
                It.IsAny<IEnumerable<int>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, bool> { { 100, true } });

        IEnumerable<AtualizarContextoRespostaAlunoDto>? loteCapturado = null;
        repositorioResposta
            .Setup(r => r.AtualizarContextoLoteAsync(It.IsAny<IEnumerable<AtualizarContextoRespostaAlunoDto>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<AtualizarContextoRespostaAlunoDto>, CancellationToken>((l, _) => loteCapturado = l)
            .ReturnsAsync(1);

        var sut = CriarUseCase(new AmbienteAtualizarContextoLegado
        {
            RepositorioResposta = repositorioResposta,
            RepositorioSondagem = repositorioSondagem,
            DadosAlunosService = dadosAlunosService,
            UeService = ueService,
            AlunoPap = alunoPap,
            ElasticTurma = elasticTurma,
            ElasticAluno = elasticAluno,
            RepositorioRacaCor = repositorioRacaCor,
        });

        var retorno = await sut.ExecutarAsync(0, 1, 50, CancellationToken.None);

        Assert.Equal(1, retorno);
        Assert.NotNull(loteCapturado);
        var dto = Assert.Single(loteCapturado!.ToList());
        Assert.Equal(42, dto.Id);
        Assert.Equal("50", dto.TurmaId);
        Assert.Equal("UE1", dto.UeId);
        Assert.Equal("DRE-01", dto.DreId);
        Assert.Equal(2026, dto.AnoLetivo);
        Assert.Equal(5, dto.AnoTurma);
        Assert.Equal(8, dto.ModalidadeId);
        Assert.Equal(7, dto.RacaCorId);
        Assert.Equal(3, dto.GeneroSexoId);
        Assert.True(dto.Pap);
        Assert.False(dto.Aee);
        Assert.True(dto.Deficiente);

        repositorioResposta.Verify(
            r => r.AtualizarContextoLoteAsync(It.IsAny<IEnumerable<AtualizarContextoRespostaAlunoDto>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static SondagemEntidade CriarSondagem(int id, DateTime dataAplicacao)
    {
        var sondagem = new SondagemEntidade("Sondagem teste", dataAplicacao);
        sondagem.Id = id;
        return sondagem;
    }

    private static AtualizarContextoRespostasLegadoUseCase CriarUseCase(Mock<IRepositorioRespostaAluno> repositorioResposta, Mock<IRepositorioSondagem>? repositorioSondagem = null)
    {
        repositorioSondagem ??= new Mock<IRepositorioSondagem>();
        var dadosAlunosService = new Mock<IDadosAlunosService>();
        var alunoPap = new Mock<IAlunoPapService>();
        var ueService = new Mock<IUeComDreEolService>();
        var repositorioRacaCor = new Mock<IRepositorioRacaCor>();
        repositorioRacaCor.Setup(r => r.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<RacaCorEntidade>());
        var repositorioGeneroSexo = new Mock<IRepositorioGeneroSexo>();
        var elasticTurma = new Mock<IRepositorioElasticTurma>();
        elasticTurma.Setup(t => t.ObterTurmasPorIds(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TurmaElasticDto>());
        var elasticAluno = new Mock<IRepositorioElasticAluno>();
        elasticAluno.Setup(a => a.ObterAlunosPorIdTurma(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<AlunoElasticDto>());
        return CriarUseCase(new AmbienteAtualizarContextoLegado
        {
            RepositorioResposta = repositorioResposta,
            RepositorioSondagem = repositorioSondagem,
            DadosAlunosService = dadosAlunosService,
            UeService = ueService,
            AlunoPap = alunoPap,
            ElasticTurma = elasticTurma,
            ElasticAluno = elasticAluno,
            RepositorioRacaCor = repositorioRacaCor,
            RepositorioGeneroSexo = repositorioGeneroSexo,
        });
    }

    private static AtualizarContextoRespostasLegadoUseCase CriarUseCase(
        Mock<IRepositorioRespostaAluno> repositorioResposta,
        Mock<IRepositorioSondagem> repositorioSondagem,
        Mock<IDadosAlunosService> dadosAlunosService)
    {
        var alunoPap = new Mock<IAlunoPapService>();
        var ueService = new Mock<IUeComDreEolService>();
        var repositorioRacaCor = new Mock<IRepositorioRacaCor>();
        repositorioRacaCor.Setup(r => r.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<RacaCorEntidade>());
        var repositorioGeneroSexo = new Mock<IRepositorioGeneroSexo>();
        var elasticTurma = new Mock<IRepositorioElasticTurma>();
        elasticTurma.Setup(t => t.ObterTurmasPorIds(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TurmaElasticDto>());
        var elasticAluno = new Mock<IRepositorioElasticAluno>();
        elasticAluno.Setup(a => a.ObterAlunosPorIdTurma(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<AlunoElasticDto>());
        return CriarUseCase(new AmbienteAtualizarContextoLegado
        {
            RepositorioResposta = repositorioResposta,
            RepositorioSondagem = repositorioSondagem,
            DadosAlunosService = dadosAlunosService,
            UeService = ueService,
            AlunoPap = alunoPap,
            ElasticTurma = elasticTurma,
            ElasticAluno = elasticAluno,
            RepositorioRacaCor = repositorioRacaCor,
            RepositorioGeneroSexo = repositorioGeneroSexo,
        });
    }

    private sealed class AmbienteAtualizarContextoLegado
    {
        public required Mock<IRepositorioRespostaAluno> RepositorioResposta { get; init; }
        public required Mock<IRepositorioSondagem> RepositorioSondagem { get; init; }
        public required Mock<IDadosAlunosService> DadosAlunosService { get; init; }
        public required Mock<IUeComDreEolService> UeService { get; init; }
        public required Mock<IAlunoPapService> AlunoPap { get; init; }
        public required Mock<IRepositorioElasticTurma> ElasticTurma { get; init; }
        public required Mock<IRepositorioElasticAluno> ElasticAluno { get; init; }
        public Mock<IRepositorioRacaCor>? RepositorioRacaCor { get; init; }
        public Mock<IRepositorioGeneroSexo>? RepositorioGeneroSexo { get; init; }
    }

    private static AtualizarContextoRespostasLegadoUseCase CriarUseCase(AmbienteAtualizarContextoLegado ambiente)
    {
        var racaCorMock = ambiente.RepositorioRacaCor ?? new Mock<IRepositorioRacaCor>();
        if (ambiente.RepositorioRacaCor == null)
            racaCorMock.Setup(r => r.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<RacaCorEntidade>());
        var repositorioGeneroSexo = ambiente.RepositorioGeneroSexo ?? new Mock<IRepositorioGeneroSexo>();
        var dependencias = new AtualizarContextoRespostasLegadoDependencias
        {
            RepositorioRespostaAluno = ambiente.RepositorioResposta.Object,
            RepositorioSondagem = ambiente.RepositorioSondagem.Object,
            DadosAlunosService = ambiente.DadosAlunosService.Object,
            AlunoPapService = ambiente.AlunoPap.Object,
            UeComDreEolService = ambiente.UeService.Object,
            RepositorioSondagemRelatorioPorTodasTurma = new RepositorioSondagemRelatorioPorTodasTurma(ambiente.DadosAlunosService.Object, ambiente.UeService.Object),
            RepositorioRacaCor = racaCorMock.Object,
            RepositorioGeneroSexo = repositorioGeneroSexo.Object,
            RepositoriosElastic = new RepositoriosElastic(ambiente.ElasticTurma.Object, ambiente.ElasticAluno.Object),
        };
        return new AtualizarContextoRespostasLegadoUseCase(dependencias);
    }
}
