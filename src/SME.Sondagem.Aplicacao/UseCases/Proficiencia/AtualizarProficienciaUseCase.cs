using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Proficiencia;
using SME.Sondagem.Infra.Extensions;

namespace SME.Sondagem.Aplicacao.UseCases.Proficiencia;

public class AtualizarProficienciaUseCase : IAtualizarProficienciaUseCase
{
    private readonly IRepositorioProficiencia proficienciaRepositorio;

    public AtualizarProficienciaUseCase(IRepositorioProficiencia proficienciaRepositorio)
    {
        this.proficienciaRepositorio =
            proficienciaRepositorio ?? throw new ArgumentNullException(nameof(proficienciaRepositorio));
    }

    public async Task<ProficienciaDto?> ExecutarAsync(long id, ProficienciaDto proficienciaDto,CancellationToken cancellationToken = default)
    {
        var proficienciaExistente = await proficienciaRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (proficienciaExistente == null)
            return null;

        proficienciaExistente.Atualizar(proficienciaDto.Nome, proficienciaDto.ComponenteCurricularId,proficienciaDto.ModalidadeId);

        var sucesso = await proficienciaRepositorio.SalvarAsync(proficienciaExistente, cancellationToken: cancellationToken);
        
        if (sucesso == 0)
            return null;

        return new ProficienciaDto
        {
            Id = proficienciaExistente.Id,
            Nome = proficienciaExistente.Nome,
            ComponenteCurricularId = proficienciaExistente.ComponenteCurricularId,
            ModalidadeId =  proficienciaExistente.ModalidadeId,
            Modalidade = ObterNomeModalidade(proficienciaExistente.ModalidadeId),
            CriadoEm = proficienciaExistente.CriadoEm,
            CriadoPor = proficienciaExistente.CriadoPor,
            CriadoRF = proficienciaExistente.CriadoRF,
            AlteradoEm = proficienciaExistente.AlteradoEm,
            AlteradoPor = proficienciaExistente.AlteradoPor,
            AlteradoRF = proficienciaExistente.AlteradoRF
        };
    }

    private static string ObterNomeModalidade(int modalidadeId)
    {
        var nome = Enum
            .GetValues<Modalidade>().FirstOrDefault(c => (int)c == modalidadeId);

        return nome.Nome();
    }
}