using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.API.Teste.Mock.Dto;

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
                        AlunoId = 101,
                        NumeroEstudante = "2024001234",
                        NomeEstudante = "João Silva Santos",
                        Lp = true,
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
                        AlunoId = 102,
                        NumeroEstudante = "2024001235",
                        NomeEstudante = "Maria Oliveira Costa",
                        Lp = false,
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
                        AlunoId = 103,
                        NumeroEstudante = "2024001236",
                        NomeEstudante = "Pedro Henrique Souza",
                        Lp = true,
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

        public static AlunoSondagemDto ObterAlunoMock()
        {
            return new AlunoSondagemDto
            {
                AlunoId = 201,
                NumeroEstudante = "2024005678",
                NomeEstudante = "Ana Carolina Ferreira",
                Lp = false,
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