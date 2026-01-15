using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Infra.Teste.DTO;

public static class SondagemMockData
{
    public static SondagemSalvarDto ObterSondagemMock()
    {
        return new SondagemSalvarDto
        {
            SondagemId = 1,
            Alunos =
            [
                new AlunoSondagemDto
                {
                    Codigo = 101,
                    NumeroAlunoChamada = "2024001234",
                    NomeEstudante = "João Silva Santos",
                    LinguaPortuguesaSegundaLingua = true,
                    Respostas =
                    [
                        new RespostaSondagemDto
                        {
                            BimestreId = 1,
                            QuestaoId = 1,
                            OpcaoRepostaId = 3
                        },

                        new RespostaSondagemDto
                        {
                            BimestreId = 1,
                            QuestaoId = 2,
                            OpcaoRepostaId = 2
                        },

                        new RespostaSondagemDto
                        {
                            BimestreId = 1,
                            QuestaoId = 3,
                            OpcaoRepostaId = 4
                        }
                    ]
                },

                new AlunoSondagemDto
                {
                    Codigo = 102,
                    NumeroAlunoChamada = "2024001235",
                    NomeEstudante = "Maria Oliveira Costa",
                    LinguaPortuguesaSegundaLingua = false,
                    Respostas =
                    [
                        new RespostaSondagemDto
                        {
                            BimestreId = 1,
                            QuestaoId = 1,
                            OpcaoRepostaId = 2
                        },

                        new RespostaSondagemDto
                        {
                            BimestreId = 1,
                            QuestaoId = 2,
                            OpcaoRepostaId = 3
                        }
                    ]
                },

                new AlunoSondagemDto
                {
                    Codigo = 103,
                    NumeroAlunoChamada = "2024001236",
                    NomeEstudante = "Pedro Henrique Souza",
                    LinguaPortuguesaSegundaLingua = true,
                    Respostas =
                    [
                        new RespostaSondagemDto
                        {
                            BimestreId = 1,
                            QuestaoId = 1,
                            OpcaoRepostaId = 4
                        },

                        new RespostaSondagemDto
                        {
                            BimestreId = 1,
                            QuestaoId = 2,
                            OpcaoRepostaId = 4
                        },

                        new RespostaSondagemDto
                        {
                            BimestreId = 1,
                            QuestaoId = 3,
                            OpcaoRepostaId = 3
                        },

                        new RespostaSondagemDto
                        {
                            BimestreId = 1,
                            QuestaoId = 4,
                            OpcaoRepostaId = 2
                        }
                    ]
                }
            ]
        };
    }

    public static SondagemSalvarDto ObterSondagemVazia()
    {
        return new SondagemSalvarDto
        {
            SondagemId = 2,
            Alunos = []
        };
    }

    public static Dominio.Entidades.Sondagem.Sondagem CriarSondagemAtiva(int id, int bimestreId = 1)
    {
        var sondagem = new Dominio.Entidades.Sondagem.Sondagem("SondagemAtiva", DateTime.Now)
        {
            Id = id
        };

        var periodo = new SondagemPeriodoBimestre(
            id,
            bimestreId,
            DateTime.Now.AddDays(-1),
            DateTime.Now.AddDays(1));

        sondagem.PeriodosBimestre.Add(periodo);

        return sondagem;
    }


    public static AlunoSondagemDto ObterAlunoMock()
    {
        return new AlunoSondagemDto
        {
            Codigo = 201,
            NumeroAlunoChamada = "2024005678",
            NomeEstudante = "Ana Carolina Ferreira",
            LinguaPortuguesaSegundaLingua = false,
            Respostas =
            [
                new RespostaSondagemDto
                {
                    BimestreId = 2,
                    QuestaoId = 5,
                    OpcaoRepostaId = 1
                }
            ]
        };
    }
}