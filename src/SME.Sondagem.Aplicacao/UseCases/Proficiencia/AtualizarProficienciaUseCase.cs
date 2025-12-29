using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Proficiencia;

namespace SME.Sondagem.Aplicacao.UseCases.Proficiencia;

public class AtualizarProficienciaUseCase : IAtualizarProficienciaUseCase
{
    private readonly IRepositorioProficiencia proficienciaRepositorio;

    public AtualizarProficienciaUseCase(IRepositorioProficiencia proficienciaRepositorio)
    {
        this.proficienciaRepositorio = proficienciaRepositorio;
    }

    public async Task<bool> ExecutarAsync(long id, ProficienciaDto proficienciaDto, CancellationToken cancellationToken = default)
    {
        var proficienciaExistente = await proficienciaRepositorio.ObterPorIdAsync(id);

        if (proficienciaExistente == null)
            return false;

        proficienciaExistente.Atualizar(proficienciaDto.Nome, proficienciaDto.ComponenteCurricularId);
        proficienciaExistente.AlteradoEm = DateTime.Now;
        proficienciaExistente.AlteradoPor = proficienciaDto.AlteradoPor;
        proficienciaExistente.AlteradoRF = proficienciaDto.AlteradoRF;

        return await proficienciaRepositorio.AtualizarAsync(proficienciaExistente, cancellationToken: cancellationToken);
    }
}