using ClosedXML.Excel;
using CsvHelper.Configuration.Attributes;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Extensions;
using SME.Sondagem.Infrastructure.Dtos;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using System.Reflection;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio
{
    public class ObterSondagemRelatorioPorTodasTurmaUseCase : IObterSondagemRelatorioPorTodasTurmaUseCase
    {
        private readonly RepositoriosElastic _repositoriosElastic;
        private readonly RepositoriosSondagem _repositoriosSondagem;
        private readonly RepositorioSondagemRelatorioPorTodasTurma _repositorioSondagemRelatorioPorTodasTurma;
        private readonly IConsultaDeDresService _consultaDeDresService;


        public ObterSondagemRelatorioPorTodasTurmaUseCase(IUeComDreEolService ueComDreEolService,
            RepositoriosElastic repositoriosElastic,
            RepositoriosSondagem repositoriosSondagem,
            RepositorioSondagemRelatorioPorTodasTurma repositorioSondagemRelatorioPorTodasTurma,
            IConsultaDeDresService consultaDeDresService)
        {
            _repositoriosElastic = repositoriosElastic ?? throw new ArgumentNullException(nameof(repositoriosElastic));
            _repositoriosSondagem = repositoriosSondagem ?? throw new ArgumentNullException(nameof(repositoriosSondagem));
            _repositorioSondagemRelatorioPorTodasTurma = repositorioSondagemRelatorioPorTodasTurma ?? throw new ArgumentNullException(nameof(repositorioSondagemRelatorioPorTodasTurma));
            _consultaDeDresService = consultaDeDresService ?? throw new ArgumentNullException(nameof(consultaDeDresService));
        }

        public async Task<FileResultDto?> ObterSondagemRelatorio(FiltroExtracaoDadosDTO filtroExtracaoDados, CancellationToken cancellationToken = default)
        {
            var lista = new List<ExtracaoSondagemLpEscritaDto>();
            var dres = await _consultaDeDresService.ObterDresSgpAsync(cancellationToken);
            var dresIs = dres.Select(x => x.CodigoDre);
            var listaDeComponentes = await ObterComponentesCurriculares(cancellationToken);
            
            string NOME_MODALIDADE = filtroExtracaoDados.Modalidade.ObterNome();
            var modalidadeId = (int)filtroExtracaoDados.Modalidade;

            foreach (var componente in listaDeComponentes)
            {
                foreach (var itemDreId in dresIs)
                {
                    var respostas = await ObterExtracaoDadosRespostasAsync(itemDreId!, modalidadeId, componente!, cancellationToken);

                    var codigoUes = respostas?.Select(c => c.CodigoEolEscola)?.Distinct() ?? new List<string>();
                    var codigoTurmas = respostas?.Select(c => Convert.ToInt32(c.TurmaId))?.Distinct() ?? new List<int>();

                    var codigoAlunos = ObterCodigosAlunos(respostas!);

                    var dadosAlunos = await ObterAlunos(codigoAlunos, codigoUes, codigoTurmas, cancellationToken);
                    var dadosCompletosTurmas = await ObterTurmasPorCodigosNoElastic(codigoTurmas, codigoUes!, cancellationToken);
                    var turmasCodigoNome = MapearTurma(dadosCompletosTurmas);

                    var uesComDre = await BuscarUesDres(codigoUes!);
                    await MapearAquivo(lista, respostas!, uesComDre, turmasCodigoNome, dadosAlunos);
                }

            }

            if (lista.Count > 0)
            {
                var xlsxStream = GerarXlsx(lista);

                return new FileResultDto(
                    xlsxStream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"sondagem-ExtracaoDadosSondagem-escrita-{DateTime.Now:yyyy-MM-dd HH.mm.ss}_{NOME_MODALIDADE.Trim() ?? string.Empty}.xlsx"
                );
            }
            else
            {
                return null;
            }

        }

        private static IEnumerable<TurmaCodigoElasticDto> MapearTurma(IEnumerable<Infra.Dtos.Questionario.TurmaElasticDto> dadosCompletosTurmas)
        {
            return dadosCompletosTurmas.Select(x => new TurmaCodigoElasticDto(x.CodigoTurma, x.NomeTurma, x.AnoTurma)).Distinct();
        }

        private async Task<IEnumerable<Infra.Dtos.Questionario.TurmaElasticDto>> ObterTurmasPorCodigosNoElastic(IEnumerable<int> codigosTurma, IEnumerable<string> codigoUes, CancellationToken cancellationToken)
        {
            var dados = await _repositoriosElastic.RepositorioElasticTurma.ObterTurmasPorIds(codigosTurma, cancellationToken);
            var dadosPorUE = dados.Where(x => codigoUes.Contains(x.CodigoEscola)).ToList();
            return dadosPorUE;
        }

        private static List<int> ObterCodigosAlunos(IEnumerable<ExtracaoConsultaSondagemLpEscritaDto> responstas)
        {
            return responstas.Select(x => Convert.ToInt32(x.CodigoEolEstudante)!).Distinct().ToList() ?? new List<int>();
        }

        private async Task<IEnumerable<ExtracaoConsultaSondagemLpEscritaDto>> ObterExtracaoDadosRespostasAsync(string dreId, int modalidadeId, Dominio.Entidades.ComponenteCurricular componenteCurricular, CancellationToken cancellationToken)
        {
            if (componenteCurricular == null) return new List<ExtracaoConsultaSondagemLpEscritaDto>();
            return await _repositoriosSondagem.RepositorioRespostaAluno.ObterExtracaoDadosRespostasAsync(modalidadeId, componenteCurricular!.Id, dreId, cancellationToken) ?? new List<ExtracaoConsultaSondagemLpEscritaDto>();
        }

        private async Task<IEnumerable<Dominio.Entidades.ComponenteCurricular>> ObterComponentesCurriculares(CancellationToken cancellationToken)
        {
            return await _repositoriosSondagem.RepositorioComponenteCurricular.ListarAsync(cancellationToken);
        }


        private async Task<IEnumerable<ExtracaoSondagemLpEscritaDto>> MapearAquivo(
                    List<ExtracaoSondagemLpEscritaDto> lista,
                    IEnumerable<ExtracaoConsultaSondagemLpEscritaDto> responstas,
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
            var lista = dados.ToList();
            var propriedades = typeof(ExtracaoSondagemLpEscritaDto).GetProperties();

            var gruposPorDre = lista
                .GroupBy(x => x.NomeDre)
                .ToList();

            var nomesAbas = new HashSet<string>();

            foreach (var grupo in gruposPorDre)
            {
                var nomeAba = SanitizarNomeAbaUnico(grupo.Key, nomesAbas);
                nomesAbas.Add(nomeAba);

                var worksheet = workbook.Worksheets.Add(nomeAba);

                for (int i = 0; i < propriedades.Length; i++)
                {
                    var prop = propriedades[i];
                    var nameAttr = prop.GetCustomAttribute<NameAttribute>();
                    var headerText = nameAttr?.Names?.FirstOrDefault() ?? prop.Name;
                    worksheet.Cell(1, i + 1).Value = headerText;
                }

                int rowExcel = 2;
                foreach (var item in grupo)
                {
                    for (int col = 0; col < propriedades.Length; col++)
                    {
                        var valor = propriedades[col].GetValue(item);
                        worksheet.Cell(rowExcel, col + 1).Value = valor?.ToString() ?? string.Empty;
                    }
                    rowExcel++;
                }
            }

            workbook.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        private static string SanitizarNomeAbaUnico(string nomeOriginal, HashSet<string> nomesExistentes)
        {
            if (string.IsNullOrEmpty(nomeOriginal))
                return "Sem DRE";

            var nomeLimpo = nomeOriginal.Replace("DIRETORIA REGIONAL DE EDUCACAO ", "", StringComparison.OrdinalIgnoreCase).Trim();

            if (string.IsNullOrEmpty(nomeLimpo))
                nomeLimpo = nomeOriginal;

            nomeLimpo = System.Text.RegularExpressions.Regex.Replace(
                nomeLimpo,
                @"[:\/\?\*|$$|]",
                ""
            );

            if (nomeLimpo.Length > 31)
                nomeLimpo = nomeLimpo.Substring(0, 31).TrimEnd();

            if (nomesExistentes.Contains(nomeLimpo))
            {
                int contador = 1;
                string nomeComNumero;
                do
                {
                    var sufixo = $"_{contador}";
                    nomeComNumero = nomeLimpo.Length + sufixo.Length > 31
                        ? nomeLimpo.Substring(0, 31 - sufixo.Length) + sufixo
                        : nomeLimpo + sufixo;
                    contador++;
                } while (nomesExistentes.Contains(nomeComNumero));

                return nomeComNumero;
            }

            return nomeLimpo;
        }

        private async Task<IEnumerable<AlunoEolDto>> ObterAlunos(List<int> codigoAlunos, IEnumerable<string?> codigoUes, IEnumerable<int> codigoTurmas, CancellationToken cancellationToken)
        {
            var retorno = new List<AlunoEolDto>();
            if (codigoAlunos.Count == 0)
                return retorno;

            var dados = await _repositorioSondagemRelatorioPorTodasTurma.DadosAlunosService.ObterDadosAlunosPorCodigoUe(codigoAlunos, DateTime.Now.Year, cancellationToken);
            if (dados.Any())
                retorno.AddRange(dados);

            var dadosPorUE = retorno.Where(d => codigoUes.Contains(d.CodigoEscola)).ToList();
            var dadosPorTurma = dadosPorUE.Where(d => codigoTurmas.Contains(d.CodigoTurma)).ToList();
            return dadosPorTurma;
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
