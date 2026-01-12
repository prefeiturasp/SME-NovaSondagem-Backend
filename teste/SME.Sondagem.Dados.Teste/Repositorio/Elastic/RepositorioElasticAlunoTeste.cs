using FluentAssertions;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Elastic;

public class RepositorioElasticAlunoTeste
{
    [Fact]
    public async Task ObterAlunosPorIdTurma_ComAlunosConfigurados_DeveRetornarListaDeAlunos()
    {
        var alunosEsperados = new List<AlunoElasticDto>
        {
            new()
            {
                CodigoAluno = 1,
                NumeroAlunoChamada = "1",
                NomeAluno = "João Silva",
                PossuiDeficiencia = 0,
                CodigoTurma = 10
            },
            new()
            {
                CodigoAluno = 2,
                NumeroAlunoChamada = "2",
                NomeAluno = "Maria Santos",
                PossuiDeficiencia = 1,
                CodigoTurma = 10
            }
        };

        var repositorioFake = new RepositorioElasticAlunoFake
        {
            Retorno = alunosEsperados
        };

        var resultado = await repositorioFake.ObterAlunosPorIdTurma(10, CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.Should().BeEquivalentTo(alunosEsperados);
    }

    [Fact]
    public async Task ObterAlunosPorIdTurma_SemAlunosConfigurados_DeveRetornarListaVazia()
    {
        var repositorioFake = new RepositorioElasticAlunoFake
        {
            Retorno = Enumerable.Empty<AlunoElasticDto>()
        };

        var resultado = await repositorioFake.ObterAlunosPorIdTurma(10, CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterAlunosPorIdTurma_ComUmAluno_DeveRetornarAlunoUnico()
    {
        var alunoEsperado = new AlunoElasticDto
        {
            CodigoAluno = 100,
            NumeroAlunoChamada = "5",
            NomeAluno = "Pedro Oliveira",
            PossuiDeficiencia = 0,
            CodigoTurma = 25
        };

        var repositorioFake = new RepositorioElasticAlunoFake
        {
            Retorno = new[] { alunoEsperado }
        };

        var resultado = await repositorioFake.ObterAlunosPorIdTurma(25, CancellationToken.None);

        resultado.Should().ContainSingle();
        
        var aluno = resultado.First();
        aluno.CodigoAluno.Should().Be(100);
        aluno.NumeroAlunoChamada.Should().Be("5");
        aluno.NomeAluno.Should().Be("Pedro Oliveira");
        aluno.PossuiDeficiencia.Should().Be(0);
        aluno.CodigoTurma.Should().Be(25);
    }

    [Fact]
    public async Task ObterAlunosPorIdTurma_ComAlunosComDeficiencia_DeveRetornarCorretamente()
    {
        var alunos = new List<AlunoElasticDto>
        {
            new()
            {
                CodigoAluno = 1,
                NumeroAlunoChamada = "1",
                NomeAluno = "Aluno sem deficiência",
                PossuiDeficiencia = 0
            },
            new()
            {
                CodigoAluno = 2,
                NumeroAlunoChamada = "2",
                NomeAluno = "Aluno com deficiência",
                PossuiDeficiencia = 1
            }
        };

        var repositorioFake = new RepositorioElasticAlunoFake
        {
            Retorno = alunos
        };

        var resultado = await repositorioFake.ObterAlunosPorIdTurma(10, CancellationToken.None);

        resultado.Should().HaveCount(2);
        resultado.Should().Contain(a => a.PossuiDeficiencia == 0);
        resultado.Should().Contain(a => a.PossuiDeficiencia == 1);
    }

    [Fact]
    public async Task ObterAlunosPorIdTurma_ComAlunosSemNome_DeveRetornarComNomeNulo()
    {
        var alunos = new List<AlunoElasticDto>
        {
            new()
            {
                CodigoAluno = 1,
                NumeroAlunoChamada = "2",
                NomeAluno = null,
                PossuiDeficiencia = 0
            }
        };

        var repositorioFake = new RepositorioElasticAlunoFake
        {
            Retorno = alunos
        };

        var resultado = await repositorioFake.ObterAlunosPorIdTurma(10, CancellationToken.None);

        var aluno = resultado.First();
        aluno.NomeAluno.Should().BeNull();
    }

    [Fact]
    public async Task ObterAlunosPorIdTurma_ComMultiplosAlunos_DeveManterOrdem()
    {
        var alunos = new List<AlunoElasticDto>
        {
            new()
            {
                CodigoAluno = 3,
                NumeroAlunoChamada = "3",
                NomeAluno = "Carlos"
            },
            new()
            {
                CodigoAluno = 1,
                NumeroAlunoChamada = "1",
                NomeAluno = "Ana"
            },
            new()
            {
                CodigoAluno = 2,
                NumeroAlunoChamada = "2",
                NomeAluno = "Bruno"
            }
        };

        var repositorioFake = new RepositorioElasticAlunoFake
        {
            Retorno = alunos
        };

        var resultado = await repositorioFake.ObterAlunosPorIdTurma(10, CancellationToken.None);

        resultado.Should().HaveCount(3);
        resultado.Should().ContainInOrder(alunos);
    }

    [Fact]
    public async Task ObterAlunosPorIdTurma_ComCancellationToken_DeveExecutarNormalmente()
    {
        var alunos = new List<AlunoElasticDto>
        {
            new() { CodigoAluno = 1, NomeAluno = "Aluno Teste" }
        };

        var repositorioFake = new RepositorioElasticAlunoFake
        {
            Retorno = alunos
        };

        using var cts = new CancellationTokenSource();

        var resultado = await repositorioFake.ObterAlunosPorIdTurma(10, cts.Token);

        resultado.Should().NotBeNull();
        resultado.Should().ContainSingle();
    }

    [Fact]
    public async Task ObterAlunosPorIdTurma_ComDiferentesTurmas_DeveRetornarMesmoResultado()
    {
        var alunos = new List<AlunoElasticDto>
        {
            new() { CodigoAluno = 1, CodigoTurma = 10 },
            new() { CodigoAluno = 2, CodigoTurma = 10 }
        };

        var repositorioFake = new RepositorioElasticAlunoFake
        {
            Retorno = alunos
        };

        var resultado1 = await repositorioFake.ObterAlunosPorIdTurma(10, CancellationToken.None);
        var resultado2 = await repositorioFake.ObterAlunosPorIdTurma(20, CancellationToken.None);

        resultado1.Should().BeEquivalentTo(resultado2);
        resultado1.Should().BeSameAs(resultado2);
    }

    [Fact]
    public async Task ObterAlunosPorIdTurma_AlterandoRetorno_DeveRetornarNovosDados()
    {
        var primeiroRetorno = new List<AlunoElasticDto>
        {
            new() { CodigoAluno = 1, NomeAluno = "Aluno 1" }
        };

        var segundoRetorno = new List<AlunoElasticDto>
        {
            new() { CodigoAluno = 2, NomeAluno = "Aluno 2" },
            new() { CodigoAluno = 3, NomeAluno = "Aluno 3" }
        };

        var repositorioFake = new RepositorioElasticAlunoFake
        {
            Retorno = primeiroRetorno
        };

        var resultado1 = await repositorioFake.ObterAlunosPorIdTurma(10, CancellationToken.None);

        repositorioFake.Retorno = segundoRetorno;

        var resultado2 = await repositorioFake.ObterAlunosPorIdTurma(10, CancellationToken.None);

        resultado1.Should().HaveCount(1);
        resultado1.First().NomeAluno.Should().Be("Aluno 1");

        resultado2.Should().HaveCount(2);
        resultado2.Should().Contain(a => a.NomeAluno == "Aluno 2");
        resultado2.Should().Contain(a => a.NomeAluno == "Aluno 3");
    }

    [Fact]
    public async Task ObterAlunosPorIdTurma_ComTodosOsCamposPreenchidos_DeveRetornarCompleto()
    {
        var alunoCompleto = new AlunoElasticDto
        {
            CodigoAluno = 12345,
            NumeroAlunoChamada = "15",
            NomeAluno = "Maria da Silva Santos",
            PossuiDeficiencia = 1,
            CodigoTurma = 100,
            DataNascimento = new DateTime(2010, 5, 15)
        };

        var repositorioFake = new RepositorioElasticAlunoFake
        {
            Retorno = new[] { alunoCompleto }
        };

        var resultado = await repositorioFake.ObterAlunosPorIdTurma(100, CancellationToken.None);

        var aluno = resultado.First();
        aluno.Should().BeEquivalentTo(alunoCompleto);
    }

    [Fact]
    public async Task ObterAlunosPorIdTurma_ComGrandeQuantidadeDeAlunos_DeveRetornarTodos()
    {
        var alunos = Enumerable.Range(1, 250)
            .Select(i => new AlunoElasticDto
            {
                CodigoAluno = i,
                NumeroAlunoChamada = "{i}",
                NomeAluno = $"Aluno {i}",
                PossuiDeficiencia = i % 10 == 0 ? 1 : 0
            })
            .ToList();

        var repositorioFake = new RepositorioElasticAlunoFake
        {
            Retorno = alunos
        };

        var resultado = await repositorioFake.ObterAlunosPorIdTurma(10, CancellationToken.None);

        resultado.Should().HaveCount(250);
        resultado.Should().Contain(a => a.CodigoAluno == 1);
        resultado.Should().Contain(a => a.CodigoAluno == 250);
        resultado.Count(a => a.PossuiDeficiencia == 1).Should().Be(25);
    }

    [Fact]
    public async Task ObterAlunosPorIdTurma_DeveSerAsync()
    {
        var repositorioFake = new RepositorioElasticAlunoFake
        {
            Retorno = new[] { new AlunoElasticDto { CodigoAluno = 1 } }
        };

        var task = repositorioFake.ObterAlunosPorIdTurma(10, CancellationToken.None);

        task.Should().NotBeNull();
        task.IsCompleted.Should().BeTrue();
        
        var resultado = await task;
        resultado.Should().NotBeNull();
    }
}
