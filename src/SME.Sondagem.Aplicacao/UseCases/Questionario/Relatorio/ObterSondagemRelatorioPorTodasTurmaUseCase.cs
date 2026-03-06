using MediatR;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Queries.Alunos.DadosAlunos;
using SME.Sondagem.Aplicacao.Queries.ObterTodasTurmaElastic;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using SME.Sondagem.Infrastructure.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio
{
    public class ObterSondagemRelatorioPorTodasTurmaUseCase : IObterSondagemRelatorioPorTodasTurmaUseCase
    {
        protected readonly RepositoriosSondagem _repositoriosSondagem;
        protected readonly IAlunoPapService _alunoPapService;
        protected readonly IControleAcessoService _controleAcessoService;
        protected readonly IServicoUsuario _servicoUsuario;
        private readonly IMediator _mediator;
        private readonly IUeComDreEolService _ueComDreEolService;


        public ObterSondagemRelatorioPorTodasTurmaUseCase(IMediator mediator, IUeComDreEolService ueComDreEolService,
            RepositoriosSondagem repositoriosSondagem, IAlunoPapService alunoPapService, IControleAcessoService controleAcessoService, IServicoUsuario servicoUsuario)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _ueComDreEolService = ueComDreEolService ?? throw new ArgumentNullException(nameof(ueComDreEolService));
            _repositoriosSondagem = repositoriosSondagem ?? throw new ArgumentNullException(nameof(repositoriosSondagem));
            _alunoPapService = alunoPapService ?? throw new ArgumentNullException(nameof(alunoPapService));
            _controleAcessoService = controleAcessoService ?? throw new ArgumentNullException(nameof(controleAcessoService));
            _servicoUsuario = servicoUsuario ?? throw new ArgumentNullException(nameof(servicoUsuario));
            _servicoUsuario = servicoUsuario ?? throw new ArgumentNullException(nameof(servicoUsuario));
        }

        //public Task<MemoryStream> ObterSondagemRelatorio(CancellationToken cancellationToken)
        public async Task ObterSondagemRelatorioTodas(CancellationToken cancellationToken)
        {
            var dadosCompletosTurmas = await _mediator.Send(new ObterTodasTurmaElasticQuery(), cancellationToken);
            var uesComDre = await BuscarUesDres(dadosCompletosTurmas.Select(x => x.CodigoEscola).Distinct());
            var turmasCodigoNome = dadosCompletosTurmas.Select(x => new TurmaCodigoElasticDto(x.CodigoTurma, x.NomeFiltro)).Distinct();
            var componentes = dadosCompletosTurmas.Select(x => x.Componentes.Select(s => new ComponenteCurricularElasticDto(s.Nome, s.Codigo))).Distinct();
            var dadosAlunos = await ObterAlunos(uesComDre.Select(x => x.CodigoEscola),cancellationToken);
            var codigosAlunos = dadosAlunos.Select(x => x.CodigoAluno);
        }

        private async Task<IEnumerable<AlunoEolDto>> ObterAlunos(IEnumerable<string> ueCodigos,CancellationToken cancellationToken)
        {
            var retorno = new List<AlunoEolDto>();
            if(!ueCodigos.Any())
                return retorno;
            foreach (var ue in ueCodigos)
            {
               var dados =  await _mediator.Send(new DadosAlunosServiceQuery(ue), cancellationToken);
               if(dados.Any())
                   retorno.AddRange(dados);
            }

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
