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
            var turmasCodigoNome = MapearTurma(dadosCompletosTurmas);
            var codigoUes = ObterCodigosUes(dadosAlunos);
            var uesComDre = await BuscarUesDres(codigoUes);
            var dadosArquivo = await MapearAquivo(lista, respostas, uesComDre, turmasCodigoNome, dadosAlunos);
            var xlsxStream = GerarXlsx(dadosArquivo);

            return new FileResultDto(
                xlsxStream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"sondagem-lp-escrita-{DateTime.Now:yyyy-MM-dd}.xlsx"
            );

        }

        private static IEnumerable<string> ObterCodigosUes(IEnumerable<AlunoEolDto> dadosAlunos)
        {
            return dadosAlunos.Select(x => x.CodigoEscola!).Distinct();
        }

        private static IEnumerable<TurmaCodigoElasticDto> MapearTurma(IEnumerable<Infra.Dtos.Questionario.TurmaElasticDto> dadosCompletosTurmas)
        {
            return dadosCompletosTurmas.Select(x => new TurmaCodigoElasticDto(x.CodigoTurma, x.NomeTurma, x.AnoTurma)).Distinct();
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

            var turmasPorCodigo = turmasCodigoNome
                .Where(t => t.CodigoTurma.HasValue)
                .GroupBy(t => t.CodigoTurma!.Value)
                .ToDictionary(g => g.Key, g => g.First());

            var uesPorCodigo = uesComDre
                .Where(e => !string.IsNullOrEmpty(e.CodigoEscola))
                .GroupBy(e => e.CodigoEscola!)
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

                turmasPorCodigo.TryGetValue(aluno.CodigoTurma, out var turma);
                uesPorCodigo.TryGetValue(aluno.CodigoEscola!, out var ueDre);

                var bimestreDesc = responsta?.Bimestre != null && bimestresPorId.TryGetValue(responsta.Bimestre, out var bimestre)
                    ? bimestre.Descricao
                    : "Todos";

                var modalidadeNome = modalidadesCache.TryGetValue(responsta?.ModalidadeId ?? 0, out var nome)
                    ? nome
                    : string.Empty;

                lista.Add(new ExtracaoSondagemLpEscritaDto
                {
                    NomeDre = ueDre?.NomeDRE ?? string.Empty,
                    Bimestre = bimestreDesc,
                    CodigoDre = ueDre?.CodigoDRE ?? string.Empty,
                    NomeEscola = ueDre?.NomeEscola ?? string.Empty,
                    CodigoEolEscola = ueDre?.CodigoEscola ?? string.Empty,
                    NomeTurma = turma?.NomeTurma ?? string.Empty,
                    CodigoEolEstudante = aluno.CodigoAluno.ToString(),
                    NomeEstudanteEstudante = aluno?.NomeAluno ?? string.Empty,
                    ComponenteCurricular = responsta?.ComponenteCurricular,
                    Proficiencia = responsta?.Proficiencia,
                    Ano = turma?.AnoTurma,
                    Questao = responsta?.Questao ?? string.Empty,
                    Resposta = responsta?.Resposta,
                    Legenda = responsta?.Legenda ?? string.Empty,
                    Modalidade = modalidadeNome,
                    ModalidadeId = responsta?.ModalidadeId,
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
