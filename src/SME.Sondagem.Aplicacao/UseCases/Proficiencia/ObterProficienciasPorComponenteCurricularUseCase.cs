using System.Net;
using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infra.Dtos.Proficiencia;
using SME.Sondagem.Infra.Exceptions;

namespace SME.Sondagem.Aplicacao.UseCases.Proficiencia;

public class ObterProficienciasPorComponenteCurricularUseCase :IObterProficienciasPorComponenteCurricularUseCase
{
    private readonly IRepositorioProficiencia proficienciaRepositorio;

    public ObterProficienciasPorComponenteCurricularUseCase(IRepositorioProficiencia proficienciaRepositorio)
    {
        this.proficienciaRepositorio =
            proficienciaRepositorio ?? throw new ArgumentNullException(nameof(proficienciaRepositorio));
    }

    public async Task<IEnumerable<ProficienciaDto>> ExecutarAsync(long componenteCurricularId, long modalidadeId,CancellationToken cancellationToken = default)
    {
        if(modalidadeId == 0)
            throw new NegocioException(MensagemNegocioComuns.MODALIDADE_ID_MAIOR_QUE_ZERO,HttpStatusCode.NotFound);
        
        if(componenteCurricularId == 0)
            throw new NegocioException(MensagemNegocioComuns.INFORMAR_ID_MAIOR_QUE_ZERO,HttpStatusCode.NotFound);
            
        var lista = await proficienciaRepositorio.ObterProeficienciaPorComponenteCurricular(componenteCurricularId,modalidadeId,
                cancellationToken);
            return !lista.Any() ? throw new NegocioException(MensagemNegocioComuns.NENHUM_REGISTRO_ENCONTRADO,HttpStatusCode.NotFound) : lista;
    }
}