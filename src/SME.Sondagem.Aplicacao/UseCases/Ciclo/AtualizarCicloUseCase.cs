using SME.Sondagem.Aplicacao.Interfaces.Ciclo;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Ciclo;

namespace SME.Sondagem.Aplicacao.UseCases.Ciclo;

public class AtualizarCicloUseCase : IAtualizarCicloUseCase
{
    private readonly IRepositorioCiclo cicloRepositorio;

    public AtualizarCicloUseCase(IRepositorioCiclo cicloRepositorio)
    {
        this.cicloRepositorio = cicloRepositorio;
    }

    public async Task<CicloDto?> ExecutarAsync(long id, CicloDto cicloDto, CancellationToken cancellationToken = default)
    {
        var cicloExistente = await cicloRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (cicloExistente == null)
            return null;

        cicloExistente.Atualizar(cicloDto.DescCiclo, cicloDto.CodCicloEnsinoEol);
        cicloExistente.AlteradoEm = DateTime.Now;
        cicloExistente.AlteradoPor = cicloDto.AlteradoPor;
        cicloExistente.AlteradoRF = cicloDto.AlteradoRF;

        var sucesso = await cicloRepositorio.AtualizarAsync(cicloExistente, cancellationToken: cancellationToken);
        
        if (!sucesso)
            return null;

        return new CicloDto
        {
            Id = cicloExistente.Id,
            DescCiclo = cicloExistente.DescCiclo,
            CodCicloEnsinoEol = cicloExistente.CodCicloEnsinoEol,
            CriadoEm = cicloExistente.CriadoEm,
            CriadoPor = cicloExistente.CriadoPor,
            CriadoRF = cicloExistente.CriadoRF,
            AlteradoEm = cicloExistente.AlteradoEm,
            AlteradoPor = cicloExistente.AlteradoPor,
            AlteradoRF = cicloExistente.AlteradoRF
        };
    }
}