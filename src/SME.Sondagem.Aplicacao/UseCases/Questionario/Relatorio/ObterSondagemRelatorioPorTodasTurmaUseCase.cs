using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Queries.Alunos.DadosAlunos;
using SME.Sondagem.Aplicacao.Queries.ObterTodasTurmaElastic;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Extensions;
using SME.Sondagem.Infrastructure.Dtos;
using System.Globalization;
using System.Text;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio
{
    public class ObterSondagemRelatorioPorTodasTurmaUseCase : IObterSondagemRelatorioPorTodasTurmaUseCase
    {
        private readonly IMediator _mediator;
        private readonly IRepositorioRespostaAluno _repositorioRespostaAluno; 
        private readonly IRepositorioComponenteCurricular _repositorioComponenteCurricular; 
        private readonly IUeComDreEolService _ueComDreEolService;


        public ObterSondagemRelatorioPorTodasTurmaUseCase(IMediator mediator, IUeComDreEolService ueComDreEolService,
            IRepositorioRespostaAluno repositorioRespostaAluno,
            IRepositorioComponenteCurricular repositorioComponenteCurricular
            )
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _ueComDreEolService = ueComDreEolService ?? throw new ArgumentNullException(nameof(ueComDreEolService));
            _repositorioRespostaAluno = repositorioRespostaAluno ?? throw new ArgumentNullException(nameof(repositorioRespostaAluno));
            _repositorioComponenteCurricular = repositorioComponenteCurricular ?? throw new ArgumentNullException(nameof(repositorioComponenteCurricular));
        }

        public async Task<FileResultDto> ObterSondagemRelatorio(CancellationToken cancellationToken)
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
            var dadosArquivo = MapearAquivo(lista, respostas, uesComDre, turmasCodigoNome, dadosAlunos);
            var csvStream = GerarCsv(dadosArquivo);

            return new FileResultDto(csvStream, "text/csv", $"sondagem-lp-escrita-{DateTime.Now:yyyy-MM-dd}.csv");
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
            return await _mediator.Send(new ObterTodasTurmaElasticQuery(dadosAlunos.Select(x => x.CodigoTurma)), cancellationToken);
        }

        private static List<string> ObterCodigosAlunos(IEnumerable<ExtracaoSondagemLpEscritaDto> responstas)
        {
            return responstas.Select(x => x.CodigoEolEstudante!).ToList() ?? new List<string>();
        }

        private async Task<IEnumerable<ExtracaoSondagemLpEscritaDto>> ObterExtracaoDadosRespostasAsync(int modalidadeIdFundamental, Dominio.Entidades.ComponenteCurricular componenteLp, CancellationToken cancellationToken)
        {
            if (componenteLp == null) return new List<ExtracaoSondagemLpEscritaDto>();
            return await _repositorioRespostaAluno.ObterExtracaoDadosRespostasAsync(modalidadeIdFundamental, componenteLp!.Id, cancellationToken);
        }

        private async Task<Dominio.Entidades.ComponenteCurricular?> ObterPorNomeModalidade(string NOME_COMPONENTE, int modalidadeIdFundamental, CancellationToken cancellationToken)
        {
            return await _repositorioComponenteCurricular.ObterPorNomeModalidade(NOME_COMPONENTE, modalidadeIdFundamental.ToString(), cancellationToken);
        }

        private static IEnumerable<ExtracaoSondagemLpEscritaDto> MapearAquivo(List<ExtracaoSondagemLpEscritaDto> lista, IEnumerable<ExtracaoSondagemLpEscritaDto> responstas,IEnumerable<UeComDreEolDto> uesComDre, 
            IEnumerable<TurmaCodigoElasticDto> turmasCodigoNome, IEnumerable<AlunoEolDto> dadosAlunos)
        {

            foreach (var responsta in responstas)
            {
                 var aluno = dadosAlunos.FirstOrDefault(x => x.CodigoAluno.ToString() == responsta.CodigoEolEstudante);
                 if (aluno != null)
                 {
                     var turma = turmasCodigoNome.FirstOrDefault(t =>
                         t.CodigoTurma == aluno.CodigoTurma);

                     var ueDre = uesComDre.FirstOrDefault(e => e.CodigoEscola == aluno?.CodigoEscola);

                     var item = new ExtracaoSondagemLpEscritaDto
                     {
                         NomeDre = ueDre?.NomeDRE ?? string.Empty,
                         Bimestre = responsta?.Bimestre,
                         CodigoDre = ueDre?.CodigoDRE ?? string.Empty,
                         NomeEscola = ueDre?.NomeEscola ?? string.Empty,
                         CodigoEolEscola = ueDre?.CodigoEscola ?? string.Empty,
                         NomeTurma = turma?.NomeTurma ?? string.Empty,
                         CodigoEolEstudante = aluno.CodigoAluno.ToString(),
                         NomeEstudanteEstudante = aluno?.NomeAluno ?? string.Empty,
                         ComponenteCurricular = responsta?.ComponenteCurricular,
                         Proficiencia = responsta?.Proficiencia,
                         Ano = turma?.AnoTurma,
                         Questao =  responsta?.Questao,
                         Resposta =  responsta?.Resposta,
                         Legenda =  responsta?.Legenda,
                         Modalidade =  ObterNomeModalidade(responsta?.ModalidadeId ?? 0),
                         ModalidadeId =  responsta?.ModalidadeId,
                     };
                     lista.Add(item);
                 }
            }


            return lista.OrderBy(x => x.NomeEstudanteEstudante);
        }

        private static MemoryStream GerarCsv(IEnumerable<ExtracaoSondagemLpEscritaDto> dados)
        {
            var memoryStream = new MemoryStream();

            using var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);
            using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
            });

            csv.WriteRecords(dados);
            writer.Flush();

            memoryStream.Position = 0;
            return memoryStream;
        }

        private  static string ObterNomeModalidade(int modalidadeId)
        {
            var item = Enum.GetValues<Modalidade>()
                .Select(m => new
                {
                    Id = (int)m,
                    Name = m.Nome() ?? m.ToString()
                })
                .FirstOrDefault(x => x.Id == modalidadeId)?.Name ?? string.Empty;


            return item;
        }
        private async Task<IEnumerable<AlunoEolDto>> ObterAlunos(List<string> codigoAlunos,CancellationToken cancellationToken)
        {
            var retorno = new List<AlunoEolDto>();
            if(codigoAlunos.Count == 0)
                return retorno;

               var dados =  await _mediator.Send(new DadosAlunosServiceQuery(codigoAlunos), cancellationToken);
               if(dados.Any())
                   retorno.AddRange(dados);
            

            return retorno;
        }
        private async Task<IEnumerable<UeComDreEolDto>> BuscarUesDres(IEnumerable<string> codigosUes)
        {
            var retorno = new List<UeComDreEolDto>();

            if(!codigosUes.Any())
                return retorno;

            var busca = await _ueComDreEolService.ObterUesComDrePorCodigosUes(codigosUes);

            if(busca.Any())
                retorno.AddRange(busca);

            return retorno;
        }
    }
}
