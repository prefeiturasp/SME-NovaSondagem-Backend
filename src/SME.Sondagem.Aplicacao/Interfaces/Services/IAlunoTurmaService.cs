using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IAlunoTurmaService
    {
        Task<IEnumerable<DadosAlunoPorTurmaDTO>> InformacoesAlunosPorTurma(long codigoTurma, CancellationToken cancellationToken = default);
    }
}