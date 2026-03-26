using ClosedXML.Excel;
using CsvHelper.Configuration.Attributes;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Extensions;
using SME.Sondagem.Infrastructure.Dtos;
using System.Reflection;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio
{
    public class ObterSondagemRelatorioPorTodasTurmaUseCase : IObterSondagemRelatorioPorTodasTurmaUseCase
    {
        private readonly RepositoriosElastic _repositoriosElastic;
        private readonly RepositoriosSondagem _repositoriosSondagem;
        private readonly RepositorioSondagemRelatorioPorTodasTurma _repositorioSondagemRelatorioPorTodasTurma;


        public ObterSondagemRelatorioPorTodasTurmaUseCase(IUeComDreEolService ueComDreEolService,
            RepositoriosElastic repositoriosElastic,
            RepositoriosSondagem repositoriosSondagem,
            RepositorioSondagemRelatorioPorTodasTurma repositorioSondagemRelatorioPorTodasTurma
            )
        {
            _repositoriosElastic = repositoriosElastic ?? throw new ArgumentNullException(nameof(repositoriosElastic));
            _repositoriosSondagem = repositoriosSondagem ?? throw new ArgumentNullException(nameof(repositoriosSondagem));
            _repositorioSondagemRelatorioPorTodasTurma = repositorioSondagemRelatorioPorTodasTurma ?? throw new ArgumentNullException(nameof(repositorioSondagemRelatorioPorTodasTurma));
        }

        public async Task<FileResultDto> ObterSondagemRelatorio(CancellationToken cancellationToken = default)
        {
            const string NOME_COMPONENTE = "Língua Portuguesa";
            const int modalidadeIdFundamental = (int)Modalidade.Fundamental;

            var lista = new List<ExtracaoSondagemLpEscritaDto>();
            var componenteLp = await ObterPorNomeModalidade(NOME_COMPONENTE, modalidadeIdFundamental, cancellationToken);
            var respostas = await ObterExtracaoDadosRespostasAsync(modalidadeIdFundamental, componenteLp!, cancellationToken);
            var codigoAlunos = ObterCodigosAlunos(respostas);
            var dadosAlunos = await ObterAlunos(codigoAlunos, cancellationToken);
            var dadosCompletosTurmas = await ObterTurmasPorCodigosNoElastic(dadosAlunos, cancellationToken);
            var turmasCodigoNome = MapearTurma(dadosCompletosTurmas, dadosAlunos, respostas);
            var codigoUes = ObterCodigosUes(dadosAlunos);
            var uesComDre = await BuscarUesDres(codigoUes);
            var dadosArquivo = await MapearAquivo(lista, respostas, uesComDre, turmasCodigoNome, dadosAlunos);
            var xlsxStream = GerarXlsx(dadosArquivo);

            return new FileResultDto(
                xlsxStream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"sondagem-lp-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.xlsx"
            );

        }

        private static IEnumerable<string> ObterCodigosUes(IEnumerable<AlunoEolDto> dadosAlunos)
        {
            return dadosAlunos.Select(x => x.CodigoEscola!).Distinct();
        }

        private  IEnumerable<TurmaCodigoElasticDto> MapearTurma2(IEnumerable<Infra.Dtos.Questionario.TurmaElasticDto> dadosCompletosTurmas, IEnumerable<AlunoEolDto> dadosAlunos, IEnumerable<ExtracaoSondagemLpEscritaDto> respostas)
        {
            var retor = new List<TurmaCodigoElasticDto>();
            foreach (var item in dadosAlunos.OrderBy(x => x.DataMatricula))
            {
                var resposta = respostas.FirstOrDefault(x => x.DataResposta?.Date >= item.DataMatricula.Date);
                
                var aluno = dadosAlunos.FirstOrDefault(x => resposta?.DataResposta?.Date >= x.DataMatricula.Date);
                var turma = dadosCompletosTurmas.Where(x =>x.CodigoTurma == aluno?.CodigoTurma).Select(x => new TurmaCodigoElasticDto(x.CodigoTurma, $"{x.NomeTurma} - {dadosAlunos.Where(w => w.CodigoTurma == x.CodigoTurma)?.FirstOrDefault()?.SituacaoMatricula ?? string.Empty} - {dadosAlunos.Where(w => w.CodigoTurma == x.CodigoTurma)?.FirstOrDefault()?.DataMatricula.ToString() ?? string.Empty}", x.AnoTurma,aluno.CodigoAluno,aluno.CodigoEscola)).Distinct();
                if (turma.Count() > 0)
                    retor.AddRange(turma);
            }
            return retor;
        }
        private IEnumerable<TurmaCodigoElasticDto> MapearTurma(
                 IEnumerable<Infra.Dtos.Questionario.TurmaElasticDto> dadosCompletosTurmas,
                 IEnumerable<AlunoEolDto> dadosAlunos,
                 IEnumerable<ExtracaoSondagemLpEscritaDto> respostas)
        {
            var resultado = new List<TurmaCodigoElasticDto>();

            // 1. Agrupamos as respostas por Código de Estudante para facilitar a busca
            var respostasPorAluno = respostas
                .Where(r => !string.IsNullOrEmpty(r.CodigoEolEstudante))
                .ToLookup(r => r.CodigoEolEstudante);

            // 2. Foreach em dadosAlunos conforme solicitado
            foreach (var aluno in dadosAlunos)
            {

                var codigoAlunoString = aluno.CodigoAluno.ToString();

                // Obtemos todas as respostas desse aluno específico
                var respostasDoAluno = respostasPorAluno[codigoAlunoString];

                if (respostasDoAluno.Any())
                {
                    // 3. Obter a resposta mais recente (ordenada por data)
                    var ultimaResposta = respostasDoAluno
                        .OrderByDescending(r => r.DataResposta)
                        .FirstOrDefault();

                    // 4. Verificar se a matrícula do aluno é anterior ou igual à data da resposta
                    // (Garantindo que ele estava nessa turma quando respondeu)
                    if (aluno.DataMatricula.Date <= ultimaResposta?.DataResposta?.Date)
                    {
                        var turmaElastic = dadosCompletosTurmas
                            .FirstOrDefault(t => t.CodigoTurma == aluno.CodigoTurma);

                        if (turmaElastic != null)
                        {
                            resultado.Add(new TurmaCodigoElasticDto(
                                turmaElastic.CodigoTurma,
                                $"{turmaElastic.NomeTurma} - {aluno.SituacaoMatricula} - {aluno.DataMatricula}",
                                turmaElastic.AnoTurma,aluno.CodigoAluno, turmaElastic.CodigoEscola!)
                            {
                                DataMatricula = aluno.DataMatricula
                            });
                        }
                    }
                    else
                    {
                        var turmaElastic = dadosCompletosTurmas
                                .FirstOrDefault(t => t.CodigoTurma == aluno.CodigoTurma);

                        if (turmaElastic != null)
                        {
                            resultado.Add(new TurmaCodigoElasticDto(
                                turmaElastic.CodigoTurma,
                                $"{turmaElastic.NomeTurma} - {aluno.SituacaoMatricula} - {aluno.DataMatricula}",
                                turmaElastic.AnoTurma, aluno.CodigoAluno, turmaElastic.CodigoEscola!)
                            {
                                DataMatricula = aluno.DataMatricula
                            });
                        }
                    }
                }
            }

            // Retorna apenas turmas distintas para evitar duplicidade no mapeamento final
            return resultado;
        }

        private async Task<IEnumerable<Infra.Dtos.Questionario.TurmaElasticDto>> ObterTurmasPorCodigosNoElastic(IEnumerable<AlunoEolDto> dadosAlunos, CancellationToken cancellationToken)
        {
            return await _repositoriosElastic.RepositorioElasticTurma.ObterTurmasPorIds(dadosAlunos.Select(x => x.CodigoTurma), cancellationToken);
        }

        private static List<string> ObterCodigosAlunos(IEnumerable<ExtracaoSondagemLpEscritaDto> responstas)
        {
            return responstas.Select(x => x.CodigoEolEstudante!).ToList() ?? new List<string>();
        }

        private async Task<IEnumerable<ExtracaoSondagemLpEscritaDto>> ObterExtracaoDadosRespostasAsync(int modalidadeIdFundamental, Dominio.Entidades.ComponenteCurricular componenteLp, CancellationToken cancellationToken)
        {
            if (componenteLp == null) return new List<ExtracaoSondagemLpEscritaDto>();
            return await _repositoriosSondagem.RepositorioRespostaAluno.ObterExtracaoDadosRespostasAsync(modalidadeIdFundamental, componenteLp!.Id, cancellationToken);
        }

        private async Task<Dominio.Entidades.ComponenteCurricular?> ObterPorNomeModalidade(string NOME_COMPONENTE, int modalidadeIdFundamental, CancellationToken cancellationToken)
        {
            return await _repositoriosSondagem.RepositorioComponenteCurricular.ObterPorNomeModalidade(NOME_COMPONENTE, modalidadeIdFundamental.ToString(), cancellationToken);
        }

        private async Task<IEnumerable<ExtracaoSondagemLpEscritaDto>> MapearAquivo(
                List<ExtracaoSondagemLpEscritaDto> lista,
                IEnumerable<ExtracaoSondagemLpEscritaDto> responstas,
                IEnumerable<UeComDreEolDto> uesComDre,
                IEnumerable<TurmaCodigoElasticDto> turmasCodigoNome,
                IEnumerable<AlunoEolDto> dadosAlunos)
        {
            var bimestresLista = await _repositoriosSondagem.RepositorioBimestre.ListarAsync();

            var alunosPorCodigo = dadosAlunos
                .GroupBy(x => x.CodigoAluno.ToString())
                .ToDictionary(g => g.Key, g => g.First());

            var bimestresPorId = (bimestresLista ?? [])
                .GroupBy(x => x.Id.ToString())
                .ToDictionary(g => g.Key, g => g.First());

            var modalidadesCache = Enum.GetValues<Modalidade>()
                .ToDictionary(m => (int)m, m => m.Nome() ?? m.ToString());

            var turmasPorAluno = turmasCodigoNome
                .Where(x => x.CodigoAluno.HasValue)
                .GroupBy(x => x.CodigoAluno!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var uesPorCodigoEscola = uesComDre
                .Where(x => x.CodigoEscola != null)
                .GroupBy(x => x.CodigoEscola!)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var responsta in responstas)
            {
                if (!alunosPorCodigo.TryGetValue(responsta.CodigoEolEstudante!, out var aluno))
                    continue;

                var bimestreDesc = responsta?.Bimestre != null && bimestresPorId.TryGetValue(responsta.Bimestre, out var bimestre)
                    ? bimestre.Descricao
                    : "Todos";

                var modalidadeNome = modalidadesCache.TryGetValue(responsta?.ModalidadeId ?? 0, out var nome)
                    ? nome
                    : string.Empty;

                var alunosTurma = turmasPorAluno.TryGetValue(aluno.CodigoAluno, out var turmas)
                    ? turmas
                    : new List<TurmaCodigoElasticDto>();

                var codigoEscolasDistinct = alunosTurma
                    .Select(x => x.CodigoEscola)
                    .Where(c => c != null)
                    .Distinct()
                    .ToList();

                var ueDresNomes = codigoEscolasDistinct
                    .Where(c => uesPorCodigoEscola.ContainsKey(c!))
                    .Select(c => uesPorCodigoEscola[c!])
                    .Distinct()
                    .ToList();

                lista.Add(new ExtracaoSondagemLpEscritaDto
                {
                    NomeDre = string.Join(" | ", ueDresNomes.Select(x => x.NomeDRE).Distinct()),
                    Bimestre = bimestreDesc,
                    CodigoDre = string.Join(" | ", ueDresNomes.Select(x => x.CodigoDRE).Distinct()),
                    NomeEscola = string.Join(" | ", ueDresNomes.Select(x => x.NomeEscola).Distinct()),
                    CodigoEolEscola = string.Join(" | ", ueDresNomes.Select(x => x.CodigoEscola).Distinct()),
                    NomeTurma = string.Join(" | ", alunosTurma.Select(x => x.NomeTurma).Distinct()),
                    CodigoEolEstudante = aluno.CodigoAluno.ToString(),
                    NomeEstudanteEstudante = aluno?.NomeAluno ?? string.Empty,
                    ComponenteCurricular = responsta?.ComponenteCurricular,
                    Proficiencia = responsta?.Proficiencia,
                    Ano = string.Join(" | ", alunosTurma.Select(x => x.AnoTurma).Distinct()),
                    Questao = responsta?.Questao ?? string.Empty,
                    Resposta = responsta?.Resposta,
                    Legenda = responsta?.Legenda ?? string.Empty,
                    Modalidade = modalidadeNome,
                    ModalidadeId = responsta?.ModalidadeId,
                    DataResposta = responsta?.DataResposta,
                });
            }

            return lista.OrderBy(x => x.NomeEstudanteEstudante);
        }


        private async Task<IEnumerable<ExtracaoSondagemLpEscritaDto>> MapearAquivo4(
                    List<ExtracaoSondagemLpEscritaDto> lista,
                    IEnumerable<ExtracaoSondagemLpEscritaDto> responstas,
                    IEnumerable<UeComDreEolDto> uesComDre,
                    IEnumerable<TurmaCodigoElasticDto> turmasCodigoNome,
                    IEnumerable<AlunoEolDto> dadosAlunos)
        {
            var bimestresLista = await _repositoriosSondagem.RepositorioBimestre.ListarAsync();

            var alunosPorCodigo = dadosAlunos
                .GroupBy(x => x.CodigoAluno.ToString())
                .ToDictionary(g => g.Key, g => g.First());

            var bimestresPorId = (bimestresLista ?? [])
                .GroupBy(x => x.Id.ToString())
                .ToDictionary(g => g.Key, g => g.First());

            var modalidadesCache = Enum.GetValues<Modalidade>()
                .ToDictionary(
                    m => (int)m,
                    m => m.Nome() ?? m.ToString()
                );

            foreach (var responsta in responstas)
            {
                if (!alunosPorCodigo.TryGetValue(responsta.CodigoEolEstudante!, out var aluno))
                    continue;

                
                var bimestreDesc = responsta?.Bimestre != null && bimestresPorId.TryGetValue(responsta.Bimestre, out var bimestre)
                    ? bimestre.Descricao
                    : "Todos";

                var modalidadeNome = modalidadesCache.TryGetValue(responsta?.ModalidadeId ?? 0, out var nome)
                    ? nome
                    : string.Empty;
                var alunosTurma = turmasCodigoNome.Where(x => x.CodigoAluno == aluno?.CodigoAluno).ToList();
                var codigoEscolas  = alunosTurma.Select(x =>x.CodigoEscola);
                var ueDresNomes = uesComDre.Where(x => codigoEscolas.Distinct().Contains(x.CodigoEscola)).Distinct();

                lista.Add(new ExtracaoSondagemLpEscritaDto
                {
                    NomeDre = string.Join(" | ", ueDresNomes.Select(x => x.NomeDRE).Distinct()) ?? string.Empty,
                    Bimestre = bimestreDesc,
                    CodigoDre = string.Join(" | ", ueDresNomes.Select(x => x.CodigoDRE).Distinct()) ?? string.Empty,
                    NomeEscola = string.Join(" | ", ueDresNomes.Select(x => x.NomeEscola).Distinct()) ?? string.Empty,
                    CodigoEolEscola = string.Join(" | ", ueDresNomes.Select(x => x.CodigoEscola).Distinct()) ?? string.Empty,
                    NomeTurma = string.Join(" | ", alunosTurma.Select(x =>x.NomeTurma).Distinct()) ?? string.Empty,
                    CodigoEolEstudante = aluno.CodigoAluno.ToString(),
                    NomeEstudanteEstudante = aluno?.NomeAluno ?? string.Empty,
                    ComponenteCurricular = responsta?.ComponenteCurricular,
                    Proficiencia = responsta?.Proficiencia,
                    Ano = string.Join(" | ", alunosTurma.Select(x => x.AnoTurma).Distinct()) ?? string.Empty,
                    Questao = responsta?.Questao ?? string.Empty,
                    Resposta = responsta?.Resposta,
                    Legenda = responsta?.Legenda ?? string.Empty,
                    Modalidade = modalidadeNome,
                    ModalidadeId = responsta?.ModalidadeId,
                    DataResposta = responsta?.DataResposta,
                });
            }

            return lista.OrderBy(x => x.NomeEstudanteEstudante);
        }

        private static MemoryStream GerarXlsx(IEnumerable<ExtracaoSondagemLpEscritaDto> dados)
        {
            var memoryStream = new MemoryStream();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sondagem");

            var propriedades = typeof(ExtracaoSondagemLpEscritaDto).GetProperties();

            for (int i = 0; i < propriedades.Length; i++)
            {
                var prop = propriedades[i];

                var nameAttr = prop
                    .GetCustomAttribute<NameAttribute>();

                var headerText = nameAttr?.Names?.FirstOrDefault() ?? prop.Name;

                worksheet.Cell(1, i + 1).Value = headerText;
            }

            var lista = dados.ToList();
            for (int row = 0; row < lista.Count; row++)
            {
                for (int col = 0; col < propriedades.Length; col++)
                {
                    var valor = propriedades[col].GetValue(lista[row]);
                    worksheet.Cell(row + 2, col + 1).Value = valor?.ToString() ?? string.Empty;
                }
            }

            workbook.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        private async Task<IEnumerable<AlunoEolDto>> ObterAlunos(List<string> codigoAlunos, CancellationToken cancellationToken)
        {
            var retorno = new List<AlunoEolDto>();
            if (codigoAlunos.Count == 0)
                return retorno;

            var dados = await _repositorioSondagemRelatorioPorTodasTurma.DadosAlunosService.ObterDadosAlunosPorCodigoUe(codigoAlunos, cancellationToken);
            if (dados.Any())
                retorno.AddRange(dados);


            return retorno;
        }
        private async Task<IEnumerable<UeComDreEolDto>> BuscarUesDres(IEnumerable<string> codigosUes)
        {
            var retorno = new List<UeComDreEolDto>();

            if (!codigosUes.Any())
                return retorno;

            var busca = await _repositorioSondagemRelatorioPorTodasTurma.UeComDreEolService.ObterUesComDrePorCodigosUes(codigosUes);

            if (busca.Any())
                retorno.AddRange(busca);

            return retorno;
        }
    }
}
